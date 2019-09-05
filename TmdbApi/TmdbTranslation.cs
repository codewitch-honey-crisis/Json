using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbTranslation : TmdbEntity
	{
		public TmdbTranslation(IDictionary<string,object> json) : base(json)
		{
		}
		
		public string Country {
			get {
				return GetField<string>("iso_3166_1");
			}
		}
		public string Language {
			get {
				return GetField<string>("iso_639_1");
			}
		}
		public string Name {
			get {
				return GetField<string>("name");
			}
		}
		public string EnglishName {
			get {
				return GetField<string>("english_name");
			}
		}
		public DataEntry Data {
			get {
				var d = GetField<IDictionary<string, object>>("data");
				if(null!=d)
					return new DataEntry(d);
				return null;
			}
		}
		public class DataEntry : TmdbEntity
		{
			public DataEntry(IDictionary<string,object> json) : base(json)
			{
			}
			
			public string Name {
				get {
					return GetField<string>("title");
				}
			}
			public string Overview {
				get {
					return GetField<string>("overview");
				}
			}
			public string Homepage {
				get {
					return GetField<string>("homepage");
				}
			}
		}
	}
}
