using Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TmdbApi
{
	partial class Tmdb
	{
		const string _getAuthenticationRequestTokenUserUrlBase = "https://www.themoviedb.org/authenticate/{0}";

		static Lazy<KeyValuePair<string, DateTime>> _guestSession = new Lazy<KeyValuePair<string, DateTime>>(() => { return _CreateGuestSession(); });
		internal static string GuestSessionId {
			get {
				if (DateTime.UtcNow >= _guestSession.Value.Value)
					_guestSession = new Lazy<KeyValuePair<string, DateTime>>(() => { return _CreateGuestSession(); });

				return _guestSession.Value.Key;
			}
		}
		public static TmdbGuestSession GuestSession
			=>new TmdbGuestSession(GuestSessionId);

		public static KeyValuePair<string, DateTime> CreateRequestToken()
		{
			var d = Tmdb.Invoke("/authentication/token/new");

			if (null != d)
			{
				object o;
				string rt = null;
				if (d.TryGetValue("request_token", out o))
					rt = o as string;
				DateTime dt = default(DateTime);
				if (d.TryGetValue("expires_at", out o))
				{
					var s = o as string;
					if (!string.IsNullOrEmpty(s))
						dt = _ParseDateTime(s);
				}
				return new KeyValuePair<string, DateTime>(rt, dt);
			}
			throw new Exception("Error in response.");
		}
		static KeyValuePair<string, DateTime> _CreateGuestSession()
		{

			var d = Tmdb.Invoke("/authentication/guest_session/new");
			if (null != d)
			{
				object o;
				string rt = null;
				if (d.TryGetValue("guest_session_id", out o))
					rt = o as string;
				DateTime dt = default(DateTime);
				if (d.TryGetValue("expires_at", out o))
				{
					var s = o as string;
					if (!string.IsNullOrEmpty(s))
						dt = _ParseDateTime(s);
				}
				return new KeyValuePair<string, DateTime>(rt, dt);
			}
			throw new Exception("Error in response.");
		}
		public static TmdbSession CreateSession(string requestToken)
		{
			var body = new JsonObject();
			body.Add("request_token", requestToken);
			var d = Invoke("/authentication/session/new",body);
			if (null != d)
			{
				string rt = null;
				object o;
				if (d.TryGetValue("session_id", out o))
					rt = o as string;
				return new TmdbSession(rt);
			}
			return null;
		}
		
		public static string GetAuthenticateRequestTokenUserUrl(string requestToken, string redirectUrl)
		{
			var url = string.Format(_getAuthenticationRequestTokenUserUrlBase, requestToken);
			if (!string.IsNullOrEmpty(redirectUrl))
				url = string.Concat(url, "?redirect_to=", Uri.EscapeUriString(redirectUrl));
			return url;
		}
		public static KeyValuePair<string, DateTime> AuthenticateRequestToken(string requestToken, string username, string password)
		{
			var body = new JsonObject();
			body.Add("username", username);
			body.Add("password", password);
			body.Add("request_token", requestToken);
			var d = Tmdb.Invoke("/authentication/token/validate_with_login", null, body);
			if (null != d)
			{
				object o;
				string rt = null;
				if (d.TryGetValue("request_token", out o))
					rt = o as string;
				DateTime dt = default(DateTime);
				if (d.TryGetValue("expires_at", out o))
				{
					var s = o as string;
					if (!string.IsNullOrEmpty(s))
						dt = _ParseDateTime(s);
				}
				return new KeyValuePair<string, DateTime>(rt, dt);
			}
			return default(KeyValuePair<string, DateTime>);
		}
		static DateTime _ParseDateTime(string dt)
		{
			// cheap
			var d = dt.Substring(0, 10);
			var t = dt.Substring(11, 8);
			var d1 = DateTime.ParseExact(d, "yyyy-MM-dd", CultureInfo.InvariantCulture);
			var ts = TimeSpan.ParseExact(t, @"h\:mm\:ss", CultureInfo.InvariantCulture);
			return d1 + ts;
		}
	}
}

