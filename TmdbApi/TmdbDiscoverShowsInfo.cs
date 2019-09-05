using System;
using System.Collections.Generic;
using System.Text;
using Json;
namespace TmdbApi
{
	[Flags]
	public enum TmdbDiscoverShowsSortType : byte
	{
		Default = 0,
		Descending = 0,
		Ascending = 0x80,
		//popularity.asc, 
		//popularity.desc, 
		Popularity = 0,
		//first_air_date.asc, 
		//first_air_date.desc,
		FirstAirDate = 1,
		//vote_average.asc, 
		//vote_average.desc, 
		VoteAverage = 5,
		//vote_count.asc, 
		//vote_count.desc
		VoteCount = 6
	}
	public sealed class TmdbDiscoverShowsInfo
	{
		public TmdbDiscoverShowsSortType SortBy { get; set; }
		public DateTime AirDateMinimum { get; set; }
		public DateTime AirDateMaximum { get; set; }
		public DateTime FirstAirDateMinimum { get; set; }
		public DateTime FirstAirDateMaximum { get; set; }
		public int FirstAirDateYear { get; set; }
		public string TimeZone { get; set; }
		public double VoteAverageMinimum { get; set; }
		public int VoteCountMinumum { get; set; }
		public KeyValuePair<int, string>[] GenresAllOf { get; set; }
		public KeyValuePair<int, string>[] GenresAnyOf { get; set; }
		public int[] GenresAllOfIds { get; set; }
		public int[] GenresAnyOfIds { get; set; }
		public KeyValuePair<int, string>[] WithoutGenresAllOf { get; set; }
		public KeyValuePair<int, string>[] WithoutGenresAnyOf { get; set; }
		public int[] WithoutGenresAllOfIds { get; set; }
		public int[] WithoutGenresAnyOfIds { get; set; }
		public TmdbNetwork[] NetworksAllOf { get; set; }
		public int[] NetworksAllOfIds { get; set; }
		public TmdbNetwork[] NetworksAnyOf { get; set; }
		public int[] NetworksAnyOfIds { get; set; }
		public TimeSpan RunTimeMinimum { get; set; }
		public TimeSpan RunTimeMaximum { get; set; }
		public bool IncludeEmptyFirstAirDates { get; set; }
		public string OriginalLanguage { get; set; }
		public string[] KeywordsAllOf { get; set; }
		public string[] KeywordsAnyOf { get; set; }
		public int[] KeywordsAllOfIds { get; set; }
		public int[] KeywordsAnyOfIds { get; set; }
		public string[] WithoutKeywordsAllOf { get; set; }
		public string[] WithoutKeywordsAnyOf { get; set; }
		public int[] WithoutKeywordsAllOfIds { get; set; }
		public int[] WithoutKeywordsAnyOfIds { get; set; }
		public bool IsScreenedTheatrically { get; set; }
		public TmdbCompany[] CompaniesAllOf { get; set; }
		public TmdbCompany[] CompaniesAnyOf { get; set; }
		public int[] CompaniesAllOfIds { get; set; }
		public int[] CompaniesAnyOfIds { get; set; }
		public IDictionary<string,object> ToArguments()
		{
			var result = new JsonObject();
			if (TmdbDiscoverShowsSortType.Default != SortBy)
			{
				var sfx = (byte)SortBy > 0x7F ? ".asc" : ".desc";
				var sort = (TmdbDiscoverShowsSortType)(((byte)SortBy) & 0x7F);
				switch (sort)
				{
					case TmdbDiscoverShowsSortType.Popularity:
						result.Add("sort_by", "popularity" + sfx);
						break;
					case TmdbDiscoverShowsSortType.VoteAverage:
						result.Add("sort_by", "vote_average" + sfx);
						break;
					case TmdbDiscoverShowsSortType.VoteCount:
						result.Add("sort_by", "vote_count" + sfx);
						break;
					case TmdbDiscoverShowsSortType.FirstAirDate:
						result.Add("sort_by", "first_air_date"+sfx);
						break;
				}
				
			}

			if (default(DateTime) != AirDateMaximum)
				result.Add("air_date.lte",AirDateMaximum.ToString("yyyy-MM-dd"));

			if (default(DateTime) != AirDateMinimum)
				result.Add("air_date.gte", AirDateMinimum.ToString("yyyy-MM-dd"));
			
			if (default(DateTime) != FirstAirDateMaximum)
				result.Add("first_air_date.lte", FirstAirDateMaximum.ToString("yyyy-MM-dd"));

			if (default(DateTime) != FirstAirDateMinimum)
				result.Add("first_air_date.gte", FirstAirDateMinimum.ToString("yyyy-MM-dd"));

			if(!string.IsNullOrEmpty(TimeZone))
				result.Add("timezone", TimeZone);
				
			if (0 != FirstAirDateYear)
				result.Add("first_air_date_year=", FirstAirDateYear);

			if (0d < VoteAverageMinimum)
				result.Add("vote_average.gte", VoteAverageMinimum);

			var sb = new StringBuilder();
			if (null != CompaniesAllOf || null != CompaniesAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CompaniesAllOf, CompaniesAllOfIds, true);
				result.Add("with_companies",sb.ToString());
			}
			else if (null != CompaniesAnyOf || null != CompaniesAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CompaniesAnyOf, CompaniesAnyOfIds, false);
				result.Add("with_companies",sb.ToString());
			}

			if (null != NetworksAllOf || null != NetworksAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, NetworksAllOf, NetworksAllOfIds, true);
				result.Add("with_networks", sb.ToString());
			}
			else if (null != NetworksAnyOf || null != NetworksAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, NetworksAnyOf, NetworksAnyOfIds, false);
				result.Add("with_networks", sb.ToString());
			}

			if (null != GenresAllOf || null != GenresAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, GenresAllOf, GenresAllOfIds, true);
				result.Add("with_genres", sb.ToString());
			}
			else if (null != GenresAnyOf || null != GenresAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, GenresAnyOf, GenresAnyOfIds, false);
				result.Add("with_genres", sb.ToString());
			}

			if (null != WithoutGenresAllOf || null != WithoutGenresAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, WithoutGenresAllOf, WithoutGenresAllOfIds, true);
				result.Add("without_genres", sb.ToString());
			}
			else if (null != WithoutGenresAnyOf || null != WithoutGenresAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, WithoutGenresAnyOf, WithoutGenresAnyOfIds, false);
				result.Add("without_genres", sb.ToString());
			}

			if (null != WithoutGenresAllOf || null != WithoutGenresAllOfIds)
			{
				sb.Append("&without_genres=");
				TmdbDiscoverUtility.AppendWith(sb, WithoutGenresAllOf, WithoutGenresAllOfIds, true);
			}
			else if (null != WithoutGenresAnyOf || null != WithoutGenresAnyOfIds)
			{
				sb.Append("&without_genres=");
				TmdbDiscoverUtility.AppendWith(sb, WithoutGenresAnyOf, WithoutGenresAnyOfIds, true);
			}
			if (null != KeywordsAllOf || null != KeywordsAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, KeywordsAllOf, KeywordsAllOfIds, true);
				result.Add("with_keywords", sb.ToString());
			}
			else if (null != KeywordsAnyOf || null != KeywordsAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, KeywordsAnyOf, KeywordsAnyOfIds, false);
				result.Add("with_keywords", sb.ToString());
			}

			if (null != WithoutKeywordsAllOf || null != WithoutKeywordsAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, WithoutKeywordsAllOf, WithoutKeywordsAllOfIds, true);
				result.Add("without_keywords", sb.ToString());
			}
			else if (null != WithoutKeywordsAnyOf || null != WithoutKeywordsAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, WithoutKeywordsAnyOf, WithoutKeywordsAnyOfIds, false);
				result.Add("without_keywords", sb.ToString());
			}

			if (default(TimeSpan) != RunTimeMaximum)
				result.Add("runtime.lte", RunTimeMaximum.TotalMinutes);
			
			if (default(TimeSpan) != RunTimeMinimum)
				result.Add("runtime.gte", RunTimeMinimum.TotalMinutes);

			if (!string.IsNullOrEmpty(OriginalLanguage))
				result.Add("original_language", OriginalLanguage);

			return result;
		}
	}
}
