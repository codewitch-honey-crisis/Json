using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public sealed class TmdbSession : TmdbCachedEntityWithId2, IDisposable
	{
		bool _isDisposed = false;
		public TmdbSession(string id) : base(id)
		{
			// not cached
		}
		public TmdbSession(IDictionary<string,object> json) : base(json)
		{
			// not cached
		}
		public override string[] PathIdentity => null;
		public TmdbMovieList CreateMovieList(string name = null, string description = null, string language = null)
		{
			_CheckDisposed();
			var args = new JsonObject();
			args.Add("session_id", Id);
			var payload = new JsonObject();
			if (null != name)
				payload.Add("name", name);
			if (null != description)
				payload.Add("description", name);
			if (null != language)
				payload.Add("language", name);

			var d = Tmdb.Invoke(string.Concat("/list/", Id), args, payload);
			if (null != d)
			{
				object o;
				if (d.TryGetValue("list_id", out o))
				{
					var id = o as string;
					if (!string.IsNullOrEmpty(id))
					{
						// create the object to initialize the movie list with
						var j = new JsonObject();
						j.Add("id", id);
						return new TmdbMovieList(j);
					}
				}
			}
			return null;
		}
		public bool DeleteMovieList(TmdbMovieList list)
		{
			_CheckDisposed();
			if (null == list) throw new ArgumentNullException(nameof(list));
			var args = new JsonObject();
			args.Add("session_id", Id);
			var d = Tmdb.Invoke(string.Concat("/list/", Id), args,httpMethod:"DELETE");
			if (null != d)
			{
				object o;
				if (d.TryGetValue("status_code", out o) && o is int && 13 == (int)o)
					return true;
			}
			return false;
		}
		public TmdbAccount Account {
			get {
				_CheckDisposed();
				var args = new JsonObject();
				args.Add("session_id", Id);
				var d = Tmdb.Invoke("/account", args);
				if (null != d)
					return new TmdbAccount(d);
				return null;
			}
		}
		public void Close()
		{
			if (!_isDisposed)
			{
				var payload = new JsonObject();
				payload.Add("session_id", Id);
				Tmdb.Invoke("/authentication/session", null, payload, httpMethod: "DELETE");
			}
			_isDisposed = true;
		}
		void IDisposable.Dispose()
		{
			Close();
		}
		void _CheckDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(typeof(TmdbSession).Name);
		}
	}
}
