using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TmdbApi;

namespace TmdbBrowser
{
	public partial class SearchShows : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			search = Request.Params["q"];
			if(!string.IsNullOrEmpty(search))
			{
				results = Tmdb.SearchShows(search, 0, 4);
			} else
			{
				results = Tmdb.DiscoverShows(new TmdbDiscoverShowsInfo(), 0, 0);
			}
		}
		public string search = "";
		public TmdbShow[] results = null;
		public string HtmlEncodeElem(string str)
		{
			if (null == str) str = "";
			return str.Replace("\r", "").Replace("\n", "<br />\n").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
		}
		public string Denull(string str, string @default = null)
		{
			if (null == str) return @default == null ? "" : @default;
			return str;
		}
	}
}