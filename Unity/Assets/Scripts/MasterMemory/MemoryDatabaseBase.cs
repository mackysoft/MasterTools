using MasterMemory.Internal;
using MessagePack;
using MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Buffers;
using MasterMemory.Validation;

namespace MasterMemory
{
    /// <summary>
    /// Base class for memory database operations.
    /// </summary>
    public abstract class MemoryDatabaseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDatabaseBase"/> class.
        /// </summary>
        protected MemoryDatabaseBase()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryDatabaseBase"/> class with the specified parameters.
        /// </summary>
        /// <param name="databaseBinary">The binary data of the database.</param>
        /// <param name="internString">Whether to intern strings.</param>
        /// <param name="formatterResolver">The formatter resolver to use.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism.</param>
        public MemoryDatabaseBase(byte[] databaseBinary, bool internString = true, IFormatterResolver formatterResolver = null, int maxDegreeOfParallelism = 1)
        {
            var reader = new MessagePackReader(databaseBinary);
            var formatter = new DictionaryFormatter<string, (int, int)>();

            var header = formatter.Deserialize(ref reader, HeaderFormatterResolver.StandardOptions);
            var resolver = formatterResolver ?? MessagePackSerializer.DefaultOptions.Resolver;
            if (internString)
            {
                resolver = new InternStringResolver(resolver);
            }
            if (maxDegreeOfParallelism < 1)
            {
                maxDegreeOfParallelism = 1;
            }

            Init(header, databaseBinary.AsMemory((int)reader.Consumed), MessagePackSerializer.DefaultOptions.WithResolver(resolver).WithCompression(MessagePackCompression.Lz4Block), maxDegreeOfParallelism);
        }

        /// <summary>
        /// Extracts table data from the database.
        /// </summary>
        /// <typeparam name="T">The type of the table elements.</typeparam>
        /// <typeparam name="TView">The type of the view to create.</typeparam>
        /// <param name="header">The header dictionary containing table offsets and counts.</param>
        /// <param name="databaseBinary">The binary data of the database.</param>
        /// <param name="options">The MessagePack serializer options.</param>
        /// <param name="createView">The function to create a view from the table data.</param>
        /// <returns>The created view.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the type is not annotated with <see cref="MemoryTableAttribute"/>.</exception>
        protected static TView ExtractTableData<T, TView>(Dictionary<string, (int offset, int count)> header, ReadOnlyMemory<byte> databaseBinary, MessagePackSerializerOptions options, Func<T[], TView> createView)
        {
            var tableName = typeof(T).GetCustomAttribute<MemoryTableAttribute>();
            if (tableName == null) throw new InvalidOperationException("Type is not annotated MemoryTableAttribute. Type:" + typeof(T).FullName);

            if (header.TryGetValue(tableName.TableName, out var segment))
            {
                var data = MessagePackSerializer.Deserialize<T[]>(databaseBinary.Slice(segment.offset, segment.count), options);
                return createView(data);
            }
            else
            {
                // return empty
                var data = Array.Empty<T>();
                return createView(data);
            }
        }

        /// <summary>
        /// Initializes the database with the specified parameters.
        /// </summary>
        /// <param name="header">The header dictionary containing table offsets and counts.</param>
        /// <param name="databaseBinary">The binary data of the database.</param>
        /// <param name="options">The MessagePack serializer options.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism.</param>
        protected abstract void Init(Dictionary<string, (int offset, int count)> header, ReadOnlyMemory<byte> databaseBinary, MessagePackSerializerOptions options, int maxDegreeOfParallelism);

        /// <summary>
        /// Gets information about the tables in the database.
        /// </summary>
        /// <param name="databaseBinary">The binary data of the database.</param>
        /// <param name="storeTableData">Whether to store table data in the returned <see cref="TableInfo"/> objects.</param>
        /// <returns>An array of <see cref="TableInfo"/> objects containing information about the tables.</returns>
        public static TableInfo[] GetTableInfo(byte[] databaseBinary, bool storeTableData = true)
        {
            var formatter = new DictionaryFormatter<string, (int, int)>();
            var reader = new MessagePackReader(databaseBinary);
            var header = formatter.Deserialize(ref reader, HeaderFormatterResolver.StandardOptions);

            return header.Select(x => new TableInfo(x.Key, x.Value.Item2, storeTableData ? databaseBinary : null, x.Value.Item1)).ToArray();
        }

        /// <summary>
        /// Validates the specified table.
        /// </summary>
        /// <typeparam name="TElement">The type of the table elements.</typeparam>
        /// <param name="table">The table to validate.</param>
        /// <param name="database">The validation database.</param>
        /// <param name="pkName">The name of the primary key.</param>
        /// <param name="pkSelector">The primary key selector.</param>
        /// <param name="result">The validation result.</param>
        protected void ValidateTable<TElement>(IReadOnlyList<TElement> table, ValidationDatabase database, string pkName, Delegate pkSelector, ValidateResult result)
        {
            var onceCalled = new System.Runtime.CompilerServices.StrongBox<bool>(false);
            foreach (var item in table)
            {
                if (item is IValidatable<TElement> validatable)
                {
                    var validator = new Validator<TElement>(database, item, result, onceCalled, pkName, pkSelector);
                    validatable.Validate(validator);
                }
            }
        }
    }

    /// <summary>
    /// Diagnostic info of MasterMemory's table.
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the size of the table.
        /// </summary>
        public int Size { get; }

        byte[] binaryData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class with the specified parameters.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="size">The size of the table.</param>
        /// <param name="rawBinary">The raw binary data of the table.</param>
        /// <param name="offset">The offset of the table data in the binary array.</param>
        public TableInfo(string tableName, int size, byte[] rawBinary, int offset)
        {
            TableName = tableName;
            Size = size;
            if (rawBinary != null)
            {
                this.binaryData = new byte[size];
                Array.Copy(rawBinary, offset, binaryData, 0, size);
            }
        }

        /// <summary>
        /// Dumps the table data as a JSON string.
        /// </summary>
        /// <returns>A JSON string representing the table data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the table data is not stored.</exception>
        public string DumpAsJson()
        {
            return DumpAsJson(MessagePackSerializer.DefaultOptions);
        }

        /// <summary>
        /// Dumps the table data as a JSON string with the specified serializer options.
        /// </summary>
        /// <param name="options">The MessagePack serializer options.</param>
        /// <returns>A JSON string representing the table data.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the table data is not stored.</exception>
        public string DumpAsJson(MessagePackSerializerOptions options)
        {
            if (binaryData == null)
            {
                throw new InvalidOperationException("DumpAsJson can only call from GetTableInfo(storeTableData = true).");
            }

            return MessagePackSerializer.ConvertToJson(binaryData, options.WithCompression(MessagePackCompression.Lz4Block));
        }
    }
}