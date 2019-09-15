using System;
using System.Collections.Generic;
using System.Text;
using Json;
using Bee;
namespace TmdbApi
{
	public sealed class TmdbConfiguration : TmdbCachedEntityWithId
	{
		private static readonly string[] _pathIdentity = new string[] { "configuration" };
		public TmdbConfiguration(IDictionary<string, object> json) : base(json)
		{
			InitializeCache();
		}
		public TmdbConfiguration() : base(new JsonObject().Synchronize())
		{
			InitializeCache();
		}
		public override string[] PathIdentity {
			get {
				return _pathIdentity;
			}
		}
		public string[] ChangeKeys {
			get {
				var l = GetCachedField<IList<object>>("change_keys", null);
				return JsonArray.ToArray<string>(l);
			}
		}
		
		public ImagesEntry Images {
			get {
				var d = GetCachedField<IDictionary<string, object>>("images", null);
				if (null != d)
					return new ImagesEntry(d);
				return null;
			}
		}
		public TmdbLanguage[] Languages {
			get {
				_EnsureFetchedLanguages();
				var l = GetField<IList<object>>("languages");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbLanguage((IDictionary<string, object>)d));
				return null;
			}
		}
		public string[] PrimaryTranslationsIetf {
			get {
				_EnsureFetchedPrimaryTranslations();
				var l = GetField<IList<object>>("primary_translations", null);
				if (null != l)
					return JsonArray.ToArray<string>(l);
				return null;
			}
		}
		public KeyValuePair<string, string>[] CountriesByIso31661 {
			get {
				_EnsureFetchedCountries();
				var countries = GetField<IList<object>>("countries");
				return Tmdb.ToKvpArray<string, string>(countries, "iso_3166_1", "name");
			}
		}
		public KeyValuePair<string, string[]>[] TimeZonesGroupedByCountry {
			get {
				_EnsureFetchedTimeZones();
				var timezones = GetField<IList<object>>("timezones");
				if (null != timezones)
				{
					var result = new KeyValuePair<string, string[]>[timezones.Count];
					for (var i = 0; i < result.Length; i++)
					{
						var d = timezones[i] as IDictionary<string, object>;
						if (null != d)
						{
							string key = null;
							object o;
							if (d.TryGetValue("iso_3166_1", out o))
								key = o as string;
							if (d.TryGetValue("zones", out o))
							{
								var jl = o as IList<object>;
								if (null != jl)
								{
									var arr = JsonArray.ToArray<string>(jl);
									result[i] = new KeyValuePair<string, string[]>(key, arr);
								}
							}
						}
					}
					return result;
				}
				return null;
			}
		}
		public KeyValuePair<string, string[]>[] JobsGroupedByDepartment {
			get {
				_EnsureFetchedJobs();
				var jobs = GetField<IList<object>>("jobs");
				if (null != jobs)
				{
					var result = new KeyValuePair<string, string[]>[jobs.Count];
					for (var i = 0; i < result.Length; i++)
					{
						var d = jobs[i] as IDictionary<string, object>;
						if (null != d)
						{
							string key = null;
							object o;
							if (d.TryGetValue("department", out o))
								key = o as string;
							if (d.TryGetValue("jobs", out o))
							{
								var jl = o as IList<object>;
								if (null != jl)
								{
									var arr = JsonArray.ToArray<string>(jl);
									result[i] = new KeyValuePair<string, string[]>(key, arr);
								}
							}
						}
					}
					return result;
				}
				return null;
			}
		}
		void _EnsureFetchedLanguages()
		{
			var l = GetField<IList<object>>("languages");
			if (null == l)
			{
				l = Tmdb.InvokeLangEx(string.Concat("/", string.Join("/", PathIdentity), "/languages")) as IList<object>;
				if (null != l)
					Json.Add("languages", l);
			}
		}
		void _EnsureFetchedCountries()
		{
			var l = GetField<IList<object>>("countries");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/countries"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("countries", l);
			}
		}
		void _EnsureFetchedJobs()
		{
			var l = GetField<IList<object>>("jobs");
			if (null == l)
			{
				l = Tmdb.InvokeLangEx(string.Concat("/", string.Join("/", PathIdentity), "/jobs")) as IList<object>;
				if (null != l)
					Json.Add("jobs", l);
			}
		}
		void _EnsureFetchedTimeZones()
		{
			var l = GetField<IList<object>>("timezones");
			if (null == l)
			{
				l = Tmdb.InvokeLangEx(string.Concat("/", string.Join("/", PathIdentity), "/timezones")) as IList<object>;
				if (null != l)
					Json.Add("timezones", l);
			}
		}
		void _EnsureFetchedPrimaryTranslations()
		{
			var l = GetField<IList<object>>("primary_translations");
			if (null == l)
			{
				l = Tmdb.InvokeLangEx(string.Concat("/", string.Join("/", PathIdentity), "/primary_translations")) as IList<object>;
				if (null != l)
					Json.Add("primary_translations", l);
			}
		}
		public sealed class ImagesEntry : TmdbEntity
		{
			internal ImagesEntry(IDictionary<string,object> json) :base(json)
			{
			}
			public string BaseUrl {
				get {
					return GetField<string>("base_url", null);
				}
			}
			public string SecureBaseUrl {
				get {
					return GetField<string>("secure_base_url", null);
				}
			}
			public string[] BackdropSizes {
				get {
					var l= GetField<IList<object>>("backdrop_sizes", null);
					if (null != l)
						return JsonArray.ToArray<string>(l);
					return null;
				}
			}
			public string[] LogoSizes {
				get {
					var l = GetField<IList<object>>("logo_sizes", null);
					if (null != l)
						return JsonArray.ToArray<string>(l);
					return null;
				}
			}
			public string[] PosterSizes {
				get {
					var l = GetField<IList<object>>("poster_sizes", null);
					if (null != l)
						return JsonArray.ToArray<string>(l);
					return null;
				}
			}
			public string[] ProfileSizes {
				get {
					var l = GetField<IList<object>>("profile_sizes", null);
					if (null != l)
						return JsonArray.ToArray<string>(l);
					return null;
				}
			}
			public string[] StillSizes {
				get {
					var l = GetField<IList<object>>("still_sizes", null);
					if (null != l)
						return JsonArray.ToArray<string>(l);
					return null;
				}
			}
		}
	}
}
