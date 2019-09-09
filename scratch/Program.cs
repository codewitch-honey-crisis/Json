using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json;
using TmdbApi;
namespace scratch
{
	class Program
	{
		const string Username = "myUsername";
		const string Password = "myPassword";
		const string ApiKey = "c83a68923b7fe1d18733e8776bba59bb";
		static void Main()
		{
			using (var reader = JsonTextReader.CreateFrom(@"..\..\data.json"))
			{
				if (reader.SkipTo("seasons",0,"name"))
				{
					// move past the field name "name"
					if (JsonNodeType.Key!=reader.NodeType || reader.Read())
					{
						Console.WriteLine(reader.ParseSubtree());
					}
				}
			}
			// or...
			using (var reader = JsonTextReader.CreateFrom(@"..\..\data.json"))
			{

				if (reader.SkipToField("created_by"))
				{
					if (reader.SkipToIndex(0))
					{
						reader.Read();
						if (reader.SkipToField("name"))
						{
							if (reader.Read())
							{
								Console.WriteLine(reader.Value);
							}
						}
					}
				}
			}
			
			return;
			Tmdb.ApiKey = ApiKey;
			Tmdb.CacheLevel = JsonRpcCacheLevel.Aggressive;
			
			// hit it *hard* - 10 pages of movies, 10 of TV
			foreach (var movie in TmdbMovie.GetTopRated(0,9))
			{
				_RunMovieDemo(movie.Id);
			}

			foreach (var show in TmdbShow.GetTopRated(0,9))
			{
				_RunShowDemo(show.Id);
			}
			
		}
		static void _RunCacheDemo2()
		{
			// demos the secondary (url based) caching
			// make it hit the server less (about every
			// 5 minutes usually (depending on machine 
			// policy)
			Tmdb.CacheLevel = JsonRpcCacheLevel.Aggressive;

			// grab the show changes just so we have something to work with.
			// this is a non-caching function which means it doesn't use
			// the primary (memory) cache, but it will use this one.
			try
			{
				foreach (var show in Tmdb.GetShowChanges(DateTime.UtcNow, maxPage: 2))
					Console.WriteLine(show.Name);
			}
			catch (JsonRpcException rex)
			{
				Console.Error.WriteLine("Error: " + rex.Json.ToString());
			}
			Console.WriteLine("Press any key...");
			Console.Read();
			Console.Clear();
			Console.WriteLine("Hopefully cached and faster (also no request limit delay if cache mode is aggressive)");
			// do it again, this time with some cache hits.
			try
			{
				foreach (var show in Tmdb.GetShowChanges(DateTime.UtcNow, maxPage: 2))
					Console.WriteLine(show.Name);
			}
			catch (JsonRpcException rex)
			{
				Console.Error.WriteLine("Error: " + rex.Json.ToString());
			}
		}
		static void _RunCacheDemo()
		{
			// Normally, you don't really have to mess with the cache in a desktop app
			// or a console app. Long running apps and services will require some 
			// maintenance. Otherwise caching is automatic.

			// Fetch a show.
			var show = new TmdbShow(2919); // get "Burn Notice"
			var movie = new TmdbMovie(219); // fetch "Volver"
											// right now our entries only have an id. use some properties
											// so we can get the show into the cache.
			Console.WriteLine("Fetched {0} from tmdb", show.Name);
			Console.WriteLine("Fetched {0} from tmdb", movie.Name);

			// write the cache to a file in the project directory
			//Tmdb.SaveCacheTo(@"..\..\cache.json");
			// we won't use the above helper method since we want our cache to
			// be pretty printed
			JsonObject.SaveTo(Tmdb.Json, @"..\..\cache.json", "    ");


			// Note that the cache is thread static. It's per thread instance.
			// Do not pass objects between threads. Serialize if you must like above
			// or in memory.

			// clear our cache.
			Tmdb.Json.Clear();

			// any existing instances will be orphaned. this impacts equality as well.

			// if you MUST re-cache an orphaned object, what you can do is this:
			movie = new TmdbMovie(movie.Json);
			// this will copy the old json data into a new movie instance which will then root 
			// that in the current cache.

			// should only have the movie we just re-rooted above in it.
			Console.WriteLine(Tmdb.Json);

			// that's cool but how about we load our cache that we saved?
		
			Tmdb.LoadCacheFrom(@"..\..\cache.json");
			// Note that this MERGES the loaded data with the existing data.
			// You can call it multiple times in a row with different files
			// if you ever wanted to. This might be useful down the road 
			// for cache differentials in a distributed system. There's also
			// LoadCacheFromUrl() with an eye toward distribution down the 
			// road

			// remember if we create new TmdbEntry instances like movie or 
			// TV objects, they will use the existing cache rather than 
			// fetching from TMDB.

			// So, since we loaded the cache we should now be able to do 
			show = new TmdbShow(3050);
			Console.WriteLine("This is cached: {0}", show.Name);
			// without making any remote requests.

			// use value equality semantics.
			// get the show again, this time through a search.
			// searches themselves are not cached, but each individual result
			// is. That means The Practice will get filled in with any additional 
			// data returned from the search query. 
			var showComparand = Tmdb.SearchShows("The Practice", minPage: 0, maxPage: 0)[0];
		
			// check to make sure showComparand and show are actually two different TmdbShow wrapper instances
			// should report that they are different
			Console.WriteLine("Instances are {0} actual objects", ReferenceEquals(show, showComparand) ? "the same" : "different");

			// check to make sure showComparand and show are actually logically equal.
			// should report that they are the equal
			Console.WriteLine("Instances are logically {0}", show == showComparand ? "equal" : "not equal");


			// NOTES:
			// Clearing the cache "unroots" all existing instances of TmdbEntry
			// derived classes. They will no longer operate on anything you have a 
			// root for. In fact, they are still writing to parts of the old cache. 
			// The moral is don't hang on to object instances for longer than you need
			// them. Don't store them as class members or in collections just because.
			// Instead, use them and throw them away. Get a new object when you need
			// it. This will ensure that the most current cache is the one being used.

			// our show and movie instances are still valid, but are not writing or
			// reading from the current cache.

			// This also impacts equality. Objects from the old cache cannot be compared
			// to objects from the new cache. They will always return false.

			// Because of the above, it's best to release all instances of the old objects
			// when you clear the cache. The system does not track which object instances
			// are expired objects. You can check if an object is expired manually, but it's
			// expensive.

			// ideally, a batch of operations is performed, then the cache can be cleared, then
			// the next batch (with all new objects) is performed, if you clear the cache at all.
			// a server app or a long running desktop app should clear it periodically, but doing
			// so will make a bunch of HTTP requests happen again, even if they already happened 
			// since they weren't cached anymore. 
			// The HTTP requests might be cached through the second level caching mechanism.
			// 
			// if you like, you can save and load the cache from disk, that way you can keep the cache
			// across application runs. Eventually it will get big and stale though. In the future I
			// might monitor the Tmdb.Configuration.ChangeKeys to expire old objects, if i can figure
			// out how it works exactly.


		}
		static void _RunMovieDemo(int movieId = -1)
		{
			// fetch a movie - this just grabs the top result. By default most 
			// results from the API are sorted by popularity, most popular first.
			var movie = (0> movieId) ? Tmdb.SearchMovies("Star Wars", minPage: 0, maxPage: 0)[0] : new TmdbMovie(movieId);
			//Console.WriteLine(movie.Json); // write the json (may not be complete)

			// A note about paged functions, like the SearchMovies() function above:
			// Be careful not specifying pages. Some searches can return a lot of results.
			// if you want to limit the pages, specify BOTH parameters. Each one is 0 based. 
			// The first page is 0. If they are unspecified, all pages are returned. This 
			// will take several HTTP requests in most cases. Some of the paged functions
			// *require* the minPage and maxPage parameters. They'll still accept zero and 
			// behave the same way, but typically, some of these functions may return 
			// hundreds of pages which is why the parameters are required in those cases.

			// The search itself is not cached, but the individual results will be.

			Console.WriteLine();
			Console.WriteLine(movie.Name);
			Console.WriteLine();
			Console.Write("Overview: ");
			Console.WriteLine(movie.Overview);
			Console.WriteLine();

			// write out the genres
			Console.Write("Genres:");
			foreach (var g in movie.GenresById)
				Console.Write(string.Concat(" ", g.Value));

			Console.WriteLine();

			Console.WriteLine();

			// write out the cast
			Console.Write("Cast:");
			foreach (var c in movie.Cast)
				Console.WriteLine("\t{0} as {1}", c.Name, c.Character);

			Console.WriteLine();

			// write out the crew
			Console.Write("Crew:");
			foreach (var c in from cs in movie.Crew select cs)
				Console.WriteLine("\t{1} - {0}", c.Name, c.Job);

			Console.WriteLine();

			// write out the reviews we found, if their are any.
			// this is not cached, which is why it's a paged method
			// the individual reviews are still cached
			var reviews = movie.GetReviews();
			if (0 < reviews.Length)
			{
				Console.WriteLine();
				Console.WriteLine("Reviews:");
				foreach (var r in reviews)
					Console.WriteLine("\t\"{1}\" - {0}", r.Author, r.Content);
			}

			var images = movie.Images;
			if (0 < images.Length)
			{
				Console.WriteLine();
				Console.WriteLine("Images:");
				foreach (var i in images)
				{
					Console.WriteLine("{0}: {1}", i.ImageType, Tmdb.GetImageUrl(i.Path));
				}
			}

			// Use the DiscoverXXXX() methods to find movies and TV shows based on many
			// possible parameters.

			try
			{
				// create a request token
				var requestToken = Tmdb.CreateRequestToken();

				// now authenticate the token using the supplied credentials
				Tmdb.AuthenticateRequestToken(requestToken.Key, Username, Password);

				// The other option is to send a user to a website so that *they* can log
				// in and therefore validate this token. Use 
				// Tmdb.GetAuthenticateRequestTokenUserUrl() to retrieve the url to send
				// the user to. That url is not an rpc sink - it requires a browser.

				// now our token is "special" in that until it expires it can be used to
				// create sessions. You don't *have* to dispose the session - the remote
				// end will eventually close it, but implementing dispose makes it easer 
				// to do what i'm about to do, and also it's good practice (causes a 
				// little network traffic)
				using (var session = Tmdb.CreateSession(requestToken.Key))
				{

					// You can also perform tasks under a guest session simply by
					// ommitting the sessionId from the calls that accept one. The system 
					// keeps a guest session open but only if it is used. If you really 
					// want to, you can use a guest session without authenticating simply by 
					// not passing around a sessionId to methods that take one. Be careful 
					// using guest actions. The service tracks IP and tracks usage. Don't 
					// use them for automated tasks.

					// you can use a session to rate a movie under a particular account.
					// if you don't pass a session, the current guest session is used.
					movie.Rate(5, session);

					// you can also delete your rating
					movie.ClearRating(session);

					// ending the using block or calling Close() deletes the session, 
					// basically logging the session off
					// and expiring the session id. This doesn't apply to guest sessions.
				}
			}
			catch
			{
				Console.WriteLine();
				Console.WriteLine("Couldn't log in.");
			}
			// note how the Json data associated with the object grew in response
			// to use using the object. This is to minimize network traffic.
			// Anything fetched before will be retrieved from the cache.
			//Console.WriteLine(movie.Json); // write the json again

		}
		static void _RunShowDemo(int id = 2919/*Burn Notice*/)
		{
			// fetch a show - you can use SearchShows() or if you already know the id
			// of the object you want, just create it with the given id.
			var show = new TmdbShow(id); 
			//Console.WriteLine(show.Json); // write the json - for now just an id

		
			Console.WriteLine();

			var name = show.Name;
			var overview = show.Overview;
			// watch how it grew after getting some properties
			Console.WriteLine(show.Json);
			Console.WriteLine();

			Console.WriteLine(name);
			Console.WriteLine(overview);
			Console.WriteLine();
			//
			// write out the genres
			Console.Write("Genres:");
			var gbid = show.GenresById;
			if (null != gbid) {
				foreach (var g in gbid)
					Console.Write(string.Concat(" ", g.Value));
			}
			Console.WriteLine();

			Console.WriteLine();

			// write out the cast
			Console.Write("Cast:");
			foreach (var c in show.Cast)
				Console.WriteLine("\t{0} as {1}", c.Name, c.Character);

			Console.WriteLine();

			// write out the crew.
			Console.Write("Crew:");
			foreach (var c in from cs in show.Crew select cs)
				Console.WriteLine("\t{1} - {0}", c.Name, c.Job);
			Console.WriteLine();
			// Seasons zero is Specials when the series has them
			// To account for that, we look to TotalSeasons instead
			// of Seasons.Length
			Console.WriteLine(
				"{0} Seasons and {1} Episodes in {2} years",
				show.TotalSeasons,
				show.TotalEpisodes,
				Math.Round((show.LastAirDate - show.FirstAirDate).TotalDays / 365)
				);

			// write out the reviews we found, if their are any.
			// this is not cached, which is why it's a paged method
			// the individual reviews are still cached
			var reviews = show.GetReviews();
			if (0 < reviews.Length)
			{
				Console.WriteLine();
				Console.WriteLine("Reviews:");
				foreach (var r in reviews)
					Console.WriteLine("\t\"{1}\" - {0}", r.Author, r.Content);
			}
			var images = show.Images;
			if (0 < images.Length)
			{
				Console.WriteLine();
				Console.WriteLine("Images:");
				foreach (var i in images)
				{
					Console.WriteLine("{0}: {1}", i.ImageType, Tmdb.GetImageUrl(i.Path));
				}
			}
			// note how the Json data associated with the object grew even more
			// in response to use using the object. 
			// Console.WriteLine(show.Json); // write the json again

		}
	}
}
