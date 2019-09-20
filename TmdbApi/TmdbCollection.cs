using Json;
using System;
using System.Collections.Generic;
using System.Text;
using Bee;
namespace TmdbApi
{
	public sealed class TmdbCollection : TmdbCachedEntityWithId
	{
		public TmdbCollection(int id) : base(id)
		{
		}
		public TmdbCollection(IDictionary<string,object> json) : base(json)
		{
		}
		public string Name => GetCachedField<string>("name");
		public override string[] PathIdentity => new string[] { "collection",Id.ToString() };
		public TmdbCastMember[] Cast {
			get {
				_EnsureFetchedCredits();
				var credits = GetField("credits", (IDictionary<string, object>)null);
				if (null != credits)
				{
					object o;
					if (credits.TryGetValue("cast", out o))
					{
						var l = o as IList<object>;
						return JsonArray.ToArray(l, (d) => new TmdbCastMember((IDictionary<string, object>)d));
					}
				}
				return null;
			}
		}
		public TmdbCrewMember[] Crew {
			get {
				_EnsureFetchedCredits();
				var credits = GetField("credits", (IDictionary<string, object>)null);
				if (null != credits)
				{
					object o;
					if (credits.TryGetValue("crew", out o))
					{
						var l = o as IList<object>;
						return JsonArray.ToArray(l, (d) => new TmdbCrewMember((IDictionary<string, object>)d));
					}
				}
				return null;
			}
		}
		public string Overview {
			get {
				return GetCachedField<string>("overview");
			}
		}
		public string PosterPath {
			get {
				return GetCachedField<string>("poster_path");
			}
		}
		public string BackdropPath {
			get {
				return GetCachedField<string>("backdrop_path");
			}
		}
		public TmdbMovie[] Movies {
			get {
				var l = GetCachedField<IList<object>>("parts");
				if(null!=l)
					return JsonArray.ToArray(l, (d) => new TmdbMovie(d as IDictionary<string,object>));
				return null;
			}
		}
		public KeyValuePair<int, string>[] GenresById {
			get {
				var l = GetCachedField<IList<object>>("genres");
				if (null != l)
					return Tmdb.ToKvpArray<int, string>(l, "id", "name");
				return null;
			}
		}
		public TmdbImage[] Images {
			get {
				_EnsureFetchedImages();
				var l = GetField<IList<object>>("images");
				if(null!=l)
					return JsonArray.ToArray(l, (d) => new TmdbImage(d as IDictionary<string,object>));
				return null;
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
		public TmdbReview[] GetReviews(int minPage = 0, int maxPage = 999)
		{
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/reviews"), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbReview((IDictionary<string, object>)d));
		}
		public void Rate(double rating, TmdbSession session = null)
		{
			if (0.5d > rating || 10d < rating)
				throw new ArgumentOutOfRangeException(nameof(rating));
			var args = new JsonObject();
			if (null != session)
				args.Add("session_id", session.Id);
			else
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			var payload = new JsonObject();
			payload.Add("value", rating);
			Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/rating"), args, payload);
		}
		public void ClearRating(TmdbSession session = null)
		{
			var args = new JsonObject();
			if (null != session)
				args.Add("session_id", session.Id);
			else
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/rating"), args, httpMethod: "DELETE");
		}
		void _EnsureFetchedCredits()
		{
			var credits = GetField<IList<object>>("credits");
			if (null != credits) return;
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/credits"));
			if (null != json)
				Json["credits"] = json;
		}
		void _EnsureFetchedImages()
		{
			var l = GetField<IList<object>>("images");
			if(null==l)
			{
				l = new JsonArray().Synchronize() as IList<object>;
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/images"));
				var array = new JsonArray().Synchronize() as IList<object>;
				object o;
				if(json.TryGetValue("backdrops",out o))
				{
					var ll = o as IList<object>;
					if(null!=ll)
					{
						for(int ic=ll.Count,i=0;i<ic;++i)
						{
							var d = ll[i] as IDictionary<string, object>;
							if(null!=d)
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
				Json.Add("images", l);
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
				if(null!=l)
					Json.Add("translations", l);
			}
		}
	}
}
