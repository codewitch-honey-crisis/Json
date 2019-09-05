using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	[Flags]
	public enum TmdbRatedSortType
	{
		Default = 0,
		Descending = 0,
		Ascending = 0x80,
		//created_at.asc, 
		//created_at.desc, 
		DateCreated = 0,
	}
}
