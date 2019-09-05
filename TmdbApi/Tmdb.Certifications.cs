using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	partial class Tmdb
	{
		public static KeyValuePair<string,TmdbCertification[]>[] MovieCertificationsGroupedByCountry {
			get {
				var d = GetField("movie_certifications", (IDictionary<string, object>)null);
				if (null == d)
				{
					var pt = Tmdb.Invoke("/certification/movie/list");
					if (null != pt)
					{
						object o;
						if (pt.TryGetValue("certifications", out o))
						{
							var dd = new JsonObject();
							Json.Add("movie_certifications", dd);
							JsonObject.CopyTo(o, dd);
							d = dd;
						}
					}
				}
				return _BuildCerts(d);
			}
		}
		public static KeyValuePair<string, TmdbCertification[]>[] ShowCertificationsGroupedByCountry {
			get {
				var d = GetField("show_certifications", (IDictionary<string, object>)null);
				if (null == d)
				{
					var pt = Tmdb.Invoke("/certification/tv/list");
					if (null != pt)
					{
						object o;
						if (pt.TryGetValue("certifications", out o))
						{
							var dd = new JsonObject();
							Json.Add("show_certifications", dd);
							JsonObject.CopyTo(o, dd);
							d = dd;
						}
					}
				}
				return _BuildCerts(d);
			}
		}

		private static KeyValuePair<string, TmdbCertification[]>[] _BuildCerts(IDictionary<string, object> json)
		{
			if (null != json)
			{
				var result = new KeyValuePair<string, TmdbCertification[]>[json.Count];
				if (null != json)
				{
					var ii = 0;
					foreach (var kvp in json)
					{
						var key = kvp.Key;
						var l = kvp.Value as IList<object>;
						var arr = new TmdbCertification[l.Count];
						for (var i = 0; i < arr.Length; i++)
							arr[i] = new TmdbCertification(l[i] as IDictionary<string, object>);
						Array.Sort(arr, (x, y) => {
							object o;
							int ox = 0;
							int oy = 0;
							if (x.Json.TryGetValue("order", out o) && o is int)
								ox = (int)o;
							if (y.Json.TryGetValue("order", out o) && o is int)
								oy = (int)o;
							return ox.CompareTo(oy);
						});
						result[ii] = new KeyValuePair<string, TmdbCertification[]>(key, arr);
						++ii;
					}
				}
				return result;

			}
			return null;
		}
	}
}
