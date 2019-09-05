using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public class TmdbLanguage : TmdbEntity
	{
		public TmdbLanguage(IDictionary<string,object> json) : base(json)
		{
		}
		public string Name => GetField<string>("name");
		public string EnglishName => GetField<string>("english_name");
		public string Iso => GetField<string>("iso_639_1");

	}
}
