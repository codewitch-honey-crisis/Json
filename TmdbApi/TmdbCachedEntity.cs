using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public abstract class TmdbCachedEntity : TmdbEntity
	{
		protected TmdbCachedEntity(IDictionary<string, object> json) : base(json)
		{
		}
		public abstract string[] PathIdentity { get; }
		protected virtual void Fetch()
		{
			// in case you forget to override and the
			// API doesn't accept a language argument 
			// all this does is send an extra parameter
			FetchJsonLang();
		}
		protected void FetchJson(string path = null, Func<object, object> fixupResponse = null, Func<object, object> fixupError = null)
		{
			try
			{
				var json = Tmdb.Invoke(path ?? string.Join("/", PathIdentity), null, null, fixupResponse, fixupError);
				JsonObject.CopyTo(json, Json);
			}
			catch (JsonRpcException rex)
			{
				if (rex.ErrorCode == -39)
				{
					return;
				}
				throw;
			}
		}
		protected void FetchJsonLang(string path = null, Func<object, object> fixupResponse = null, Func<object, object> fixupError = null)
		{
			var json = Tmdb.InvokeLang(path ?? string.Join("/", PathIdentity), null, null, fixupResponse, fixupError);
			JsonObject.CopyTo(json, Json);
		}
		protected T GetCachedField<T>(string name, T @default = default(T))
		{
			object o;
			if (Json.TryGetValue(name, out o) && o is T)
				return (T)o;
			Fetch();
			if (Json.TryGetValue(name, out o) && o is T)
				return (T)o;
			return @default;
		}
		/// <summary>
		/// Call this method in your entity's constructor to root it in the in memory cache. This is important.
		/// </summary>
		protected void InitializeCache()
		{
			var path = PathIdentity;
			if (null != path)
			{
				var json = JsonObject.CreatePath(Tmdb.Json, path);
				JsonObject.CopyTo(Json, json);
				Json = json;
			}
		}
	}
	public abstract class TmdbCachedEntityWithId : TmdbCachedEntity
	{
		protected TmdbCachedEntityWithId(IDictionary<string,object> json) : base(json)
		{
		}
		public TmdbCachedEntityWithId(int id) : this(_CreateJsonFromId(id)) { }
		public virtual int Id {
			get {
				return GetField("id", -1);
			}
		}
		
		private static IDictionary<string,object> _CreateJsonFromId(int id)
		{
			var result = new JsonObject();
			result.Add("id", id);
			return result;
		}
		
	}
	public abstract class TmdbCachedEntityWithId2 : TmdbCachedEntity
	{
		public TmdbCachedEntityWithId2(IDictionary<string, object> json) : base(json)
		{

		}
		public TmdbCachedEntityWithId2(string id) : this(_CreateJsonFromId(id)) { }
		public virtual string Id {
			get {
				return GetField<string>("id");
			}
		}
		private static IDictionary<string, object> _CreateJsonFromId(string id)
		{
			var result = new JsonObject();
			result.Add("id", id);
			return result;
		}
		protected override void Fetch()
		{
			// all of our primaries have the language parameter
			FetchJsonLang();
		}
	}
}
