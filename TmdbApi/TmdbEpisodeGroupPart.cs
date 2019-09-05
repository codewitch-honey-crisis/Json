using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbEpisodeGroupPart : TmdbEntity
	{
		public TmdbEpisodeGroupPart(IDictionary<string,object> json) : base(json)
		{
		}
		public string Id => GetField<string>("id");
		public string Name => GetField<string>("name");
		public int Order => GetField("order",0);
		public bool Locked => GetField("locked", false);
		public TmdbEpisode[] Episodes
			=> JsonArray.ToArray(GetField<IList<object>>("episodes"), (d) => new TmdbEpisode((IDictionary<string, object>)d));

	}
}
