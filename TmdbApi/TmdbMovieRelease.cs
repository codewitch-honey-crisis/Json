using Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TmdbApi
{
	public enum TmdbMovieReleaseType
	{
		Unknown = 0,
		Premiere = 1,
		TheatricalLimited = 2,
		Theatrical = 3,
		Digital = 4,
		Physical = 5,
		TV = 6
	}
	public sealed class TmdbMovieRelease : TmdbEntity
	{
		public TmdbMovieRelease(IDictionary<string, object> json) : base(json)
		{
		}
		
		public string Certification {
			get {
				return GetField<string>("certification");
			}
		}
		public TmdbMovie Movie {
			get {
				var id = GetField("id", -1);
				if (-1<id)
					return new TmdbMovie(id);
				return null;
			}
		}
		public string Language {
			get {
				return GetField<string>("iso_639_1");
			}
		}
		public string Note {
			get {
				return GetField<string>("note");
			}
		}
		public DateTime ReleaseDate {
			get {
				// we cut the time out of this
				var rd = GetField<string>("release_date");
				if (!string.IsNullOrEmpty(rd))
				{
					var i = rd.IndexOf('T');
					if (-1 < i)
						rd = rd.Substring(0, i);
					return Tmdb.DateToDateTime(rd);
				}
				return default(DateTime);
			}
		}
		public KeyValuePair<string, TmdbMovieRelease[]>[] ReleasesByCountry {
			get {
				_EnsureFetchedReleases();
				var l = GetField<IList<object>>("releases");
				if (null != l)
				{
					var result = new KeyValuePair<string, TmdbMovieRelease[]>[l.Count];
					for (var i = 0; i < result.Length; i++)
					{
						var d = l[i] as IDictionary<string, object>;
						object o;
						if (null != d && d.TryGetValue("release_dates", out o))
						{
							
							var ll = o as IList<object>;
							JsonArray.ToArray(ll, (dd) => new TmdbMovieRelease((IDictionary<string,object>)dd));
							var arr = new TmdbMovieRelease[ll.Count];
							for (var j = 0; j < arr.Length; j++)
							{
								d = ll[j] as IDictionary<string, object>;
								if (null != d)
									arr[j] = new TmdbMovieRelease(d);
							}
						}
					}
					return result;
				}
				return null;
			}
		}
		// optimization
		string[] _ParentPathIdentity
			=> new string[] { "movie",GetField("id",-1).ToString()};	
		
		void _EnsureFetchedReleases()
		{
			var l = GetField<IList<object>>("releases");
			if (null == l)
			{
				var json = Tmdb.Invoke(string.Concat("/", string.Join("/", _ParentPathIdentity), "/release_dates"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("keywords", l);
			}
		}
		public TmdbMovieReleaseType ReleaseType {
			get {
				return (TmdbMovieReleaseType)GetField("type", 0);
			}
		}
	}
}
