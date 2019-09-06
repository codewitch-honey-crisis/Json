using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public abstract class TmdbEntity : IEquatable<TmdbEntity>
	{
		protected TmdbEntity(IDictionary<string, object> json)
		{
			Json = json ?? throw new ArgumentNullException(nameof(json));
		}
		
		public IDictionary<string, object> Json { get; protected set; }

		
		protected T GetField<T>(string name,T @default=default(T))
		{
			object o;
			if (Json.TryGetValue(name, out o) && o is T)
				return (T)o;
			return @default;
		}
		// objects are considered equal if they
		// point to the same actual json reference
		public bool Equals(TmdbEntity rhs)
		{
			if (ReferenceEquals(this, rhs))
				return true;
			if (ReferenceEquals(rhs, null))
				return false;
			return ReferenceEquals(Json, rhs.Json);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as TmdbEntity);
		}
		public static bool operator==(TmdbEntity lhs, TmdbEntity rhs)
		{
			if (object.ReferenceEquals(lhs, rhs)) return true;
			if (object.ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator!=(TmdbEntity lhs, TmdbEntity rhs)
		{
			if (object.ReferenceEquals(lhs, rhs)) return false;
			if (object.ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		public override int GetHashCode()
		{
			// we need the actual base hashcode, not the one
			// computed by the JsonObject.
			var jo = Json as JsonObject; // should always be but it doesn't *have* to be
			if(null!=jo)
			{
				return jo.BaseDictionary.GetHashCode();
			}
			return Json.GetHashCode();
		}
	}
}
