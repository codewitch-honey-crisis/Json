using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbMovieStatus
	{
		Unknown,
		Rumored,
		Planned,
		InProduction,
		PostProduction,
		Released,
		Canceled
	}
	public class TmdbMovie : TmdbMedia
	{
		public TmdbMovie(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public TmdbMovie(int id) : base(id)
		{
			InitializeCache();
		}
		public override TmdbPrimaryType PrimaryType => TmdbPrimaryType.Movie;
		public override string[] PathIdentity {
			get {
				return new string[] { "movie", Id.ToString() };
			}
		}
		public override TmdbLanguage[] Languages {
			get {
				EnsureFetchedLanguages("spoken_languages");
				var l = GetField<IList<object>>("languages");
				return JsonArray.ToArray(l, (d) => new TmdbLanguage((IDictionary<string, object>)d));
			}
		}
		public override string Name => GetCachedField<string>("title");
		public override string OriginalName => GetCachedField<string>("original_title");
		public string Tagline=> GetCachedField<string>("tagline");
		public int Budget => GetCachedField("budget", 0);
		public bool HasVideo => GetCachedField("video", false);
		public TmdbCollection Collection {
			get {
				var d = GetCachedField<IDictionary<string, object>>("belongs_to_collection");
				if (null != d)
					return new TmdbCollection(d);
				return null;
			}
		}
		public override string ImdbId {
			get {
				var s = GetField<string>("imdb_id");
				if (null != s)
					return s;
				return base.ImdbId;
			}
		}
		public KeyValuePair<string,string>[] ProductionCountriesByIso31661 {
			get {
				var countries = GetField<IList<object>>("production_countries");
				return Tmdb.ToKvpArray<string, string>(countries, "iso_3166_1", "name");
			}
		}
		public DateTime ReleaseDate
			=>Tmdb.DateToDateTime(GetCachedField<string>("release_date"));
		public int Revenue => GetCachedField("revenue", 0);
		public TmdbMovieStatus Status {
			get {
				switch (GetCachedField<string>("status"))
				{
					case "Rumored":
						return TmdbMovieStatus.Rumored;
					case "Planned":
						return TmdbMovieStatus.Planned;
					case "In Production":
						return TmdbMovieStatus.InProduction;
					case "Post Production":
						return TmdbMovieStatus.PostProduction;
					case "Released":
						return TmdbMovieStatus.Released;
					case "Canceled":
						return TmdbMovieStatus.Canceled;
					default:
						return TmdbMovieStatus.Unknown;
				}
			}
		}
		
		
		public TmdbMovieList[] GetLists(int minPage=0,int maxPage=999)
		{
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/lists"), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbMovieList((IDictionary<string, object>)d));
		}
		public static TmdbMovie[] GetNowPlaying(string region=null,int minPage = 0, int maxPage = 999)
		{
			return _GetMovies("now_playing", region, minPage, maxPage);
		}
		public static TmdbMovie[] GetPopular(string region = null, int minPage = 0, int maxPage = 999)
		{
			return _GetMovies("popular",region, minPage, maxPage);
		}
		public static TmdbMovie[] GetTopRated(string region = null, int minPage = 0, int maxPage = 999)
		{
			return _GetMovies("top_rated", region, minPage, maxPage);
		}
		public static TmdbMovie[] GetUpcoming(string region = null, int minPage = 0, int maxPage = 999)
		{
			return _GetMovies("upcoming", region, minPage, maxPage);
		}
		public static TmdbMovie[] GetTrending(TmdbTimeWindow window,int minPage=0,int maxPage=999)
		{
			string tw="day";
			switch(window)
			{
				case TmdbTimeWindow.Week:
					tw = "week";
					break;
			}
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePaged(string.Concat("/trending/movie/", tw), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbMovie((IDictionary<string, object>)d));
		}
		public static TmdbMovie[] _GetMovies(string method,string region, int minPage, int maxPage)
		{
			var args = new JsonObject();
			if (null != region)
				args.Add("region", region);
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/movie/",method), minPage, maxPage, args));
			return JsonArray.ToArray(l, (d) => new TmdbMovie((IDictionary<string, object>)d));
		}
		
		public new TmdbMovie[] GetSimilar(int minPage = 0, int maxPage = 999)
		{
			return (TmdbMovie[])base.GetSimilar(minPage, maxPage);
		}
		
		protected override TmdbMedia[] GetSimilarImpl(int minPage, int maxPage)
		{
			return JsonArray.ToArray(Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/similar"))),
					(d) => new TmdbMovie((IDictionary<string, object>)d));
		}
		public static TmdbMovie GetLatest()
		{
			var json = Tmdb.Invoke("/movie/latest");
			if (null != json)
				return new TmdbMovie(json);
			return null;
		}
	}
}
