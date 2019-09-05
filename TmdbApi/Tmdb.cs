using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Json;
namespace TmdbApi
{
	public static partial class Tmdb
	{
		const string _apiUrlBase = "https://api.themoviedb.org/3";
		static ThreadLocal<IDictionary<string, object>> _json=new ThreadLocal<IDictionary<string, object>>(()=>new JsonObject());
		public static IDictionary<string,object> Json { get { return _json.Value; } }
		public static string ApiKey { get; set; }
		public static string Language { get; set; }
		// by my tests this wait is just about right
		/// <summary>
		/// Sets the delay, in milliseconds for request throttling when the server returns a "too many requests" error.
		/// </summary>
		/// <remarks>TMDb has an upper limit on the number of requests allowed per a given period. In case of going over that, the server returns a specific error. When we receive the error, we wait, and then retry. This is how long we wait. It's appropriate for their settings as of September 2019</remarks>
		public static int RequestThrottleDelay { get; set; } = 6000;
		public static TmdbConfiguration Configuration { get { return new TmdbConfiguration(); } }
		public static string GetImageUrl(string path, string size = "original", bool secure = true)
		{
			if (null == path) throw new ArgumentNullException(nameof(path));
			if (0 == path.Length) throw new ArgumentException("The path is empty.", nameof(path));
			if (string.IsNullOrEmpty(size))
				size = "original";
			if (!secure)
				return string.Concat(Configuration.Images.BaseUrl, size, path);
			return string.Concat(Configuration.Images.SecureBaseUrl, size, path);
		}

		public static IDictionary<string,object> Invoke(string path, IDictionary<string, object> args=null, IDictionary<string, object> payload=null, Func<object, object> fixupResult = null, Func<object, object> fixupError = null, string httpMethod = null)
			=> (IDictionary<string, object>)_Invoke(path, false, args, payload, fixupResult, fixupError, httpMethod);

		public static IDictionary<string,object> InvokeLang(string path, IDictionary<string, object> args=null, IDictionary<string, object> payload=null, Func<object, object> fixupResult = null, Func<object, object> fixupError = null, string httpMethod = null,JsonRpcCacheLevel cacheLevel = JsonRpcCacheLevel.Conservative)
			=> (IDictionary<string,object>)_Invoke(path, true, args, payload, fixupResult, fixupError, httpMethod);
		public static object InvokeEx(string path, IDictionary<string, object> args = null, IDictionary<string, object> payload = null, Func<object, object> fixupResult = null, Func<object, object> fixupError = null, string httpMethod = null)
			=> _Invoke(path, false, args, payload, fixupResult, fixupError, httpMethod);

		public static object InvokeLangEx(string path, IDictionary<string, object> args = null, IDictionary<string, object> payload = null, Func<object, object> fixupResult = null, Func<object, object> fixupError = null, string httpMethod = null, JsonRpcCacheLevel cacheLevel = JsonRpcCacheLevel.Conservative)
			=> _Invoke(path, true, args, payload, fixupResult, fixupError, httpMethod);

		public static IDictionary<string, object> InvokePaged(string path, int minPage = 0, int maxPage = 999,IDictionary<string, object> args = null, Func<object, object> fixupResultItem = null, Func<object, object> fixupError = null)
			=> _InvokePaged(path, false,minPage,maxPage, args, fixupResultItem, fixupError);

		public static IDictionary<string, object> InvokePagedLang(string path, int minPage = 0, int maxPage = 999,IDictionary<string, object> args = null, Func<object, object> fixupResultItem = null, Func<object, object> fixupError = null)
			=> _InvokePaged(path, true, minPage, maxPage, args, fixupResultItem, fixupError);

		public static IDictionary<string, object> InvokeFlatPaged(string path, int minPage = 0, int maxPage = 999, IDictionary<string, object> args = null, Func<object, object> fixupResultItem = null, Func<object, object> fixupError = null)
			=> _InvokeFlatPaged(path, false, minPage, maxPage, args, fixupResultItem, fixupError);

		public static IDictionary<string, object> InvokeFlatPagedLang(string path, int minPage = 0, int maxPage = 999, IDictionary<string, object> args = null, Func<object, object> fixupResultItem = null, Func<object, object> fixupError = null)
			=> _InvokeFlatPaged(path, true, minPage, maxPage, args, fixupResultItem, fixupError);

		static object _Invoke(string path, bool sendLang, IDictionary<string, object> args, IDictionary<string, object> payload, Func<object, object> fixupResult, Func<object, object> fixupError, string httpMethod)
		{
			var url = _apiUrlBase;
			if (!path.StartsWith("/"))
				url = string.Concat(url, "/");
			url = string.Concat(url, path);
			if (null == args)
				args = new JsonObject();
			args["api_key"] = ApiKey;
			if (sendLang && !string.IsNullOrEmpty(Language))
				args["language"] = Language;
			object result = null;
			while (null == result)
			{
				try
				{
					result = JsonRpc.Invoke(url, args, payload, null, httpMethod, fixupResult, fixupError, Tmdb.CacheLevel);
					if (null == result)
						break;
				}
				catch (JsonRpcException rex)
				{
					// are we over the request limit?
					if (25 == rex.ErrorCode)
					{
						// wait and try again
						Thread.Sleep(RequestThrottleDelay);
					}
					else if (-39 == rex.ErrorCode)
						break;
					else
						throw;
				}
			}
			//if (null == result)
			//	throw new Exception("Error in response.");
			return result;
		}
		static IDictionary<string, object> _InvokePaged(string path, bool sendLang, int minPage, int maxPage, IDictionary<string, object> args, Func<object, object> fixupResultItem, Func<object, object> fixupError)
		{
			var result = new JsonObject();
			var hasResults = false;
			if (0 > minPage)
				throw new ArgumentOutOfRangeException(nameof(minPage));
			if (0 > maxPage)
				throw new ArgumentOutOfRangeException(nameof(maxPage));
			if (maxPage < minPage)
				throw new ArgumentException("The parameter " + nameof(minPage) + "must be less than or equal to " + nameof(maxPage), nameof(minPage));
			if (null == args)
				args = new JsonObject();
			var args2 = new JsonObject();
			JsonObject.CopyTo(args, args2);

			for (var i = minPage; 1000 > i; ++i)
			{
				// the server expects the pages to be one based
				if(0!=i)
					args2["page"] = i + 1;
				var d = Tmdb._Invoke(path, sendLang, args2, null, null, fixupError, null) as IDictionary<string,object>;
				if (null != d)
				{
					object o;
					if (minPage == i)
					{
						if (0 > maxPage)
						{
							if (d.TryGetValue("total_pages", out o) && o is int)
								maxPage = ((int)o) - 1;
							else
								maxPage = minPage;
						}
						else
						{
							if (d.TryGetValue("total_pages", out o) && o is int)
							{
								var p = ((int)o) - 1;
								if (-1 != p)
								{
									if (maxPage > p)
										maxPage = p;
								}
							}
						}
					}
					if (d.TryGetValue("results", out o))
					{
						hasResults = true;
						if (null != fixupResultItem)
							o = fixupResultItem(o);
						result.Add((i+1).ToString(), o);
					}
				}
				if (maxPage <= i)
					break;

			}
			if (hasResults)
				return result;
			return null;
		}
		// some page routines are not in the paged format
		static IDictionary<string, object> _InvokeFlatPaged(string path, bool sendLang, int minPage, int maxPage, IDictionary<string, object> args, Func<object, object> fixupResultItem, Func<object, object> fixupError)
		{
			var result = new JsonObject();
			var hasResults = false;
			if (0 > minPage)
				throw new ArgumentOutOfRangeException(nameof(minPage));
			if (0 > maxPage)
				throw new ArgumentOutOfRangeException(nameof(maxPage));
			if (maxPage < minPage)
				throw new ArgumentException("The parameter " + nameof(minPage) + "must be less than or equal to " + nameof(maxPage), nameof(minPage));
			if (null == args)
				args = new JsonObject();
			var args2 = new JsonObject();
			JsonObject.CopyTo(args, args2);

			for (var i = minPage; 1000 > i; ++i)
			{
				var thisHasResults = false;
				// the server expects the pages to be one based
				if (0 != i)
					args2["page"] = i + 1;
				var d = Tmdb._Invoke(path, sendLang, args2, null, null, fixupError, null) as IDictionary<string, object>;
				if (null != d)
				{
					object o;
					// we don't know the name of the field
					// so fetch the first field that's a list
					foreach(var field in d)
					{
						var ll = field.Value as IList<object>;
						if(null!=ll)
						{
							o = ll;
							hasResults = true;
							thisHasResults = true;
							if (null != fixupResultItem)
								o = fixupResultItem(o);
							result.Add(i.ToString(), o);
							break;
						}
					}
				}
				if (maxPage <= i || !thisHasResults)
					break;

			}
			if (hasResults)
				return result;
			return null;
		}
		internal static IList<object> CollapsePagedJson(IDictionary<string, object> paged)
		{
			if (null == paged)
				return null;
			var list = new List<KeyValuePair<string, object>>(paged);
			list.Sort((x, y) => x.Key.CompareTo(y.Key));
			var result = new JsonArray();
			for (int ic = list.Count, i = 0; i < ic; ++i)
			{
				var l = list[i].Value as IList<object>;
				if (null != l)
					result.AddRange(l);
			}
			return result;
		}
		internal static T GetField<T>(string name, T @default = default(T))
		{
			object o;
			if (Json.TryGetValue(name, out o) && o is T)
				return (T)o;
			return @default;
		}
		internal static DateTime DateToDateTime(string date)
		{
			if (!string.IsNullOrEmpty(date))
				return DateTime.ParseExact(date, "yyyy-MM-dd",
					CultureInfo.InvariantCulture);
			return default(DateTime);
		}
		internal static string DateTimeToDate(DateTime date)
		{
			if (default(DateTime) == date)
				return null;
			return date.ToString("yyyy-MM-dd");
			
		}
		internal static KeyValuePair<TKey, TValue>[] ToKvpArray<TKey, TValue>(IList<object> jsonArray, string keyField, string valueField)
		{
			if (null != jsonArray)
			{
				var result = new KeyValuePair<TKey, TValue>[jsonArray.Count];
				for (var i = 0; i < result.Length; i++)
				{
					var d = jsonArray[i] as IDictionary<string, object>;
					if (null != d)
					{
						TKey key = default(TKey);
						TValue value = default(TValue);
						object o;
						if (d.TryGetValue(keyField, out o) && o is TKey)
							key = (TKey)o;
						if (d.TryGetValue(valueField, out o) && o is TValue)
							value = (TValue)o;
						result[i] = new KeyValuePair<TKey, TValue>(key, value);
					}
				}
				return result;
			}
			return null;
		}
	}
}