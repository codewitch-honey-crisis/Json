using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	// TODO:I'm not sure if this is entirely accurate
	// also I'm not sure if it's consistent across the TMDb API
	public enum TmdbGender
	{
		Male = 0,
		Female = 1,
		Unknown = 2,

	}
	public class TmdbPerson : TmdbPrimary
	{
		public TmdbPerson(IDictionary<string, object> json) : base(json)
		{
			InitializeCache();
		}
		public TmdbPerson(int id) : base(id)
		{
			InitializeCache();
		}
		public override TmdbPrimaryType PrimaryType => TmdbPrimaryType.Person;
		public override string[] PathIdentity 
				=>new string[] { "person", Id.ToString() };
		public static TmdbPerson[] GetTrending(TmdbTimeWindow window, int minPage = 0, int maxPage = 999)
		{
			string tw = "day";
			switch (window)
			{
				case TmdbTimeWindow.Week:
					tw = "week";
					break;
			}
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePaged(string.Concat("/trending/person/", tw), minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbPerson((IDictionary<string, object>)d));
		}
		public static TmdbPerson[] GetPopular(int minPage, int maxPage)
		{
			var l = Tmdb.CollapsePagedJson(
				Tmdb.InvokePaged("/person/popular", minPage, maxPage));
			return JsonArray.ToArray(l, (d) => new TmdbPerson((IDictionary<string, object>)d));
		}
		public static TmdbPerson GetLatest()
		{
			var json = Tmdb.Invoke("/person/latest");
			if (null != json)
				return new TmdbPerson(json);
			return null;
		}
		public TmdbTaggedImage[] GetTaggedImages(int minPage=0,int maxPage=999)
		{
			return JsonArray.ToArray(Tmdb.CollapsePagedJson(
					Tmdb.InvokePagedLang(string.Concat("/", string.Join("/", PathIdentity), "/tagged_images"))),
					(d) => new TmdbTaggedImage((IDictionary<string, object>)d));
		}

		public TmdbRole[] MovieRoles {
			get {
				_EnsureFetchedMovieCredits();
				return _GetCredits("movie");
			}
		}
		public TmdbRole[] ShowRoles {
			get {
				_EnsureFetchedShowCredits();
				return _GetCredits("show");
			}
		}
		// gets all roles combined in a single (uncached) request
		public TmdbRole[] GetRoles()
		{
			var credits= Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/combined_credits"));
			if (null != credits)
			{
				var result = new List<TmdbRole>();
				// we're going to effectively "join" the cast and "crew" elements like we do with Images on the TmdbPrimaryClass.
				object o;
				var hasResults = false;
				if (credits.TryGetValue("cast", out o))
				{
					var l = o as IList<object>;
					hasResults = true;
					result.AddRange(JsonArray.ToArray(l, (d) => new TmdbCastMember((IDictionary<string, object>)d)));
				}
				if (credits.TryGetValue("crew", out o))
				{
					var l = o as IList<object>;
					hasResults = true;
					result.AddRange(JsonArray.ToArray(l, (d) => new TmdbCrewMember((IDictionary<string, object>)d)));
				}
				if (hasResults)
					return result.ToArray();
			}
			return null;
		}
		private TmdbRole[] _GetCredits(string type)
		{
			var result = new List<TmdbRole>();
			var credits = GetField(string.Concat(type,"_credits"), (IDictionary<string, object>)null);
			// we're going to effectively "join" the cast and "crew" elements like we do with Images on the TmdbPrimaryClass.
			if (null != credits)
			{
				object o;
				var hasResults = false;
				if (credits.TryGetValue("cast", out o))
				{
					var l = o as IList<object>;
					hasResults = true;
					result.AddRange(JsonArray.ToArray(l, (d) => new TmdbCastMember((IDictionary<string, object>)d)));
				}
				if (credits.TryGetValue("crew", out o))
				{
					var l = o as IList<object>;
					hasResults = true;
					result.AddRange(JsonArray.ToArray(l, (d) => new TmdbCrewMember((IDictionary<string, object>)d)));
				}
				if (hasResults)
					return result.ToArray();
			}
			return null;
		}

		void _EnsureFetchedMovieCredits()
		{
			var credits = GetField<IList<object>>("movie_credits");
			if (null != credits) return;
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/movie_credits"));
			if (null != json)
				Json["movie_credits"] = json;
		}
		void _EnsureFetchedShowCredits()
		{
			var credits = GetField<IList<object>>("show_credits");
			if (null != credits) return;
			var json = Tmdb.Invoke(string.Concat("/", string.Join("/", PathIdentity), "/show_credits"));
			if (null != json)
				Json["show_credits"] = json;
		}

	}
}
