using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SerializationSample
{
	class Program
	{
		private static async Task Main()
		{
			await MakeTest(new BinarySerializer(), GetRandomBooks, 5);
			await MakeTest(new ProtobufSerializer(), GetRandomBooks, 5);

			Console.ReadKey();
		}

		private static async Task MakeTest(ISerializer serializer, Func<int, List<Book>> source, int countTimes)
		{
			for (var i = 1; i <= countTimes; i++)
			{
				Console.WriteLine(serializer.GetType().Name);
				var books = source.Invoke((int) Math.Pow(10, i));
				Console.WriteLine($"{books.Count} books");
				var sw = Stopwatch.StartNew();
				var serialized = await serializer.Serialize(books);
				Console.WriteLine($"Serialize: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks {serialized.Length} byte");
				var deserealized = await serializer.Deserialize<List<Book>>(serialized);
				sw.Stop();
				Console.WriteLine($"DeSerialize: {sw.ElapsedMilliseconds} ms, {sw.ElapsedTicks} ticks");
				Console.WriteLine();
			}
		}

		private static List<Book> GetRandomBooks(int count)
		{
			var list = new List<Book>();
			for (var i = 0; i < count; i++)
			{
				var book = new Book(Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString(),
					new DateRange(DateTime.MinValue, DateTime.MaxValue),
					new Image(Guid.NewGuid().ToString(),
						Guid.NewGuid().ToString()),
					Money.Usd(100), Guid.NewGuid().ToString(),
					new BookDifficulty(i * 3, i * 2, i * 4), i * 2018);
				book.AddCategories(new BookCategory(Guid.NewGuid().ToString()),
					new BookCategory(Guid.NewGuid().ToString()),
					new BookCategory(Guid.NewGuid().ToString()));
				list.Add(book);
			}

			return list;
		}
	}
}