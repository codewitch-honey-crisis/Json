using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public class TmdbCertification : TmdbEntity
	{
		public TmdbCertification(IDictionary<string,object> json) : base(json)
		{
		}
		public string Rating {
			get {
				return GetField<string>("certification", null);
			}
		}
		public string Description {
			get {
				return GetField<string>("meaning", null);
			}
		}
	}
}
