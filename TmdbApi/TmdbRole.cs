using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbRoleType
	{
		Cast = 0,
		Crew = 1
	}
	public abstract class TmdbRole : TmdbEntity
	{
		protected TmdbRole(IDictionary<string,object> json) :base(json)
		{
		}
		public abstract TmdbRoleType RoleType { get; }
		public TmdbPerson Person {
			get {
				var id = GetField("id", -1);
				if (-1 < id)
					return new TmdbPerson(id);
				return null;
			}
		}
		public TmdbCredit Credit {
			get {
				var id = GetField<string>("credit_id");
				if (!string.IsNullOrEmpty(id))
					return new TmdbCredit(id);
				return null;
			}
		}
		public string Name => GetField<string>("name");
		public string ProfilePath => GetField<string>("profile_path");
		public TmdbGender Gender => (TmdbGender)GetField("gender", 2);
	}
}
