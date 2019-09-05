using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbAccount : TmdbCachedEntityWithId
	{
		public TmdbAccount(int id) : base(id)
		{
			// this entity is not cached
		}
		public TmdbAccount(IDictionary<string,object> json) : base(json)
		{
			// this entity is not cached
		}
		protected override void Fetch()
		{
			throw new NotImplementedException();
		}
		public override string[] PathIdentity => null;
		public string Username => GetField<string>("username");
		public string Country => GetField<string>("iso_3166_1");
		public string Language => GetField<string>("iso_639_1");
		public bool IncludeAdult => GetField("include_adult",false);
		public string GravatarHash {
			get {
				object o;
				var d = GetField<IDictionary<string, object>>("avatar");
				if (null!=d && d.TryGetValue("gravatar",out o))
				{
					var dd = o as IDictionary<string, object>;
					if(null!=dd && dd.TryGetValue("hash",out o))
						return o as string;
				}
				return null;
			}
		}

	}
}
