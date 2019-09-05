using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	partial class Tmdb
	{
		public static TmdbMovie[] DiscoverMovies(TmdbDiscoverMoviesInfo query,int minPage, int maxPage)
		{
			if (null == query)
				query = new TmdbDiscoverMoviesInfo();
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/discover/movie",minPage,maxPage, query.ToArguments())), 
				(d) => new TmdbMovie((IDictionary<string,object>)d));
		}
		public static TmdbShow[] DiscoverShows(TmdbDiscoverShowsInfo query, int minPage, int maxPage)
		{
			if (null == query)
				query = new TmdbDiscoverShowsInfo();
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang("/discover/tv", minPage, maxPage, query.ToArguments())),
				(d) => new TmdbShow((IDictionary<string, object>)d));
		}
	}
}
