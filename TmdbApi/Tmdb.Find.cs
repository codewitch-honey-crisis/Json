using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	public enum TmdbExternalIdType
	{
		//imdb_id, freebase_mid, freebase_id, tvdb_id, tvrage_id, facebook_id, twitter_id, instagram_id
		ImdbId = 0,
		[Obsolete]
		FreebaseMid,
		[Obsolete]
		FreebaseId,
		TvdbId,
		[Obsolete]
		TvRageId,
		FacebookId,
		TwitterId,
		InstagramId
	}
	partial class Tmdb
	{
		public static TmdbPrimary[] Find(string externalId, TmdbExternalIdType type)
		{
			string t;
			switch (type)
			{
				case TmdbExternalIdType.ImdbId:
					t = "imdb_id";
					break;
#pragma warning disable 0612
				case TmdbExternalIdType.FreebaseMid:
#pragma warning restore 0612
					t = "freebase_mid";
					break;
#pragma warning disable 0612
				case TmdbExternalIdType.FreebaseId:
#pragma warning restore 0612
					t = "freebase_id";
					break;
				case TmdbExternalIdType.FacebookId:
					t = "facebook_id";
					break;
				case TmdbExternalIdType.InstagramId:
					t = "instagram_id";
					break;
				case TmdbExternalIdType.TwitterId:
					t = "twitter_id";
					break;
				case TmdbExternalIdType.TvdbId:
					t = "tvdb_id";
					break;
				default:
					t = null;
					break;
			}
			
			var hasResults = false;
			var args = new JsonObject();
			args.Add("external_source", t);
			var result = new List<TmdbPrimary>();
			var d = Tmdb.Invoke(string.Concat("/find/", externalId), args);
			if (null != d)
			{
				object o;
				if (d.TryGetValue("movie_results", out o))
				{
					var l = o as IList<object>;
					if (null != l)
					{
						result.AddRange(JsonArray.ToArray(l, (dd) => new TmdbMovie((IDictionary<string,object>)dd)));
						hasResults = true;
					}
				}
				if (d.TryGetValue("tv_results", out o))
				{
					var l = o as IList<object>;
					if (null != l)
					{
						result.AddRange(JsonArray.ToArray(l, (dd) => new TmdbShow((IDictionary<string, object>)dd)));
						hasResults = true;
					}
				}
				if (d.TryGetValue("person_results", out o))
				{
					hasResults = true;
					var l = o as IList<object>;
					if (null != l)
					{
						result.AddRange(JsonArray.ToArray(l, (dd) => new TmdbPerson((IDictionary<string, object>)dd)));
						hasResults = true;
					}
				}
				if (hasResults)
					return result.ToArray();
			}
			return null;
		}
	}
}
