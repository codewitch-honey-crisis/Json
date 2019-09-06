using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbGuestSession :TmdbEntity
	{
		public TmdbGuestSession(string id) : base(_CreateJson(id))
		{
		}
		public TmdbGuestSession(IDictionary<string,object> json) : base(json)
		{
		}
		public string Id => GetField<string>("id");

		static IDictionary<string, object> _CreateJson(string id)
		{
			var result = new JsonObject();
			result.Add("id", id);
			return result;
		}

		public TmdbShow[] RatedShows {
			get {
				_EnsureFetchedRatedShows();
				var l = GetField<IList<object>>("rated_shows");
				if(null!=l)
					return JsonArray.ToArray(l, (d) => new TmdbShow((IDictionary<string, object>)d));
				return null;
			}
		}
		
		public TmdbShow[] GetRatedShows(TmdbRatedSortType sortBy = TmdbRatedSortType.Default)
		{
			var sessionId = Id;
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					// we want to cache the default
					return RatedShows;
			}
			var args = new JsonObject();
			args.Add("sort_by", sb);
			var l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/tv"), 0, 0,args));
			if (null != l)
				return JsonArray.ToArray(l, (d) => new TmdbShow((IDictionary<string, object>)d));
			return null;
		}
		void _EnsureFetchedRatedShows()
		{
			var l = GetField<IList<object>>("rated_shows");
			if (null == l)
			{
				// this function does not actually accept a page parameter, but it returns the result
				// in paged form, so we use the paged routines with the default page (0) which does not
				// send a page parameter.
				l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/tv"), 0, 0));
				if (null != l)
					Json.Add("rated_shows", l);
			}
		}
		public TmdbMovie[] RatedMovies {
			get {
				_EnsureFetchedRatedMovies();
				var l = GetField<IList<object>>("rated_movies");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbMovie((IDictionary<string, object>)d));
				return null;
			}
		}
		public TmdbMovie[] GetRatedMovies(TmdbRatedSortType sortBy = TmdbRatedSortType.Default)
		{
			var sessionId = Id;
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					// we want to cache the default
					return RatedMovies;
			}
			var args = new JsonObject();
			args.Add("sort_by", sb);
			var l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/movies"), 0, 0, args));
			if (null != l)
				return JsonArray.ToArray(l, (d) => new TmdbMovie((IDictionary<string, object>)d));
			return null;
		}
		void _EnsureFetchedRatedMovies()
		{
			var l = GetField<IList<object>>("rated_movies");
			if (null == l)
			{
				// this function does not actually accept a page parameter, but it returns the result
				// in paged form, so we use the paged routines with the default page (0) which does not
				// send a page parameter.
				l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/movies"), 0, 0));
				if (null != l)
					Json.Add("rated_movies", l);
			}
		}
		public TmdbEpisode[] RatedEpisodes {
			get {
				_EnsureFetchedRatedEpisodes();
				var l = GetField<IList<object>>("rated_episodes");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbEpisode((IDictionary<string, object>)d));
				return null;
			}
		}
		public TmdbEpisode[] GetRatedEpisodes(TmdbRatedSortType sortBy = TmdbRatedSortType.Default)
		{
			var sessionId = Id;
			string sb = null;
			switch (sortBy)
			{
				case TmdbRatedSortType.DateCreated | TmdbRatedSortType.Ascending:
					sb = "created_at.asc";
					break;
				default:
					// we want to cache the default
					return RatedEpisodes;
			}
			var args = new JsonObject();
			args.Add("sort_by", sb);
			var l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/tv/episodes"), 0, 0, args));
			if (null != l)
				return JsonArray.ToArray(l, (d) => new TmdbEpisode((IDictionary<string, object>)d));
			return null;
		}
		void _EnsureFetchedRatedEpisodes()
		{
			var l = GetField<IList<object>>("rated_episodes");
			if (null == l)
			{
				// this function does not actually accept a page parameter, but it returns the result
				// in paged form, so we use the paged routines with the default page (0) which does not
				// send a page parameter.
				l = Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/guest_session/", Id, "/rated/tv/episodes"), 0, 0));
				if (null != l)
					Json.Add("rated_episodes", l);
			}
		}
	}
}
