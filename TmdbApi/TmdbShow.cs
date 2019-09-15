using Json;
using System;
using System.Collections.Generic;
using System.Text;
using Bee;
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
		public TmdbPerson[] CreatedBy {
			get {
				var l = GetCachedField<IList<object>>("created_by");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbPerson((IDictionary<string, object>)d));
				return null;
			}
		}
		public TimeSpan EpisodeRunTime {
			get {
				var arr = EpisodeRunTimes;
				if (null != arr && 0 < arr.Length)
					return arr[0];
				return new TimeSpan(0);
			}
		}
		// because specials is season 0 sometimes, this is a helper
		// to grab the actual first season
		public TmdbSeason FirstSeason {
			get {
				var d = GetCachedField<IDictionary<string, object>>("seasons");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("1", out o))
					{
						var dd = o as IDictionary<string, object>;
						if(null!=dd)
						{
							return new TmdbSeason(dd);
						}
					}
				}
				return null;
			}
		}
		// optimized way to get season by it's official number
		public TmdbSeason GetSeasonByNumber(int seasonNumber) {
			
			var d = GetCachedField<IDictionary<string, object>>("season");
			if (null != d)
			{
				object o;
				if (d.TryGetValue(seasonNumber.ToString(), out o))
				{
					var dd = o as IDictionary<string, object>;
					if (null != dd)
					{
						return new TmdbSeason(dd);
					}
				}
			}
			return null;
			
		}
		public TimeSpan[] EpisodeRunTimes
			=> JsonArray.ToArray(GetCachedField<IList<object>>("episode_run_time"), (i) => new TimeSpan(0, (int)i, 0));
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

		object _FixupJson(object json)
		{
			var result = json;
			var d = json as IDictionary<string, object>;
			if(null!=d)
			{
				// fix up the season data.
				object o;
				var newSeasons = new JsonObject().Synchronize();
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
									season["show_id"] = Id;
									newSeasons.Add(sn.ToString(), season);
								}
							}
						}
						d["season"] = newSeasons;
						d.Remove("seasons");
					}
				}
			}
			return result;
		}
		public TmdbSeason[] Seasons {
			get {
				var seasons = GetCachedField<IDictionary<string,object>>("season");
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
							result.Add(new TmdbSeason(season));
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
