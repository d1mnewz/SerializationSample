using System.IO;
using System.Threading.Tasks;
using ProtoBuf;

namespace SerializationSample
{
	internal interface ISerializer
	{
		Task<byte[]> Serialize<T>(T obj);
		Task<T> Deserialize<T>(byte[] arr);
	}

	public class ProtobufSerializer : ISerializer
	{
		public Task<byte[]> Serialize<T>(T obj)
		{
			using (var stream = new MemoryStream())
			{
				Serializer.Serialize(stream, obj);
				return Task.FromResult(stream.ToArray());
			}
		}

		public Task<T> Deserialize<T>(byte[] arr)
		{
			using (var stream = new MemoryStream(arr))
			{
				return Task.FromResult(Serializer.Deserialize<T>(stream));
			}
		}
	}

	public class BinarySerializer : ISerializer
	{
		public Task<byte[]> Serialize<T>(T obj)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				formatter.Serialize(stream, obj);
				return Task.FromResult(stream.ToArray());
			}
		}

		public Task<T> Deserialize<T>(byte[] arr)
		{
			var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using (var stream = new MemoryStream(arr))
			{
				return Task.FromResult((T) formatter.Deserialize(stream));
			}
		}
	}
}