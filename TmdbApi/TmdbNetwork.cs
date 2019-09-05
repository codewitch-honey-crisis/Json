using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbNetwork : TmdbCachedEntityWithId
	{
		public TmdbNetwork(int id) : base(id)
		{
			InitializeCache();
		}
		public TmdbNetwork(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public override string[] PathIdentity => new string[] { "network", Id.ToString() };
		public string Homepage => GetCachedField<string>("homepage");
		public string Headquarters => GetCachedField<string>("headquarters");
		public string Country => GetCachedField<string>("origin_country");
		public TmdbAlternativeName[] AlternativeNames {
			get {
				_EnsureFetchedAlternativeNames();
				var l = GetField<IList<object>>("alternative_names");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbAlternativeName(d as IDictionary<string, object>));
				return null;

			}
		}
		void _EnsureFetchedAlternativeNames()
		{
			var l = GetField<IList<object>>("alternative_names");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/alternative_names"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("alternative_names", l);
			}
		}
		public TmdbImage[] Images {
			get {
				_EnsureFetchedImages();
				var l = GetField<IList<object>>("images");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbImage(d as IDictionary<string, object>));
				return null;
			}
		}
		void _EnsureFetchedImages()
		{
			var l = GetField<IList<object>>("images");
			if (null == l)
			{
				l = new JsonArray();
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/images"));
				var array = new JsonArray();
				object o;
				if (json.TryGetValue("logos", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "logo";
								l.Add(d);
							}
						}
					}
				}
				Json.Add("images", l);
			}
		}

	}

}
