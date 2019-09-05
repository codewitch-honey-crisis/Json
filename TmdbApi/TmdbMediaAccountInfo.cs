using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbMediaAccountInfo : TmdbEntity
	{
		public TmdbMediaAccountInfo(IDictionary<string, object> json) : base(json)
		{
		}
		public bool IsFavorite => GetField("favorite", false);
		public bool IsOnWatchlist => GetField("watchlist", false);
		// this value can return NaN
		public double Rated {
			get {
				// this value is weird.
				// TODO: It can be a boolean, i don't know what that means, except maybe in the case of false
				// so we treat a boolean as "not yet rated" and return NaN
				var d = GetField<IDictionary<string, object>>("rated");
				if(null!=d)
				{
					object o;
					if(d.TryGetValue("value",out o))
					{
						if (o is int)
							return (int)o;
						else if (o is double)
							return (double)o;
					}
				}
				return double.NaN;
			}
		}
	}
}
