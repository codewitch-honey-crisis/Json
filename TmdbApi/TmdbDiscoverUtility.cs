using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	static class TmdbDiscoverUtility
	{
		public static void AppendWith(StringBuilder sb, TmdbCachedEntityWithId[] entries, int[] ids, bool all)
		{
			if (null != entries || null != ids)
			{
				var len = 0;
				if (null != entries)
					len += entries.Length;
				if (null != ids)
					len += ids.Length;
				if (0 != len)
				{
					var arr = new int[len];
					var si = 0;
					if (null != ids)
					{
						ids.CopyTo(arr, 0);
						si = ids.Length;
					}
					if (null != entries)
						for (var i = 0; i < entries.Length; i++)
							arr[i + si] = entries[i].Id;
					AppendWith(sb, arr, all);
				}
			}
		}
		public static void AppendWith(StringBuilder sb, TmdbEntity[] entries, string[] ids, bool all)
		{
			if (null != entries || null != ids)
			{
				var len = 0;
				if (null != entries)
					len += entries.Length;
				if (null != ids)
					len += ids.Length;
				if (0 != len)
				{
					var arr = new string[len];
					var si = 0;
					if (null != ids)
					{
						ids.CopyTo(arr, 0);
						si = ids.Length;
					}
					if (null != entries)
						for (var i = 0; i < entries.Length; i++)
						{
							var ce = entries[i] as TmdbCachedEntity;
							if (null != ce)
							{
								var pi = ce.PathIdentity;
								if (null != pi) // sanity check
								{
									arr[i + si] = pi[pi.Length - 1];
								}
							}
						}
					AppendWith(sb, arr, all);
				}
			}
		}
		public static void AppendWith(StringBuilder sb, KeyValuePair<int, string>[] entries, int[] ids, bool all)
		{
			if (null != entries || null != ids)
			{
				var len = 0;
				if (null != entries)
					len += entries.Length;
				if (null != ids)
					len += ids.Length;
				if (0 != len)
				{
					var arr = new int[len];
					var si = 0;
					if (null != ids)
					{
						ids.CopyTo(arr, 0);
						si = ids.Length;
					}
					if (null != entries)
						for (var i = 0; i < entries.Length; i++)
							arr[i + si] = entries[i].Key;
					AppendWith(sb, arr, all);
				}
			}
		}
		public static void AppendWith(StringBuilder sb, string[] entries, int[] ids, bool all)
		{
			if (null != entries || null != ids)
			{
				var len = 0;
				if (null != entries)
					len += entries.Length;
				if (null != ids)
					len += ids.Length;
				if (0 != len)
				{
					var arr = new string[len];
					var si = 0;
					if (null != ids)
					{
						for (var i = 0; i < ids.Length; i++)
							arr[i] = ids[i].ToString();
						si = ids.Length;
					}
					if (null != entries)
						for (var i = 0; i < entries.Length; i++)
							arr[i + si] = entries[i];
					AppendWith(sb, arr, all);
				}
			}
		}
		public static void AppendWith(StringBuilder sb, string[] data, bool and)
		{
			for (var i = 0; i < data.Length; i++)
			{
				if (0 != i)
					sb.Append(and ? "," : "|");
				sb.Append(data[i]);
			}
		}
		public static void AppendWith(StringBuilder sb, int[] data, bool and)
		{
			for (var i = 0; i < data.Length; i++)
			{
				if (0 != i)
					sb.Append(and ? "," : "|");
				sb.Append(data[i].ToString());
			}
		}
	}
}
