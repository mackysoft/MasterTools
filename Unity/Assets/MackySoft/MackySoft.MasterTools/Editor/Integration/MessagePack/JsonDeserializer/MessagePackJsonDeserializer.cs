using System;
using MessagePack;

namespace MackySoft.MasterTools
{
	public sealed class MessagePackJsonDeserializer : IJsonDeserializer
	{
		public object Deserialize (Type type, string json)
		{
			byte[] bytes = MessagePackSerializer.ConvertFromJson(json);
			var obj = MessagePackSerializer.Deserialize(type, bytes);
			return obj;
		}
	}
}
