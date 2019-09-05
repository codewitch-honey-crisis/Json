using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbTaggedImage : TmdbImage
	{
		public TmdbTaggedImage(IDictionary<string,object> json) : base(json)
		{
		}
		public TmdbMedia Media {
			get {
				var media = GetField<IDictionary<string,object>>("media");
				var media_type = GetField<string>("media_type");
				if(null!=media)
				{
					switch(media_type)
					{
						case "movie":
							return new TmdbMovie(media);
						case "tv":
							return new TmdbShow(media);
					}
				}
				return null;
			}
		}
	}
}
