using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace SerializationSample
{
	[ProtoContract]
	[Serializable]
	public class Book : Identifieable
	{
		public Book()
		{
		}

		public string Name { get; protected set; }
		public string Author { get; protected set; }
		public string Language { get; protected set; }
		public DateRange Valid { get; protected set; } = DateRange.Empty;
		public Image Image { get; protected set; } = Image.None;
		public Money Price { get; protected set; }
		public string Isbn { get; protected set; }
		public BookDifficulty Difficulty { get; protected set; } = BookDifficulty.Empty;
		public long Length { get; protected set; }


		public void AddCategories(params BookCategory[] categories)
		{
			foreach (var category in categories)
			{
				_categories.Add(new BookCategoryBook(category, this));
			}
		}

		public Book(string name, string author, string language, DateRange valid, Image image, Money price, string isbn, BookDifficulty difficulty, long length)
		{
			Id = Guid.NewGuid();
			Name = name;
			Author = author;
			Language = language;
			Valid = valid;
			Image = image;
			Price = price;
			Isbn = isbn;
			Difficulty = difficulty;
			Length = length;
		}

		public List<Guid> Categories => _categories.Select(x => x.BookCategoryId).ToList();
		private ICollection<BookCategoryBook> _categories { get; set; } = new HashSet<BookCategoryBook>();
	}
}