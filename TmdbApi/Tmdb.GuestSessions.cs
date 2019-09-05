using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{

	partial class Tmdb
	{
		public static TmdbMovie[] GetGuestSessionRatedMovies(TmdbRatedSortType sortBy=TmdbRatedSortType.Default)
		{ 
			var sessionId = GuestSessionId;
			var args = new JsonObject();
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					sb = "created_at.desc";
					break;
			}
			args.Add("sort_by", sb);
			// this function does not actually accept a page parameter, but it returns the result
			// in paged form, so we use the paged routines with the default page (0) which does not
			// send a page parameter.
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", sessionId, "/rated/movies"), 0, 0, args)),
				(d) => new TmdbMovie((IDictionary<string,object>)d));
		}
		public static TmdbShow[] GetGuestSessionRatedShows(TmdbRatedSortType sortBy = TmdbRatedSortType.Default)
		{
			var sessionId = GuestSessionId;
			var args = new JsonObject();
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					sb = "created_at.desc";
					break;
			}
			args.Add("sort_by", sb);
			// this function does not actually accept a page parameter, but it returns the result
			// in paged form, so we use the paged routines with the default page (0) which does not
			// send a page parameter.
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", sessionId, "/rated/tv"), 0, 0, args)),
				(d) => new TmdbShow((IDictionary<string, object>)d));
		}
		public static TmdbEpisode[] GetGuestSessionRatedEpisodes(TmdbRatedSortType sortBy = TmdbRatedSortType.Default)
		{
			var sessionId = GuestSessionId;
			var args = new JsonObject();
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					sb = "created_at.desc";
					break;
			}
			args.Add("sort_by", sb);
			// this function does not actually accept a page parameter, but it returns the result
			// in paged form, so we use the paged routines with the default page (0) which does not
			// send a page parameter.
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", sessionId, "/rated/tv/episodes"), 0, 0, args)),
				(d) => new TmdbEpisode((IDictionary<string, object>)d));
		}
	}
}
