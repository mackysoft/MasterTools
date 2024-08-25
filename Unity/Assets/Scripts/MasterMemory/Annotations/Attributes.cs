using System;

namespace MasterMemory
{
    /// <summary>
    /// Attribute to mark a class as a memory table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MemoryTableAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryTableAttribute"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public MemoryTableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }

    /// <summary>
    /// Attribute to mark a property as a primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets the order of the key.
        /// </summary>
        public int KeyOrder { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="keyOrder">Order of the key.</param>
        public PrimaryKeyAttribute(int keyOrder = 0)
        {
            this.KeyOrder = keyOrder;
        }
    }

    /// <summary>
    /// Attribute to mark a property as a secondary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SecondaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets the index number of the secondary key.
        /// </summary>
        public int IndexNo { get; }

        /// <summary>
        /// Gets the order of the key.
        /// </summary>
        public int KeyOrder { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecondaryKeyAttribute"/> class.
        /// </summary>
        /// <param name="indexNo">Index number of the secondary key.</param>
        /// <param name="keyOrder">Order of the key.</param>
        public SecondaryKeyAttribute(int indexNo, int keyOrder = 0)
        {
            this.IndexNo = indexNo;
            this.KeyOrder = keyOrder;
        }
    }

    /// <summary>
    /// Attribute to mark a property as non-unique.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NonUniqueAttribute : Attribute
    {

    }

    /// <summary>
    /// Attribute to specify string comparison options for a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringComparisonOptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the string comparison option.
        /// </summary>
        public StringComparison StringComparison { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringComparisonOptionAttribute"/> class.
        /// </summary>
        /// <param name="stringComparison">String comparison option.</param>
        public StringComparisonOptionAttribute(StringComparison stringComparison)
        {
            this.StringComparison = stringComparison;
        }
    }
}