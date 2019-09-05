using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbChangeAction :TmdbEntity
	{
		public TmdbChangeAction(IDictionary<string,object> json) : base(json) { }
		public string Id => GetField<string>("id");
		public string Action => GetField<string>("action");
		public DateTime Timestamp {
			get {
				var s = GetField<string>("time");
				if (!string.IsNullOrEmpty(s))
				{
					DateTime result;
					// TODO: see if this works
					if (DateTime.TryParse(s, out result))
						return result;
				}
					
				return default(DateTime);
			}
		}
		public string Language {
			get {
				return GetField<string>("iso_639_1");
			}
		}
		public string Value {
			get {
				return GetField<string>("value");
			}
		}
		public string OriginalValue {
			get {
				return GetField<string>("original_value");
			}
		}
	}
}
