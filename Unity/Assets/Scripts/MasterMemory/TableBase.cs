using MasterMemory.Internal;
using MasterMemory.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MasterMemory
{
    /// <summary>
    /// Base class for table operations.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the table.</typeparam>
    public abstract class TableBase<TElement>
    {
        protected readonly TElement[] data;

        /// <summary>
        /// Gets the number of elements in the table.
        /// </summary>
        public int Count => data.Length;

        /// <summary>
        /// Gets a view of all elements in the table.
        /// </summary>
        public RangeView<TElement> All => new RangeView<TElement>(data, 0, data.Length - 1, true);

        /// <summary>
        /// Gets a reverse view of all elements in the table.
        /// </summary>
        public RangeView<TElement> AllReverse => new RangeView<TElement>(data, 0, data.Length - 1, false);

        /// <summary>
        /// Gets the raw data array. Use with caution.
        /// </summary>
        /// <returns>The raw data array.</returns>
        public TElement[] GetRawDataUnsafe() => data;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableBase{TElement}"/> class.
        /// </summary>
        /// <param name="sortedData">The sorted data array.</param>
        public TableBase(TElement[] sortedData)
        {
            this.data = sortedData;
        }

        /// <summary>
        /// Validates that all elements in the array have unique keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to validate.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="message">The validation message.</param>
        /// <param name="resultSet">The validation result set.</param>
        static protected void ValidateUniqueCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, string message, ValidateResult resultSet)
        {
            var set = new HashSet<TKey>();
            foreach (var item in indexArray)
            {
                var v = keySelector(item);
                if (!set.Add(v))
                {
                    resultSet.AddFail(typeof(TElement), "Unique failed: " + message + ", value = " + v, item);
                }
            }
        }

        /// <summary>
        /// Clones and sorts the data array by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexSelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for sorting.</param>
        /// <returns>The sorted array.</returns>
        protected TElement[] CloneAndSortBy<TKey>(Func<TElement, TKey> indexSelector, IComparer<TKey> comparer)
        {
            var array = new TElement[data.Length];
            var sortSource = new TKey[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                array[i] = data[i];
                sortSource[i] = indexSelector(data[i]);
            }

            Array.Sort(sortSource, array, 0, array.Length, comparer);
            return array;
        }

        /// <summary>
        /// Throws a <see cref="KeyNotFoundException"/> for the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="key">The key that was not found.</param>
        /// <returns>Does not return.</returns>
        static protected TElement ThrowKeyNotFound<TKey>(TKey key)
        {
            throw new KeyNotFoundException("DataType: " + typeof(TElement).FullName + ", Key: " + key.ToString());
        }

        /// <summary>
        /// Finds a unique element by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="throwIfNotFound">Whether to throw an exception if the key is not found.</param>
        /// <returns>The found element, or the default value if not found.</returns>
        static protected TElement FindUniqueCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key, bool throwIfNotFound = true)
        {
            var index = BinarySearch.FindFirst(indexArray, key, keySelector, comparer);
            if (index != -1)
            {
                return indexArray[index];
            }
            else
            {
                if (throwIfNotFound)
                {
                    ThrowKeyNotFound(key);
                }
                return default;
            }
        }

        /// <summary>
        /// Finds a unique element by integer key.
        /// </summary>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="_">The comparer to use for searching (ignored).</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="throwIfNotFound">Whether to throw an exception if the key is not found.</param>
        /// <returns>The found element, or the default value if not found.</returns>
        static protected TElement FindUniqueCoreInt(TElement[] indexArray, Func<TElement, int> keySelector, IComparer<int> _, int key, bool throwIfNotFound = true)
        {
            var index = BinarySearch.FindFirstIntKey(indexArray, key, keySelector);
            if (index != -1)
            {
                return indexArray[index];
            }
            else
            {
                if (throwIfNotFound)
                {
                    ThrowKeyNotFound(key);
                }
                return default;
            }
        }

        /// <summary>
        /// Tries to find a unique element by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="result">The found element, or the default value if not found.</param>
        /// <returns>True if the element was found, otherwise false.</returns>
        static protected bool TryFindUniqueCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key, out TElement result)
        {
            var index = BinarySearch.FindFirst(indexArray, key, keySelector, comparer);
            if (index != -1)
            {
                result = indexArray[index];
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to find a unique element by integer key.
        /// </summary>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="_">The comparer to use for searching (ignored).</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="result">The found element, or the default value if not found.</param>
        /// <returns>True if the element was found, otherwise false.</returns>
        static protected bool TryFindUniqueCoreInt(TElement[] indexArray, Func<TElement, int> keySelector, IComparer<int> _, int key, out TElement result)
        {
            var index = BinarySearch.FindFirstIntKey(indexArray, key, keySelector);
            if (index != -1)
            {
                result = indexArray[index];
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Finds the closest unique element by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="selectLower">Whether to select the lower closest element.</param>
        /// <returns>The found element, or the default value if not found.</returns>
        static protected TElement FindUniqueClosestCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key, bool selectLower)
        {
            var index = BinarySearch.FindClosest(indexArray, 0, indexArray.Length, key, keySelector, comparer, selectLower);
            return (index != -1) ? indexArray[index] : default(TElement);
        }

        /// <summary>
        /// Finds a range of unique elements by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="min">The minimum key value.</param>
        /// <param name="max">The maximum key value.</param>
        /// <param name="ascendant">Whether the range should be in ascending order.</param>
        /// <returns>The range view of the found elements.</returns>
        static protected RangeView<TElement> FindUniqueRangeCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey min, TKey max, bool ascendant)
        {
            var lo = BinarySearch.FindClosest(indexArray, 0, indexArray.Length, min, keySelector, comparer, false);
            var hi = BinarySearch.FindClosest(indexArray, 0, indexArray.Length, max, keySelector, comparer, true);

            if (lo == -1) lo = 0;
            if (hi == indexArray.Length) hi -= 1;

            return new RangeView<TElement>(indexArray, lo, hi, ascendant);
        }

        /// <summary>
        /// Finds multiple elements by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexKeys">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="key">The key to search for.</param>
        /// <returns>The range view of the found elements.</returns>
        static protected RangeView<TElement> FindManyCore<TKey>(TElement[] indexKeys, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key)
        {
            var lo = BinarySearch.LowerBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
            if (lo == -1) return RangeView<TElement>.Empty;

            var hi = BinarySearch.UpperBound(indexKeys, 0, indexKeys.Length, key, keySelector, comparer);
            if (hi == -1) return RangeView<TElement>.Empty;

            return new RangeView<TElement>(indexKeys, lo, hi, true);
        }

        /// <summary>
        /// Finds the closest multiple elements by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="selectLower">Whether to select the lower closest element.</param>
        /// <returns>The range view of the found elements.</returns>
        static protected RangeView<TElement> FindManyClosestCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey key, bool selectLower)
        {
            var closest = BinarySearch.FindClosest(indexArray, 0, indexArray.Length, key, keySelector, comparer, selectLower);

            if ((closest == -1) || (closest >= indexArray.Length))
                return RangeView<TElement>.Empty;

            return FindManyCore(indexArray, keySelector, comparer, keySelector(indexArray[closest]));
        }

        /// <summary>
        /// Finds a range of multiple elements by key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="indexArray">The array to search.</param>
        /// <param name="keySelector">The function to select the key from an element.</param>
        /// <param name="comparer">The comparer to use for searching.</param>
        /// <param name="min">The minimum key value.</param>
        /// <param name="max">The maximum key value.</param>
        /// <param name="ascendant">Whether the range should be in ascending order.</param>
        /// <returns>The range view of the found elements.</returns>
        static protected RangeView<TElement> FindManyRangeCore<TKey>(TElement[] indexArray, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, TKey min, TKey max, bool ascendant)
        {
            // Empty set when min > max
            if (Comparer<TKey>.Default.Compare(min, max) > 0)
                return RangeView<TElement>.Empty;

            var lo = BinarySearch.LowerBoundClosest(indexArray, 0, indexArray.Length, min, keySelector, comparer);
            var hi = BinarySearch.UpperBoundClosest(indexArray, 0, indexArray.Length, max, keySelector, comparer);

            Debug.Assert(lo >= 0);
            Debug.Assert(hi < indexArray.Length);

            if (hi < lo)
                return RangeView<TElement>.Empty;

            return new RangeView<TElement>(indexArray, lo, hi, ascendant);
        }
    }
}
