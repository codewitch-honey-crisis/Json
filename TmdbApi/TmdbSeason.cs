using Json;
using System;
using System.Collections.Generic;
using System.Text;
using Bee;
namespace TmdbApi
{
	public sealed class TmdbSeason : TmdbCachedEntity
	{
		public TmdbSeason(int showId,int seasonNumber) : base(_CreateJson(showId,seasonNumber))
		{
			InitializeCache();
			_FixupJson(Json);
			
		}
		public TmdbSeason(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
			_FixupJson(Json);
			
		}
		// /tv/{show_id}/seasons/{season_number}
		public override string[] PathIdentity 
			=> new string[] {
				"tv",
				GetField("show_id", -1).ToString(),
				"season",
				GetField("season_number", -1).ToString()
			};
		public TmdbShow Show {
			get {
				int showId = GetField("show_id",-1);
				if(-1<showId)
					return new TmdbShow(showId);
				return null;
			}
		}
		public int Number {
			get {
				return GetField("season_number", -1);
			}
		}
		public string Name => GetCachedField<string>("name");
		public string PosterPath => GetCachedField<string>("poster_path");
		public int Season => GetCachedField<int>("number_of_episodes");
		static IDictionary<string,object> _CreateJson(int showId,int seasonNumber)
		{
			var result = new JsonObject().Synchronize();
			result.Add("show_id", showId);
			result.Add("season_number", seasonNumber);
			return result;
		}
		protected override void Fetch()
		{
			FetchJsonLang(null,_FixupJson);
		}
		object _FixupJson(object json)
		{
			var d = json as IDictionary<string, object>;
			object o;
			var newEpisodes = new JsonObject().Synchronize();
			if (d.TryGetValue("episodes", out o))
			{
				var l = o as IList<object>;
				if (null != l)
				{
					for (int ic = l.Count, i = 0; i < ic; ++i)
					{
						var episode = l[i] as IDictionary<string, object>;
						if (null != episode)
						{
							if (episode.TryGetValue("episode_number", out o) && o is int)
							{
								var en = (int)o;
								newEpisodes.Add(en.ToString(), episode);
							}
						}
					}
					d["episode"] = newEpisodes;
					d.Remove("episodes");
				}
			}
			return json;
		}
		public TmdbCastMember[] Cast {
			get {
				_EnsureFetchedCredits();
				var credits = GetField("credits", (IDictionary<string, object>)null);
				if (null != credits)
				{
					object o;
					if (credits.TryGetValue("cast", out o))
					{
						var l = o as IList<object>;
						return JsonArray.ToArray(l, (d) => new TmdbCastMember((IDictionary<string, object>)d));
					}
				}
				return null;
			}
		}
		public TmdbCrewMember[] Crew {
			get {
				_EnsureFetchedCredits();
				var credits = GetField("credits", (IDictionary<string, object>)null);
				if (null != credits)
				{
					object o;
					if (credits.TryGetValue("crew", out o))
					{
						var l = o as IList<object>;
						return JsonArray.ToArray(l, (d) => new TmdbCrewMember((IDictionary<string, object>)d));
					}
				}
				return null;
			}
		}
		public TmdbEpisode[] Episodes {
			get {
				var sid = (null != Show) ? Show.Id : -1;
				var episodes = GetCachedField<IDictionary<string,object>>("episode");
				if (null != episodes)
				{
					var result = new List<TmdbEpisode>(episodes.Count);
					var i = 0;
					foreach (var kvp in episodes)
					{
						var episode = kvp.Value as IDictionary<string, object>;
						if (null != episode)
						{
							if (-1 < sid)
								episode["show_id"] = sid;
							episode["season_number"] = Number;
							result.Add(new TmdbEpisode(episode));
						}
						++i;
					}
					result.Sort((lhs, rhs) => { return lhs.Number.CompareTo(rhs.Number); });
					return result.ToArray();
				}
				return null;
			}
		}
		public TmdbImage[] Images {
			get {
				_EnsureFetchedImages();
				var l = GetField<IList<object>>("images");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbImage(d as IDictionary<string, object>));
				return null;
			}
		}
		public string ImdbId {
			get {
				_EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("imdb_id", out o))
						return o as string;
				}
				return null;
			}
		}
		public string TvdbId {
			get {
				_EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("tvdb_id", out o))
						return o as string;
				}
				return null;
			}
		}
		public TmdbVideo[] Videos {
			get {
				_EnsureFetchedVideos();
				var l = GetField<IList<object>>("videos");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbVideo(d as IDictionary<string, object>));
				return null;
			}
		}
		public TmdbMediaAccountInfo GetAccountInfo(TmdbSession session = null)
		{
			var args = new JsonObject();
			if (null == session)
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			else
				args.Add("session_id", session.Id);
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/account_states"));
			return new TmdbMediaAccountInfo(json);
		}
		public KeyValuePair<string, TmdbChangeAction[]>[] GetChangesGroupedByKey(int minPage, int maxPage, DateTime startDate = default(DateTime), DateTime endDate = default(DateTime))
		{
			var args = new JsonObject();
			if (default(DateTime) != startDate)
				args.Add("start_date", Tmdb.DateTimeToDate(startDate));
			if (default(DateTime) != endDate)
				args.Add("end_date", Tmdb.DateTimeToDate(endDate));
			var l = Tmdb.CollapsePagedJson(Tmdb.InvokeFlatPaged(string.Concat("/", string.Join("/", PathIdentity), "/changes"), minPage, maxPage, args));
			if (null != l)
			{
				var result = new KeyValuePair<string, TmdbChangeAction[]>[l.Count];
				for (var i = 0; i < result.Length; i++)
				{
					var ci = l[i] as IDictionary<string, object>;
					if (null != ci)
					{
						string key = null;
						TmdbChangeAction[] value;
						object o;
						if (ci.TryGetValue("key", out o))
							key = o as string;
						if (ci.TryGetValue("items", out o))
						{
							var ll = o as IList<object>;
							value = JsonArray.ToArray(ll, (dd) => new TmdbChangeAction((IDictionary<string, object>)dd));
							result[i] = new KeyValuePair<string, TmdbChangeAction[]>(key, value);
						}
					}
				}
				return result;
			}
			return null;
		}
		void _EnsureFetchedExternalIds()
		{
			var l = GetField<IList<object>>("external_ids");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/external_ids"));
				if (null != json)
					Json.Add("external_ids", json);
			}
		}
		void _EnsureFetchedVideos()
		{
			var l = GetField<IList<object>>("videos");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/videos"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("videos", l);
			}
		}
		void _EnsureFetchedCredits()
		{
			var credits = GetField<IList<object>>("credits");
			if (null != credits) return;
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/credits"));
			if (null != json)
				Json["credits"] = json;

		}
		void _EnsureFetchedImages()
		{
			var l = GetField<IList<object>>("images");
			if (null == l)
			{
				l = new JsonArray();
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/images"));
				var array = new JsonArray();
				object o;
				if (json.TryGetValue("backdrops", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "backdrop";
								l.Add(d);
							}
						}
					}
				}
				if (json.TryGetValue("posters", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "poster";
								l.Add(d);
							}
						}
					}
				}
				if (json.TryGetValue("profiles", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "profile";
								l.Add(d);
							}
						}
					}
				}
				Json.Add("images", l);
			}
		}
	}
}
