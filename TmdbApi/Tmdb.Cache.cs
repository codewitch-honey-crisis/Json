using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Json;
namespace TmdbApi
{
	// the management functions for the in-memory cache, and our 
	// secondary url based cache
	partial class Tmdb
	{
		public static JsonRpcCacheLevel CacheLevel { get; set; }
		public static void LoadCacheFrom(string filename)
		{
			CombineCache(JsonObject.LoadFrom(filename));
		}
		public static void LoadCacheFromUrl(string url)
		{
			CombineCache(JsonObject.LoadFromUrl(url));
		}
		public static void ReadCacheFrom(TextReader reader)
		{
			CombineCache(JsonObject.ReadFrom(reader));
		}
		public static void WriteCacheTo(TextWriter writer)
		{
			JsonObject.WriteTo(Json, writer);
		}
		public static void SaveCacheTo(string filename)
		{
			JsonObject.SaveTo(Json, filename);
		}
		public static void ClearCache() { Json.Clear(); }

		public static void CombineCache(IDictionary<string, object> otherCache)
		{
			JsonObject.CopyTo(otherCache, Json);
		}
	}
}
