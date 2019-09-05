using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbMovieList :TmdbEntity
	{
		
		public TmdbMovieList(IDictionary<string,object> json) : base(json)
		{
		}
		
		public string Name => GetField<string>("name");
		public string Description => GetField<string>("description");
		public string Language => GetField<string>("iso_639_1");
		public string PosterPath => GetField<string>("poster_path");
		public string CreatedBy => GetField<string>("created_by");
		public int FavoriteCount => GetField("favorite_count", 0);
		public TmdbMovie[] Movies
			=> JsonArray.ToArray(GetField<IList<object>>("items"), (d) => new TmdbMovie((IDictionary<string, object>)d));
		// this isn't really a great idea since it sends a web request to get one bit of info
		// you can get all at once using the Movies property, albeit indirectly
		public bool ContainsMovie(TmdbMovie movie)
		{
			var args = new JsonObject();
			args.Add("movie_id", movie.Id);
			var id = GetField("id", -1); // optimization
			IDictionary<string, object> d=null;
 			if (-1!=id)
				d = Tmdb.Invoke(string.Concat("/list/", id.ToString(), "/item_status", args));
			if(null!=d)
			{
				object o;
				if (d.TryGetValue("item_present", out o) && o is bool && (bool)o)
					return true;
			}
			return false;
		}
		public void AddMovie(TmdbMovie movie, string sessionId)
		{
			if(null==movie) throw new ArgumentNullException(nameof(movie));
			if (null == sessionId) throw new ArgumentNullException(nameof(sessionId));			
			if (0== sessionId.Length) throw new ArgumentException(nameof(sessionId)+" must not be empty.",nameof(sessionId));
			var movieId = movie.Id;
			var args = new JsonObject();
			args.Add("session_id", sessionId);
			var payload = new JsonObject();
			payload.Add("media_id", movieId);
			var id = GetField("id", -1);
			if (-1 < id)
				Tmdb.Invoke(string.Concat("/list/",id.ToString(),"/add_item"), args, payload);
			else throw new Exception("Could not add the movie");
		}
		public void RemoveMovie(TmdbMovie movie, string sessionId)
		{
			if (null == movie) throw new ArgumentNullException(nameof(movie));
			if (null == sessionId) throw new ArgumentNullException(nameof(sessionId));
			if (0 == sessionId.Length) throw new ArgumentException(nameof(sessionId) + " must not be empty.", nameof(sessionId));
			var movieId = movie.Id;
			var args = new JsonObject();
			args.Add("session_id", sessionId);
			var payload = new JsonObject();
			payload.Add("media_id", movieId);
			var id = GetField("id", -1);
			if (-1 < id)
				Tmdb.Invoke(string.Concat("/list/" + id.ToString() + "/remove_item"), args, payload);
		}
		public void ClearMovies(string sessionId)
		{
			if (null == sessionId) throw new ArgumentNullException(nameof(sessionId));
			if (0 == sessionId.Length) throw new ArgumentException(nameof(sessionId) + " must not be empty.", nameof(sessionId));
			var args = new JsonObject();
			args.Add("session_id", sessionId);
			args.Add("confirm", true);
			var id = GetField("id", -1);
			if (-1 < id)
				Tmdb.Invoke(string.Concat("/list/" + id.ToString()+ "/remove_item"), args,httpMethod:"POST");
		}

	}
}
