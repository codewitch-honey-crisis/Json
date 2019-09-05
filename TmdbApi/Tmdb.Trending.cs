using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	partial class Tmdb
	{
		public static TmdbPrimary[] GetTrending(TmdbTimeWindow window, int minPage = 0, int maxPage = 999)
		{
			string tw = "day";
			switch (window)
			{
				case TmdbTimeWindow.Week:
					tw = "week";
					break;
			}
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePaged(string.Concat("/trending/all/", tw), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => _GetPrimaryTrending((IDictionary<string, object>)d));
		}
		static TmdbPrimary _GetPrimaryTrending(IDictionary<string,object> json)
		{
			// HACK: this API call does not return media_type
			// therefore we have to try to discern it from the 
			// present fields. Discerning movie and tv is easy.
			// discerning between people and tv requires an
			// extra step
			if (Json.ContainsKey("title"))
				return new TmdbMovie(json);
			if(!Json.ContainsKey("first_air_date") && !Json.ContainsKey("overview") && !Json.ContainsKey("poster_path"))
				return new TmdbPerson(json);
			return new TmdbShow(json);
		}
	}
}
