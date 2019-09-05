using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	public sealed class TmdbCrewMember : TmdbRole
	{
		public TmdbCrewMember(IDictionary<string,object> json) : base(json)
		{
		}
		public override TmdbRoleType RoleType => TmdbRoleType.Crew;
		public string Job => GetField<string>("job");
		public string Department => GetField<string>("department");
		

	}
	
}
