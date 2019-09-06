using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TmdbApi;

namespace TmdbBrowser
{
    public class Global : System.Web.HttpApplication
    {
		const string ApiKey = "c83a68923b7fe1d18733e8776bba59bb";
		protected void Application_Start(object sender, EventArgs e)
        {
			Tmdb.ApiKey = ApiKey;
			// we don't care that much if the data is a little stale
			Tmdb.CacheLevel = Json.JsonRpcCacheLevel.Aggressive;
        }
    }
}