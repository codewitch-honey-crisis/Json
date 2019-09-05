using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	partial class Tmdb
	{
		public static KeyValuePair<int, string>[] MovieGenresById {
			get {
				var d = InvokeLang("/genres/movie/list");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("genres",out o))
						return ToKvpArray<int, string>(o as IList<object>, "id", "name");
				}
				return null;
			}
		}
		
		public static KeyValuePair<int, string>[] ShowGenresById {
			get {
				var d = InvokeLang("/genres/tv/list");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("genres", out o))
						return ToKvpArray<int, string>(o as IList<object>, "id", "name");
					
				}
				return null;
			}
		}
	}
}
