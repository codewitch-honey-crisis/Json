using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbKeyword :TmdbCachedEntityWithId
	{
		public TmdbKeyword(int id) : base(id)
		{
			InitializeCache();
		}
		public TmdbKeyword(IDictionary<string,object> json) : base(json)
		{
			InitializeCache();
		}
		public string Name => GetField<string>("name");

		public override string[] PathIdentity => new string[] { "keyword", Id.ToString() };
		protected override void Fetch()
		{
			FetchJson();
		}
		public TmdbMovie[] GetMovies(bool includeAdult=false)
		{ 
				// this routine does not accept a page parameter but returns the results
				// in paged format
				var args = new JsonObject();
			if(includeAdult)
				args.Add("include_adult", true);
			return JsonArray.ToArray(
				Tmdb.CollapsePagedJson(Tmdb.InvokePagedLang(string.Concat("/keyword/", Id.ToString(), "/movies"),0,0,args)),
					(d) => new TmdbMovie((IDictionary<string, object>)d));
			
		}
	}
}
