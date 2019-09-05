using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbCompany : TmdbCachedEntityWithId
	{
		public TmdbCompany(int id) : base(id)
		{
			InitializeCache();
		}
		public TmdbCompany(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public override string[] PathIdentity => new string[] { "company", Id.ToString() };
		protected override void Fetch()
		{
			// this is one of the primaries that *doesn't* use the language parameter
			FetchJson();
		}
		public string Description => GetCachedField<string>("description");
		public string Homepage => GetCachedField<string>("homepage");
		public string Headquarters => GetCachedField<string>("headquarters");
		public string Country => GetCachedField<string>("origin_country");

		public TmdbCompany ParentCompany {
			get {
				var d = GetCachedField<IDictionary<string, object>>("parent_company");
				if (null != d)
					return new TmdbCompany(d);
				return null;
			}
		}
		public TmdbAlternativeName[] AlternativeNames {
			get {
				_EnsureFetchedAltNames();
				var l = GetField<IList<object>>("alternative_names");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbAlternativeName(d as IDictionary<string, object>));
				return null;
		
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
		public string LogoPath => GetCachedField<string>("logo_path");
	
		void _EnsureFetchedAltNames()
		{
			var l = GetField<IList<object>>("alternative_names");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/alternative_names"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if(null!=l)
					Json.Add("alternative_names", l);
			}
		}
		void _EnsureFetchedImages()
		{
			var l = GetField<IList<object>>("images");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/images"));
				object o;
				if (json.TryGetValue("logos", out o))
					l = o as IList<object>;
				if(null!=l)
					Json.Add("images", l);
			}
		}
	}
}
