using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	public sealed class TmdbCastMember : TmdbRole
	{
		public TmdbCastMember(IDictionary<string, object> json) : base(json)
		{
		}
		public override TmdbRoleType RoleType => TmdbRoleType.Cast;
		public string Character => GetField<string>("character");
		public int Order => GetField("order",0);

		
	}

}
