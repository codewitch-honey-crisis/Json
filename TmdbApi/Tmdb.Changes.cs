using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	partial class Tmdb
	{
		public static TmdbMovie[] GetMovieChanges(DateTime startDate = default(DateTime), DateTime endDate = default(DateTime), int minPage = 0, int maxPage = 999)
		{
			var args = new JsonObject();
			if (default(DateTime) != startDate)
				args.Add("start_date", startDate.ToString("yyyy-MM-dd"));
			if (default(DateTime) != endDate)
				args.Add("end_date", endDate.ToString("yyyy-MM-dd"));
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePaged("/movie/changes", minPage, maxPage,args))
					, (d) => new TmdbMovie(d as IDictionary<string,object>));
		}
		public static TmdbShow[] GetShowChanges(DateTime startDate = default(DateTime), DateTime endDate = default(DateTime), int minPage = 0, int maxPage = 999)
		{
			var args = new JsonObject();
			if (default(DateTime) != startDate)
				args.Add("start_date", startDate.ToString("yyyy-MM-dd"));
			if (default(DateTime) != endDate)
				args.Add("end_date", endDate.ToString("yyyy-MM-dd"));
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePaged("/tv/changes", minPage, maxPage, args))
					, (d) => new TmdbShow(d as IDictionary<string, object>));
		}
		public static TmdbPerson[] GetPersonChanges(DateTime startDate = default(DateTime), DateTime endDate = default(DateTime), int minPage = 0, int maxPage = 999)
		{
			var args = new JsonObject();
			if (default(DateTime) != startDate)
				args.Add("start_date", startDate.ToString("yyyy-MM-dd"));
			if (default(DateTime) != endDate)
				args.Add("end_date", endDate.ToString("yyyy-MM-dd"));
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePaged("/person/changes", minPage, maxPage, args))
					, (d) => new TmdbPerson(d as IDictionary<string, object>));
		}
	}
}
