using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbPrimaryType
	{
		Movie=0,
		Show,
		Person
	}
	// person, movie, or show

	public abstract class TmdbPrimary :TmdbCachedEntityWithId
	{
		protected TmdbPrimary(int id) : base(id)
		{
		}
		protected TmdbPrimary(IDictionary<string,object> json) : base(json)
		{
		}
		public abstract TmdbPrimaryType PrimaryType { get; }
		public virtual string Name {
			get {
				return GetCachedField<string>("name", null);
			}
		}
		public virtual string ImdbId {
			get {
				EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("imdb_id", out o))
						return o as string;
				}
				return null;
			}
		}
		public string FacebookId {
			get {
				EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("facebook_id", out o))
						return o as string;
				}
				return null;
			}
		}
		public string InstagramId {
			get {
				EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("instagram_id", out o))
						return o as string;
				}
				return null;
			}
		}
		public string TwitterId {
			get {
				EnsureFetchedExternalIds();
				var d = GetField<IDictionary<string, object>>("external_ids");
				if (null != d)
				{
					object o;
					if (d.TryGetValue("twitter_id", out o))
						return o as string;
				}
				return null;
			}
		}
		protected void EnsureFetchedExternalIds()
		{
			var l = GetField<IList<object>>("external_ids");
			if (null == l)
			{
				var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/external_ids"));
				if (null != json)
					Json.Add("external_ids", json);
			}
		}
		public TmdbTranslation[] Translations {
			get {
				_EnsureFetchedTranslations();
				var l = GetField<IList<object>>("translations");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbTranslation(d as IDictionary<string, object>));
				return null;
			}
		}
		void _EnsureFetchedTranslations()
		{
			var l = GetField<IList<object>>("translations");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/translations"));
				object o;
				if (json.TryGetValue("translations", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("translations", l);
			}
		}
		public KeyValuePair<string, TmdbChangeAction[]>[] GetChangesGroupedByKey(int minPage, int maxPage, DateTime startDate = default(DateTime), DateTime endDate = default(DateTime))
		{
			var args = new JsonObject();
			if (default(DateTime) != startDate)
				args.Add("start_date", Tmdb.DateTimeToDate(startDate));
			if (default(DateTime) != endDate)
				args.Add("end_date", Tmdb.DateTimeToDate(endDate));
			var l = Tmdb.CollapsePagedJson(Tmdb.InvokeFlatPaged(string.Concat("/", string.Join("/", PathIdentity), "/changes"), minPage, maxPage, args));
			if (null != l)
			{
				var result = new KeyValuePair<string, TmdbChangeAction[]>[l.Count];
				for (var i = 0; i < result.Length; i++)
				{
					var ci = l[i] as IDictionary<string, object>;
					if (null != ci)
					{
						string key = null;
						TmdbChangeAction[] value;
						object o;
						if (ci.TryGetValue("key", out o))
							key = o as string;
						if (ci.TryGetValue("items", out o))
						{
							var ll = o as IList<object>;
							value = JsonArray.ToArray(ll, (dd) => new TmdbChangeAction((IDictionary<string, object>)dd));
							result[i] = new KeyValuePair<string, TmdbChangeAction[]>(key, value);
						}
					}
				}
				return result;
			}
			return null;
		}
		public virtual TmdbImage[] Images {
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
				if (json.TryGetValue("backdrops", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "backdrop";
								l.Add(d);
							}
						}
					}
				}
				if (json.TryGetValue("posters", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "poster";
								l.Add(d);
							}
						}
					}
				}
				if (json.TryGetValue("profiles", out o))
				{
					var ll = o as IList<object>;
					if (null != ll)
					{
						for (int ic = ll.Count, i = 0; i < ic; ++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if (null != d)
							{
								d["image_type"] = "profile";
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
