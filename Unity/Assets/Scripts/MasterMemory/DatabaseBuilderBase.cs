using MasterMemory.Internal;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MasterMemory
{
    /// <summary>
    /// Base class for building a database with memory tables.
    /// </summary>
    public abstract class DatabaseBuilderBase
    {
        readonly ByteBufferWriter bufferWriter = new ByteBufferWriter();

        // TableName, (Offset, Count)
        readonly Dictionary<string, (int offset, int count)> header = new Dictionary<string, (int offset, int count)>();
        readonly MessagePackSerializerOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBuilderBase"/> class with specified MessagePack options.
        /// </summary>
        /// <param name="options">The MessagePack serializer options.</param>
        public DatabaseBuilderBase(MessagePackSerializerOptions options)
        {
            // options keep null to lazily use default options
            if (options != null)
            {
                options = options.WithCompression(MessagePackCompression.Lz4Block);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBuilderBase"/> class with specified formatter resolver.
        /// </summary>
        /// <param name="resolver">The formatter resolver.</param>
        public DatabaseBuilderBase(IFormatterResolver resolver)
        {
            if (resolver != null)
            {
                this.options = MessagePackSerializer.DefaultOptions
                    .WithCompression(MessagePackCompression.Lz4Block)
                    .WithResolver(resolver);
            }
        }

        /// <summary>
        /// Appends data to the database.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="datasource">The data source.</param>
        /// <param name="indexSelector">The function to select the index.</param>
        /// <param name="comparer">The comparer for the key.</param>
        /// <exception cref="InvalidOperationException">Thrown when the type is not annotated with MemoryTableAttribute or the table name is already appended.</exception>
        protected void AppendCore<T, TKey>(IEnumerable<T> datasource, Func<T, TKey> indexSelector, IComparer<TKey> comparer)
        {
            var tableName = typeof(T).GetCustomAttribute<MemoryTableAttribute>();
            if (tableName == null) throw new InvalidOperationException("Type is not annotated MemoryTableAttribute. Type:" + typeof(T).FullName);

            if (header.ContainsKey(tableName.TableName))
            {
                throw new InvalidOperationException("TableName is already appended in builder. TableName: " + tableName.TableName + " Type:" + typeof(T).FullName);
            }

            if (datasource == null) return;

            // sort(as indexed data-table)
            var source = FastSort(datasource, indexSelector, comparer);

            // write data and store header-data.
            var useOption = options ?? MessagePackSerializer.DefaultOptions.WithCompression(MessagePackCompression.Lz4Block);

            var offset = bufferWriter.CurrentOffset;
            MessagePackSerializer.Serialize(bufferWriter, source, useOption);

            header.Add(tableName.TableName, (offset, bufferWriter.CurrentOffset - offset));
        }

        /// <summary>
        /// Sorts the data source using the specified index selector and comparer.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements in the data source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="datasource">The data source.</param>
        /// <param name="indexSelector">The function to select the index.</param>
        /// <param name="comparer">The comparer for the key.</param>
        /// <returns>The sorted array of elements.</returns>
        static TElement[] FastSort<TElement, TKey>(IEnumerable<TElement> datasource, Func<TElement, TKey> indexSelector, IComparer<TKey> comparer)
        {
            var collection = datasource as ICollection<TElement>;
            if (collection != null)
            {
                var array = new TElement[collection.Count];
                var sortSource = new TKey[collection.Count];
                var i = 0;
                foreach (var item in collection)
                {
                    array[i] = item;
                    sortSource[i] = indexSelector(item);
                    i++;
                }
                Array.Sort(sortSource, array, 0, collection.Count, comparer);
                return array;
            }
            else
            {
                var array = new ExpandableArray<TElement>(null);
                var sortSource = new ExpandableArray<TKey>(null);
                foreach (var item in datasource)
                {
                    array.Add(item);
                    sortSource.Add(indexSelector(item));
                }

                Array.Sort(sortSource.items, array.items, 0, array.count, comparer);

                Array.Resize(ref array.items, array.count);
                return array.items;
            }
        }

        /// <summary>
        /// Builds the database and returns the byte array representation.
        /// </summary>
        /// <returns>The byte array representation of the database.</returns>
        public byte[] Build()
        {
            using (var ms = new MemoryStream())
            {
                WriteToStream(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Writes the database to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void WriteToStream(Stream stream)
        {
            MessagePackSerializer.Serialize(stream, header, HeaderFormatterResolver.StandardOptions);
            MemoryMarshal.TryGetArray(bufferWriter.WrittenMemory, out var segment);
            stream.Write(segment.Array, segment.Offset, segment.Count);
        }
    }
}