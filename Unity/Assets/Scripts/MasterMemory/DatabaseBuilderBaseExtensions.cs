using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMemory
{
    public static class DatabaseBuilderExtensions
    {
        /// <summary>
        /// Appends dynamic data to the database builder.
        /// </summary>
        /// <param name="builder">The database builder instance.</param>
        /// <param name="dataType">The type of data to append.</param>
        /// <param name="tableData">The table data to append.</param>
        /// <exception cref="InvalidOperationException">Thrown when the Append method cannot be found for the specified data type.</exception>
        public static void AppendDynamic(this DatabaseBuilderBase builder, Type dataType, IList<object> tableData)
        {
            var appendMethod = builder.GetType().GetMethods()
                .Where(x => x.Name == "Append")
                .Where(x => x.GetParameters()[0].ParameterType.GetGenericArguments()[0] == dataType)
                .FirstOrDefault();

            if (appendMethod == null)
            {
                throw new InvalidOperationException("Append(IEnumerable<DataType>) can not found. DataType:" + dataType);
            }

            var dynamicArray = Array.CreateInstance(dataType, tableData.Count);
            for (int i = 0; i < tableData.Count; i++)
            {
                dynamicArray.SetValue(Convert.ChangeType(tableData[i], dataType), i);
            }

            appendMethod.Invoke(builder, new object[] { dynamicArray });
        }
    }
}