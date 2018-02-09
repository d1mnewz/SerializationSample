using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SerializationSample
{
	public enum BookDifficultyType
	{
		Lix = 10,
		Let = 20,
		Atos = 30,
	}

	[Serializable]
	public class BookCategory : Identifieable
	{
		private ICollection<BookCategoryBook> _books { get; set; } = new HashSet<BookCategoryBook>();


		public BookCategory(string name)
		{
			Id = Guid.NewGuid();
			Name = name;
		}

		public string Name { get; private set; }


		public List<Guid> Books => _books.Select(x => x.BookId).ToList();
	}

	[Serializable]
	public class BookCategoryBook : Identifieable
	{
		public BookCategoryBook(BookCategory bookCategory, Book book)
		{
			BookCategoryId = bookCategory.Id;
			BookId = book.Id;
		}

		public Guid BookCategoryId { get; private set; }
		public Guid BookId { get; private set; }
	}

	[Serializable]
	public abstract class Identifieable
	{
		public Guid Id;
	}

	[Serializable]
	public class BookDifficulty : ValueObject<BookDifficulty>
	{
		private BookDifficulty()
		{
		}

		public BookDifficulty(int? lix, int? let, int? atos)
		{
			Lix = lix;
			Let = let;
			Atos = atos;
		}

		public int? Lix { get; private set; }
		public int? Let { get; private set; }
		public int? Atos { get; private set; }

		public static BookDifficulty Empty { get; } = new BookDifficulty();
	}

	[Serializable]
	public class Money : ValueObject<Money>
	{
		private Money()
		{
		}

		private Money(decimal value, string currency)
		{
			Value = value;
			Currency = currency.ToUpper();
		}

		public decimal Value { get; private set; }
		public string Currency { get; private set; }

		public static Money Dkk(decimal value) => new Money(value, "DKK");
		public static Money Usd(decimal value) => new Money(value, "USD");
	}

	public interface IRange<T> where T : struct
	{
		T? From { get; }
		T? To { get; }
	}

	public static class RangeExtensions
	{
		public static bool IsDefined<T>(this IRange<T> range) where T : struct => range != null && (range.From != null || range.To != null);
	}

	[Serializable]
	public class DateRange : ValueObject<DateRange>, IRange<DateTime>
	{
		private DateRange()
		{
		}

		private DateRange(DateTime? from = null, DateTime? to = null)
		{
			From = from;
			To = to;
		}

		public DateRange(DateTime from, DateTime to) : this(from, (DateTime?) to)
		{
		}

		public DateTime? From { get; private set; }
		public DateTime? To { get; private set; }

		public static DateRange Lower(DateTime from) => new DateRange(from: from);
		public static DateRange Upper(DateTime to) => new DateRange(to: to);
		public static DateRange Empty { get; } = new DateRange();
	}

	[Serializable]
	public class Image : ValueObject<Image>
	{
		private Image()
		{
		}

		public Image(string original, string thumbnail)
		{
			Original = original;
			Thumbnail = thumbnail;
		}

		public string Original { get; private set; }
		public string Thumbnail { get; private set; }

		public static Image None { get; } = new Image();
	}

	public interface IValueObject<T>
	{
	}

	[Serializable]
	public abstract class ValueObject<T> : IValueObject<T>, IEquatable<T> where T : class, IValueObject<T>
	{
		private int? _cachedHash;

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var other = obj as T;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			if (_cachedHash.HasValue) return _cachedHash.Value;

			const int startValue = 17;
			const int multiplier = 59;
			var hashCode = startValue;

			foreach (var field in GetFields(GetType()))
			{
				var value = field.GetValue(this);

				if (value != null)
					hashCode = hashCode * multiplier + value.GetHashCode();
			}

			_cachedHash = hashCode;

			return _cachedHash.Value;
		}

		public virtual bool Equals(T other)
		{
			if (other == null)
				return false;

			var t = GetType();
			var otherType = other.GetType();

			if (t != otherType)
				return false;

			foreach (var field in GetFields(t))
			{
				var value1 = field.GetValue(other);
				var value2 = field.GetValue(this);

				if (value1 == null)
				{
					if (value2 != null)
						return false;
				}

				else if (!value1.Equals(value2))
					return false;
			}

			return true;
		}


		private static IEnumerable<FieldInfo> GetFields(Type t)
		{
			var fields = new List<FieldInfo>();

			while (t != typeof(object))
			{
				fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
				t = t.BaseType;
			}

			return fields;
		}

		public static bool operator ==(ValueObject<T> x, ValueObject<T> y) => x.Equals(y);
		public static bool operator !=(ValueObject<T> x, ValueObject<T> y) => !(x == y);
	}
}