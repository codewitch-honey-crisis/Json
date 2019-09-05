using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{

	public abstract class TmdbMedia : TmdbPrimary
	{
		protected TmdbMedia(IDictionary<string, object> json) : base(json)
		{

		}
		protected TmdbMedia(int id) : base(id)
		{

		}
		public TmdbAlternativeName[] AlternativeNames {
			get {
				_EnsureFetchedAlternativeNames();
				var l = GetField<IList<object>>("alternative_names");
				return JsonArray.ToArray(l, (d) => new TmdbAlternativeName((IDictionary<string, object>)l));
			}
		}
		void _EnsureFetchedAlternativeNames()
		{
			var l = GetField<IList<object>>("alternative_names");
			if (null == l)
			{
				var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/alternative_titles"));
				object o;
				if (json.TryGetValue("titles", out o))
				{
					l = o as IList<object>;
					if (null != l)
						Json["alternative_names"] = l;
				}
			}
		}

		public virtual string OriginalName => GetCachedField<string>("original_name");
		public string OriginalLanguage => GetCachedField<string>("original_language");
		public string Overview => GetCachedField<string>("overview");
		public string Homepage => GetCachedField<string>("homepage");
		public string PosterPath => GetCachedField<string>("poster_path");
		public TmdbCompany[] ProductionCompanies
			=> JsonArray.ToArray(
				GetCachedField<IList<object>>("production_companies"),
				(d) => new TmdbCompany((IDictionary<string, object>)d));
		public string BackdropPath => GetCachedField<string>("backdrop_path");
		public double Popularity => GetCachedField("popularity", 0d);
		public double VoteAverage => GetCachedField("vote_average", 0d);
		public double VoteCount => GetCachedField("vote_count", 0);
		
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
						return JsonArray.ToArray(l, (d) => new TmdbCastMember((IDictionary<string,object>)d));
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
						return JsonArray.ToArray(l, (d) => new TmdbCrewMember((IDictionary<string,object>)d));
					}
				}
				return null;
			}
		}
		public KeyValuePair<int,string>[] KeywordsById {
			get {
				_EnsureFetchedKeywords();
				var l = GetField<IList<object>>("keywords");
				if (null != l)
					return Tmdb.ToKvpArray<int,string>(l,"id","name");
				return null;
			}
		}
		void _EnsureFetchedKeywords()
		{
			var l = GetField<IList<object>>("keywords");
			if (null == l)
			{
				var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/keywords"));
				object o;
				if (json.TryGetValue("keywords", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("keywords", l);
			}
		}
		public int TotalSeasons => GetCachedField("number_of_seasons", 0);
		public int TotalEpisodes => GetCachedField("number_of_episodes", 0);
		
		public TmdbVideo[] Videos {
			get {
				_EnsureFetchedVideos();
				var l = GetField<IList<object>>("videos");
				if (null != l)
					return JsonArray.ToArray(l, (d) => new TmdbVideo(d as IDictionary<string, object>));
				return null;
			}
		}
		void _EnsureFetchedVideos()
		{
			var l = GetField<IList<object>>("videos");
			if (null == l)
			{
				var json = Tmdb.InvokeLang(string.Concat("/", string.Join("/", PathIdentity), "/videos"));
				object o;
				if (json.TryGetValue("results", out o))
					l = o as IList<object>;
				if (null != l)
					Json.Add("videos", l);
			}
		}
		protected abstract TmdbMedia[] GetSimilarImpl(int minPage, int maxPage);
		public TmdbMedia[] GetSimilar(int minPage=0,int maxPage=999)
		{
			return GetSimilarImpl(minPage, maxPage);
		}
		public TmdbReview[] GetReviews(int minPage=0,int maxPage=999)
		{
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/reviews"), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbReview((IDictionary<string, object>)d));
		}
		public void Rate(double rating,TmdbSession session = null)
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
			Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/rating"), args,httpMethod:"DELETE");
		}
		public TmdbMediaAccountInfo GetAccountInfo(TmdbSession session = null)
		{
			var args = new JsonObject();
			if (null == session)
				args.Add("guest_session_id", Tmdb.GuestSessionId);
			else
				args.Add("session_id", session.Id);
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/account_states"));
			return new TmdbMediaAccountInfo(json);
		}
		
		public virtual TmdbLanguage[] Languages {
			get {
				EnsureFetchedLanguages();
				var l = GetField<IList<object>>("languages");
				return JsonArray.ToArray(l, (d) => new TmdbLanguage((IDictionary<string, object>)d));
			}
		}
		public virtual KeyValuePair<int,string>[] GenresById {
			get {
				var l = GetCachedField<IList<object>>("genres");
				if(null!=l)
					return Tmdb.ToKvpArray<int,string>(l, "id", "name");
				return null;
			}
		}
		
		protected void EnsureFetchedLanguages(string fieldName = "languages")
		{
			var l = GetField<IList<object>>(fieldName);
			if (null == l)
			{
				
				// this is ridiculous, but we have to join this with configuration data
				// which *may* spawn another http request depending on various factors
				// we only fetch it here if we need it
				TmdbLanguage[] cl = null;
				// join the language data on the iso field
				// or if it's a string array then join on the string
				for (int ic = l.Count, i = 0; i < ic; ++i)
				{
					string s = null;
					object o;
					var d = l[i] as IDictionary<string, object>;
					if (null == d)
						s = l[i] as string;
					else if (d.TryGetValue("iso_639_1", out o))
						s = o as string;	
					if(null!=s)
					{
						if (null == cl)
							cl = Tmdb.Configuration.Languages;
						if (null != cl)
						{
							for (var j = 0; j < cl.Length; j++)
							{
								var tl = cl[j];
								if (tl.Iso == s)
									l[i] = tl.Json;
							}
						}	
					}
				}
				Json.Add("languages", l);
				
			}
		}
		void _EnsureFetchedCredits()
		{
			var credits = GetField<IList<object>>("credits");
			if (null != credits) return;
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/credits"));
			if (null != json)
				Json["credits"] = json;	
		}
		
	}
}
