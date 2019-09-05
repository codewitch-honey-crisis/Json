using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbReview : TmdbCachedEntityWithId2
	{
		public TmdbReview(string id) : base(id)
		{
			InitializeCache();
		}
		public TmdbReview(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public override string[] PathIdentity => new string[] { "review", Id };
		public string Author => GetCachedField<string>("author");
		public string Content => GetCachedField<string>("content");
		public string Language => GetCachedField<string>("iso_639_1");
		public string Url => GetCachedField<string>("url");

		public TmdbMedia Media {
			get {
				// HACK: The API returns fields called "media_title", and "media_id" which are
				// useless for us and inconsistent to the rest of the TMDb API so we normalize it
				// as its own json object. While doing this we fixup the json to "name" or 
				// "title" depending on whether this is a movie or TV
				var m = GetCachedField<string>("media_type");
				if (null != m)
				{
					var mid = GetCachedField("media_id",-1);
					var mt = GetCachedField<string>("media_title");
					if (-1 < mid)
					{
						var obj = new JsonObject();
						obj["id"] = mid;
						switch (m)
						{
							case "movie":
								if (null != mt)
									obj["title"] = mt;
								return new TmdbMovie(obj);
							case "tv":
								if (null != mt)
									obj["name"] = mt;
								return new TmdbShow(obj);
							default:
								break;
						}
					}
				}
				return null;
			}
		}
	}
}
