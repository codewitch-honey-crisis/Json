using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbEpisode : TmdbCachedEntity
	{
		public TmdbEpisode(int showId, int seasonNumber,int episodeNumber) : 
			base(_CreateJson(showId, seasonNumber,episodeNumber))
		{
			InitializeCache();
		}
		static IDictionary<string, object> _CreateJson(int showId, int seasonNumber, int episodeNumber)
		{
			var result = new JsonObject();
			result.Add("show_id", showId);
			result.Add("season_number", seasonNumber);
			result.Add("episode_number", episodeNumber);
			return result;
		}
		public TmdbEpisode(IDictionary<string, object> json) : base(json)
		{
			InitializeCache();
		}
		// /tv/{show_id}/season/{season_number}/episode/{episode_number}
		public override string[] PathIdentity
			=> new string[] {
				"tv",
				GetField("show_id", -1).ToString(),
				"season",
				GetField("season_number", -1).ToString(),
				"episode",
				GetField("episode_number", -1).ToString(),
			};
		public TmdbShow Show {
			get {
				int showId = GetField("show_id", -1);
				if (-1 < showId)
					return new TmdbShow(showId);
				return null;
			}
		}
		public TmdbSeason Season {
			get {
				int showId = GetField("show_id", -1);
				if (-1 < showId)
				{
					int seasonNum = GetField("season_number", -1);
					if (-1 < seasonNum)
						return new TmdbSeason(showId,seasonNum);
				}
				return null;
			}
		}
		public int Number => GetField("episode_number", -1);
		
		public string Name => GetCachedField<string>("name");
		
		public DateTime AirDate => Tmdb.DateToDateTime(GetCachedField<string>("air_date"));
		// TODO: this keeps a copy of crew around but we load it with the credits
		// I think it's the same data but I can't be positive, and also if it is
		// this should be merged with the credits
		// right now this is a seperate copy!
		public TmdbCrewMember[] Crew
			=> JsonArray.ToArray(
				GetCachedField<IList<object>>("crew"),
				(d)=>new TmdbCrewMember((IDictionary<string,object>)d));
		public TmdbCastMember[] GuestStars
			=> JsonArray.ToArray(
				GetCachedField<IList<object>>("guest_stars"),
				(d) => new TmdbCastMember((IDictionary<string, object>)d));
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
		// TODO: figure out what this means and make an enum possibly
		public string ProductionCode => GetCachedField<string>("production_code");
		public string Overview => GetCachedField<string>("overview");
		public string StillPath => GetCachedField<string>("still_path");
		// TODO: refactor this. It's a duplicate from TmdbMedia and TmdbPerson, and TmdbSeason
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
		// TODO: we currently retrieve this from the inline crew data, see the associated note on the Crew property
		/*public TmdbCrewMember[] Crew {
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
		}*/
		public TmdbImage[] Images {
			get {
				_EnsureFetchedImages();
				var l = GetField<IList<object>>("images");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbImage(d as IDictionary<string, object>));
				return null;
			}
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
		public TmdbTranslation[] Translations {
			get {
				_EnsureFetchedTranslations();
				var l = GetField<IList<object>>("translations");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbTranslation(d as IDictionary<string, object>));
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
		public double VoteAverage => GetCachedField("vote_average", 0d);
		public int VoteCount => GetCachedField("vote_count", 0);

		public void Rate(double rating, TmdbSession session = null)
		{
			if (0.5d > rating || 10d < rating)
				throw new ArgumentOutOfRangeException(nameof(rating));
			var args = new JsonObject();
			if (null != session)
				args.Add("session_id", session.Id);
			else
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			var payload = new JsonObject();
			payload.Add("value", rating);
			Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/rating"), args, payload);
		}
		public void ClearRating(TmdbSession session = null)
		{
			var args = new JsonObject();
			if (null != session)
				args.Add("session_id", session.Id);
			else
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/rating"), args, httpMethod: "DELETE");
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
		void _EnsureFetchedTranslations()
		{
			var l = GetField<IList<object>>("translations");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/translations"));
				object o;
				if (json.TryGetValue("translations", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("translations", l);
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
	}
}
