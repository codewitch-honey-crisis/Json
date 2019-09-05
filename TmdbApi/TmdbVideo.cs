using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbVideoType
	{
		Unknown = 0,
		Trailer = 1,
		Teaser = 2,
		Clip = 3,
		Featurette = 4,
		OpeningCredits = 5,
		BehindTheScenes = 6,
		Bloopers = 7
	}
	public class TmdbVideo : TmdbCachedEntityWithId2
	{
		public TmdbVideo(IDictionary<string, object> json) : base(json)
		{
		}
		protected override void Fetch()
		{
			throw new NotImplementedException();
		}
		public override string[] PathIdentity => null;
		public string Language {
			get {
				return GetField<string>("iso_639_1");
			}
		}
		public string Country {
			get {
				return GetField("iso_3166_1", (string)null);
			}
		}
		public string Key {
			get {
				return GetField("key", (string)null);
			}
		}
		public string Site {
			get {
				return GetField("site", (string)null);
			}
		}
		public int Height {
			get {
				return GetField("size", 0);
			}
		}
		public TmdbVideoType VideoType {
			get {
				return (TmdbVideoType)GetField("type", 0);
			}
		}
	}
}
