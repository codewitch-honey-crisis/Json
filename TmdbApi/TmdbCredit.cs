using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbCreditType
	{
		Unknown=0,
		Movie=1,
		Show=2
	}
	/// <summary>
	/// Represents a TMDb TV or movie credit
	/// </summary>
	/// <remarks>This class is a bit strange because the underlying API is also strange. It tries to unweird it a little.</remarks>
	public class TmdbCredit : TmdbCachedEntityWithId2
	{
		public TmdbCredit(string id) : base(id)
		{
			InitializeCache();
		}
		public TmdbCredit(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public override string[] PathIdentity => new string[] { "credit", Id };
		protected override void Fetch()
		{
			FetchJson();
		}
		public TmdbPerson Person {
			get {
				var d = GetCachedField<IDictionary<string, object>>("person");
				if (null != d)
					return new TmdbPerson(d);
				return null;
			}
		}
		public string Department => GetCachedField<string>("department");
		public string Job => GetCachedField<string>("job");

		public string Character {
			get {
				var d = GetCachedField<IDictionary<string, object>>("media");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("character",out o))
						return o as string;
				}
				return null;
			}
		}
		public TmdbSeason[] MediaShowSeasons {
			get {
				var d = GetCachedField<IDictionary<string, object>>("media");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("seasons",out o))
					{
						var l = o as IList<object>;
						if (null != l)
							return JsonArray.ToArray(l, (dd) => new TmdbSeason((IDictionary<string, object>)dd));
					}
				}
				return null;
			}
		}
		public TmdbEpisode[] MediaShowEpisodes {
			get {
				var d = GetCachedField<IDictionary<string, object>>("media");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("episodes", out o))
					{
						var l = o as IList<object>;
						if (null != l)
							return JsonArray.ToArray(l, (dd) => new TmdbEpisode((IDictionary<string, object>)dd));
					}
				}
				return null;
			}
		}
		public TmdbMedia Media {
			get {
				var d = GetCachedField<IDictionary<string, object>>("media");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("media_type",out o))
					{
						switch(o as string)
						{
							case "tv":
								return new TmdbMovie(d);
							case "movie":
								return new TmdbShow(d);
						}
					}
				}
				return null;
			}
		}
	}
}
