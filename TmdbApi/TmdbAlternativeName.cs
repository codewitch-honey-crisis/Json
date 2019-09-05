using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbAlternativeName : TmdbEntity
	{
		public TmdbAlternativeName(IDictionary<string, object> json) : base(json)
		{
		}
		
		public string Name {
			get {
				var result = GetField<string>("name");
				if (null == result)
					result = GetField<string>("title");
				return result;
			}
		}
		// TODO: find out what this actually means and make an enum for it.
		// it's not documented at all.
		public string Type => GetField<string>("type");
		// only for movies 
		public string Country {
			get {
				return GetField<string>("iso_3166_1");
			}
		}
	}
}
