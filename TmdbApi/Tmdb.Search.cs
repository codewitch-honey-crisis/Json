using Json;
using System;
using System.Collections.Generic;

namespace TmdbApi
{
	partial class Tmdb
	{
		public static TmdbCompany[] SearchCompanies(string query,int minPage,int maxPage)
		{
			var args = new JsonObject();
			args.Add("query", query);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePaged("/search/company", minPage, maxPage, args)),
				(d) => new TmdbCompany((IDictionary<string, object>)d));
		}
		public static TmdbCollection[] SearchCollections(string query, int minPage, int maxPage)
		{
			var args = new JsonObject();
			args.Add("query", query);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/search/collection", minPage, maxPage, args)),
				(d) => new TmdbCollection((IDictionary<string, object>)d));
		}
		public static TmdbKeyword[] SearchKeywords(string query, int minPage, int maxPage)
		{
			var args = new JsonObject();
			args.Add("query", query);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/search/keyword", minPage, maxPage, args)),
				(d) => new TmdbKeyword((IDictionary<string, object>)d));
		}
		public static TmdbMovie[] SearchMovies(string query, int minPage, int maxPage,int year=0,int primaryReleaseYear=0,string region=null,bool includeAdult=false)
		{
			var args = new JsonObject();
			args.Add("query", query);
			if (!string.IsNullOrEmpty(region))
				args.Add("region", region);
			if (includeAdult)
				args.Add("include_adult", includeAdult);
			if (0 < year)
				args.Add("year", year);
			if (0 < primaryReleaseYear)
				args.Add("primary_release_year",primaryReleaseYear);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/search/movie", minPage, maxPage, args)),
				(d) => new TmdbMovie((IDictionary<string, object>)d));
		}
		public static TmdbPrimary[] Search(string query,int minPage,int maxPage,string region=null,bool includeAdult=false)
		{
			var args = new JsonObject();
			args.Add("query", query);
			if (!string.IsNullOrEmpty(region))
				args.Add("region", region);
			if (includeAdult)
				args.Add("includeAdult", includeAdult);
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang("/search/multi", minPage, maxPage,args));
			return JsonArray.ToArray<TmdbPrimary>(l, (dd) =>
			{
				var d = dd as IDictionary<string, object>;
				if (null != d)
				{
					object o;
					if (d.TryGetValue("media_type", out o))
					{
						switch(o as string)
						{
							case "movie":
								return new TmdbMovie((IDictionary<string, object>)dd);
							case "tv":
								return new TmdbShow((IDictionary<string, object>)dd);
							case "person":
								return new TmdbPerson((IDictionary<string, object>)dd);
						}
					}
				}
				return null;
			}
			);

		}
		public static TmdbPerson[] SearchPeople(string query, int minPage, int maxPage,string region=null,bool includeAdult=false)
		{
			var args = new JsonObject();
			args.Add("query", query);
			if (!string.IsNullOrEmpty(region))
				args.Add("region", region);
			if (includeAdult)
				args.Add("include_adult", includeAdult);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/search/person", minPage, maxPage, args)),
				(d) => new TmdbPerson((IDictionary<string, object>)d));
		}
		public static TmdbShow[] SearchShows(string query, int minPage, int maxPage,int firstAirDateYear=0)
		{
			var args = new JsonObject();
			args.Add("query", query);
			if (0 < firstAirDateYear)
				args.Add("first_air_date_year", firstAirDateYear);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/search/tv", minPage, maxPage, args)),
				(d) => new TmdbShow((IDictionary<string, object>)d));
		}
	}
}
