using System;

namespace MackySoft.MasterTools
{
	public interface IJsonDeserializer
	{
		/// <summary>
		/// Deserialize the json to the specified type.
		/// </summary>
		object Deserialize (Type type, string json);
	}
}
