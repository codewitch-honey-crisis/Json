using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbShow : TmdbMedia
	{
		public TmdbShow(IDictionary<string, object> json) : base(json)
		{
			_FixupJson(Json);
			InitializeCache();
		}
		public TmdbShow(int id) : base(id)
		{
			_FixupJson(Json);
			InitializeCache();
		}
		public override TmdbPrimaryType PrimaryType => TmdbPrimaryType.Show;
		public override string[] PathIdentity {
			get {
				return new string[] { "tv", Id.ToString() };
			}
		}
		public KeyValuePair<string,string>[] ContentRatingsByCountry {
			get {
				_EnsureFetchedContentRatings();
				var l = GetField<IList<object>>("content_ratings");
				return Tmdb.ToKvpArray<string, string>(l, "iso_3166_1", "rating");
			}
		}
		void _EnsureFetchedContentRatings()
		{
			var l = GetField<IList<object>>("content_ratings");
			if (null == l)
			{
				var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/alternative_titles"));
				object o;
				if (json.TryGetValue("results", out o))
				{
					l = o as IList<object>;
					if (null != l)
						Json["content_ratings"] = l;
				}
			}
		}
		public TmdbPerson CreatedBy {
			get {
				var d = GetCachedField<IDictionary<string, object>>("created_by");
				if(null!=d)
					return new TmdbPerson(d);
				return null;
			}
		}
		public TimeSpan EpisodeRunTime => new TimeSpan(0,GetCachedField("episode_runtime" ,0),0);
		public DateTime FirstAirDate => Tmdb.DateToDateTime(GetCachedField<string>("first_air_date"));
		public bool InProduction => GetCachedField("in_production", false);
		public DateTime LastAirDate => Tmdb.DateToDateTime(GetCachedField<string>("last_air_date"));
		public TmdbEpisode LastEpisodeToAir {
			get {
				var d = GetCachedField<IDictionary<string, object>>("last_episode_to_air");
				if (null != d)
					return new TmdbEpisode(d);
				return null;
			}
		}
		public TmdbEpisode NextEpisodeToAir {
			get {
				var d = GetCachedField<IDictionary<string, object>>("next_episode_to_air");
				if (null != d)
					return new TmdbEpisode(d);
				return null;
			}
		}
		public TmdbNetwork[] Networks
			=> JsonArray.ToArray(
				GetCachedField<IList<object>>("networks"), 
				(d) => new TmdbNetwork((IDictionary<string, object>)d));
		// no idea why this is singular
		public string[] OriginCountries => JsonArray.ToArray<string>(GetCachedField<IList<object>>("origin_country"));

		static object _FixupJson(object json)
		{
			var result = json;
			var d = json as IDictionary<string, object>;
			if(null!=d)
			{
				// fix up the season data.
				object o;
				var newSeasons = new JsonObject();
				if (d.TryGetValue("seasons", out o))
				{
					var l = o as IList<object>;
					if (null != l)
					{
						for (int ic = l.Count, i = 0; i < ic; ++i)
						{
							var season = l[i] as IDictionary<string, object>;
							if (null != season)
							{
								if (season.TryGetValue("season_number", out o) && o is int)
								{
									var sn = (int)o;
									newSeasons.Add(sn.ToString(), season);
								}
							}
						}
						d["seasons"] = newSeasons;
					}
				}
			}
			return result;
		}
		public TmdbSeason[] Seasons {
			get {
				var seasons = GetCachedField<IDictionary<string,object>>("seasons");
				if (null != seasons)
				{
					var result = new List<TmdbSeason>(seasons.Count);
					var i = 0;
					foreach (var kvp in seasons)
					{
						var season = kvp.Value as IDictionary<string, object>;
						if (null != season)
						{
							if (-1 < Id)
								season["show_id"] = Id;
							result[i] = new TmdbSeason(season);
						}
						++i;
					}
					// TODO: Should probably put this in the show seasons fixup for perf reasons
					result.Sort((lhs, rhs) => { return lhs.Number.CompareTo(rhs.Number); });
					return result.ToArray();
				}
				return null;
			}
		}
		public string Status => GetField<string>("status");
		public string ShowType => GetField<string>("type");
		protected override void Fetch()
		{
			FetchJsonLang(null,_FixupJson);
		}

		public TmdbShow[] GetSimilar(int minPage=0,int maxPage=999)
		{
			return JsonArray.ToArray(Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/similar"),minPage,maxPage)),
				(d) => new TmdbShow((IDictionary<string, object>)d));
		}
		
		public static TmdbShow GetLatest()
		{
			var json = Tmdb.Invoke("/tv/latest");
			if (null != json)
				return new TmdbShow(json);
			return null;
		}
		public static TmdbShow[] GetTrending(TmdbTimeWindow window, int minPage = 0, int maxPage = 999)
		{
			string tw = "day";
			switch (window)
			{
				case TmdbTimeWindow.Week:
					tw = "week";
					break;
			}
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePaged(string.Concat("/trending/tv/", tw), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbShow((IDictionary<string, object>)d));
		}
		public static TmdbShow[] GetOnTheAir(int minPage = 0, int maxPage = 999)
		{
			return _GetShows("on_the_air", minPage, maxPage);
		}
		public static TmdbShow[] GetAiringToday(int minPage = 0, int maxPage = 999)
		{
			// TODO: supposedly you can specify a timezone but the api doesn't include
			// it in the list of parameters. It apparently, for some reason defaults 
			// to US eastern standard time (UTC -5:00)
			return _GetShows("airing_today", minPage, maxPage);
		}
		public static TmdbShow[] GetPopular(int minPage , int maxPage )
		{
			return _GetShows("popular", minPage, maxPage);
		}
		public static TmdbShow[] GetTopRated(int minPage, int maxPage)
		{
			return _GetShows("top_rated", minPage, maxPage);
		}
		static TmdbShow[] _GetShows(string method, int minPage, int maxPage)
		{
			var args = new JsonObject();
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/tv/", method), minPage, maxPage, args));
			return JsonArray.ToArray(l, (d) => new TmdbShow((IDictionary<string, object>)d));
		}
	}
}
