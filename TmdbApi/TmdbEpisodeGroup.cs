using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbEpisodeGroup : TmdbCachedEntityWithId2
	{
		public TmdbEpisodeGroup(string id) : base(id)
		{
		}
		public TmdbEpisodeGroup(IDictionary<string,object> json) : base(json)
		{
		}
		public override string[] PathIdentity => new string[] { "tv", "episode_group", Id };

		public string Name => GetCachedField<string>("name");
		public string Description => GetCachedField<string>("description");
		public TmdbNetwork Network {
			get {
				var d = GetCachedField<IDictionary<string, object>>("network");
				if (null != d)
					return new TmdbNetwork(d);
				return null;
			}
		}
		// TODO: Find out what this means. It's completely undocumented
		public int Type => GetCachedField("type",-1);

		public int TotalParts => GetCachedField("group_count", 0);

		// this is named strangely in the API so I've adjusted it.
		public TmdbEpisodeGroupPart[] Parts
			=> JsonArray.ToArray(
				GetCachedField<IList<object>>("groups"),
				(d) => new TmdbEpisodeGroupPart((IDictionary<string, object>)d));
	}
}
