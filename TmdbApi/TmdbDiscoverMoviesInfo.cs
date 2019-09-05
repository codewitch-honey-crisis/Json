using Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	[Flags]
	public enum TmdbDiscoverMoviesSortType : byte
	{
		Default = 0,
		Descending = 0,
		Ascending = 0x80,
		//popularity.asc, 
		//popularity.desc, 
		Popularity = 0,
		//release_date.asc, 
		//release_date.desc,
		ReleaseDate = 1,
		//revenue.asc, 
		//revenue.desc, 
		Revenue = 2,
		//primary_release_date.asc,
		//primary_release_date.desc, 
		PrimaryReleaseDate = 3,
		//original_title.asc, 
		//original_title.desc, 
		OriginalTitle = 4,
		//vote_average.asc, 
		//vote_average.desc, 
		VoteAverage = 5,
		//vote_count.asc, 
		//vote_count.desc
		VoteCount = 6
	}
	public sealed class TmdbDiscoverMoviesInfo
	{
		public string Region { get; set; }
		public TmdbDiscoverMoviesSortType SortBy { get; set; }
		public string CertifiedRatingCountry { get; set; }
		public string CertifiedRating { get; set; }
		public string CertifiedRatingMaximum { get; set; }
		public string CertifiedRatingMinimum { get; set; }
		public bool IncludeAdult { get; set; }
		public bool IncludeVideo { get; set; }
		public int PrimaryReleaseYear { get; set; }
		public DateTime PrimaryReleaseDateMinimum { get; set; }
		public DateTime PrimaryReleaseDateMaximum { get; set; }
		public DateTime ReleaseDateMinimum { get; set; }
		public DateTime ReleaseDateMaximum { get; set; }
		public int ReleaseType { get; set; } = -1;
		public int Year { get; set; }
		public int VoteCountMinimum { get; set; }
		public int VoteCountMaximum { get; set; } = -1;
		public double VoteAverageMinimum { get; set; }
		public double VoteAverageMaximum { get; set; }
		// TODO: figure out what these can be
		public TmdbPerson[] CastAllOf { get; set; }
		public TmdbPerson[] CastAnyOf { get; set; }
		public int[] CastAllOfIds { get; set; }
		public int[] CastAnyOfIds { get; set; }
		public TmdbPerson[] CrewAllOf { get; set; }
		public TmdbPerson[] CrewAnyOf { get; set; }
		public string[] CrewAllOfIds { get; set; }
		public string[] CrewAnyOfIds { get; set; }
		public TmdbPerson[] PeopleAllOf { get; set; }
		public TmdbPerson[] PeopleAnyOf { get; set; }
		public int[] PeopleAllOfIds { get; set; }
		public int[] PeopleAnyOfIds { get; set; }
		public TmdbCompany[] CompaniesAllOf { get; set; }
		public TmdbCompany[] CompaniesAnyOf { get; set; }
		public int[] CompaniesAllOfIds { get; set; }
		public int[] CompaniesAnyOfIds { get; set; }
		public KeyValuePair<int, string>[] GenresAllOf { get; set; }
		public KeyValuePair<int, string>[] GenresAnyOf { get; set; }
		public int[] GenresAllOfIds { get; set; }
		public int[] GenresAnyOfIds { get; set; }
		public KeyValuePair<int, string>[] WithoutGenresAllOf { get; set; }
		public KeyValuePair<int, string>[] WithoutGenresAnyOf { get; set; }
		public int[] WithoutGenresAllOfIds { get; set; }
		public int[] WithoutGenresAnyOfIds { get; set; }
		public KeyValuePair<int, string>[] KeywordsAllOf { get; set; }
		public KeyValuePair<int, string>[] KeywordsAnyOf { get; set; }
		public int[] KeywordsAllOfIds { get; set; }
		public int[] KeywordsAnyOfIds { get; set; }
		public KeyValuePair<int, string>[] WithoutKeywordsAllOf { get; set; }
		public KeyValuePair<int, string>[] WithoutKeywordsAnyOf { get; set; }
		public int[] WithoutKeywordsAllOfIds { get; set; }
		public int[] WithoutKeywordsAnyOfIds { get; set; }
		public TimeSpan RunTimeMinimum { get; set; }
		public TimeSpan RunTimeMaximum { get; set; }
		public string OriginalLanguage { get; set; }
		public IDictionary<string, object> ToArguments()
		{
			var result = new JsonObject();
			if (TmdbDiscoverMoviesSortType.Default != SortBy)
			{
				var sfx = (byte)SortBy > 0x7F ? ".asc" : ".desc";
				var sort = (TmdbDiscoverMoviesSortType)(((byte)SortBy) & 0x7F);
				switch (sort)
				{
					case TmdbDiscoverMoviesSortType.Popularity:
						result.Add("sort_by", "popularity" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.VoteAverage:
						result.Add("sort_by", "vote_average" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.VoteCount:
						result.Add("sort_by", "vote_count" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.OriginalTitle:
						result.Add("sort_by", "original_title" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.PrimaryReleaseDate:
						result.Add("sort_by", "primary_release_date" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.ReleaseDate:
						result.Add("sort_by", "release_date" + sfx);
						break;
					case TmdbDiscoverMoviesSortType.Revenue:
						result.Add("sort_by", "revenue" + sfx);
						break;
				}
			}

			if (0d < VoteAverageMinimum)
				result.Add("vote_average.gte", VoteAverageMinimum);
			if (0d < VoteAverageMaximum)
				result.Add("vote_average.lte", VoteAverageMaximum);
			if (0d < VoteCountMinimum)
				result.Add("vote_count.gte", VoteCountMinimum);
			if (0d < VoteCountMaximum)
				result.Add("vote_count.lte", VoteCountMaximum);

			if (!string.IsNullOrEmpty(CertifiedRatingCountry))
				result.Add("certification_country", CertifiedRatingCountry);

			if (!string.IsNullOrEmpty(CertifiedRating))
				result.Add("certification",CertifiedRating);

			if (!string.IsNullOrEmpty(CertifiedRatingMaximum))
				result.Add("certification.lte", CertifiedRatingMaximum);

			if (!string.IsNullOrEmpty(CertifiedRatingMinimum))
				result.Add("certification.gte", CertifiedRatingMinimum);

			if (false != IncludeAdult)
				result.Add("include_adult",true);

			if (false != IncludeVideo)
				result.Add("include_video",true);

			if (0 < PrimaryReleaseYear)
				result.Add("primary_release_year", PrimaryReleaseYear);
				
			if (default(DateTime) != PrimaryReleaseDateMaximum)
				result.Add("primary_release_date.lte", PrimaryReleaseDateMaximum.ToString("yyyy-MM-dd"));

			if (default(DateTime) != PrimaryReleaseDateMinimum)
				result.Add("primary_release_date.gte", PrimaryReleaseDateMinimum.ToString("yyyy-MM-dd"));

			if (default(DateTime) != ReleaseDateMaximum)
				result.Add("release_date.lte", ReleaseDateMaximum.ToString("yyyy-MM-dd"));

			if (default(DateTime) != ReleaseDateMinimum)
				result.Add("release_date.gte", ReleaseDateMinimum.ToString("yyyy-MM-dd"));

			if (-1 != ReleaseType)
				result.Add("with_release_type", ReleaseType);

			if (0 != Year)
				result.Add("year", Year);
			var sb = new StringBuilder();
			if (null != CastAllOf || null != CastAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CastAllOf, CastAllOfIds, true);
				result.Add("with_cast", sb.ToString());
			}
			else if (null != CastAnyOf || null != CastAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CastAnyOf, CastAnyOfIds, false);
				result.Add("with_cast", sb.ToString());
			}
			if (null != CrewAllOf || null != CrewAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CrewAllOf, CrewAllOfIds, true);
				result.Add("with_crew", sb.ToString());
			}
			else if (null != CrewAnyOf || null != CrewAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CrewAnyOf, CrewAnyOfIds, false);
				result.Add("with_crew", sb.ToString());
			}
			if (null != PeopleAllOf || null != PeopleAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, PeopleAllOf, PeopleAllOfIds, true);
				result.Add("with_people", sb.ToString());
			}
			else if (null != PeopleAnyOf || null != PeopleAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, PeopleAnyOf, PeopleAnyOfIds, false);
				result.Add("with_people", sb.ToString());
			}
			if (null != CompaniesAllOf || null != CompaniesAllOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CompaniesAllOf, CompaniesAllOfIds, true);
				result.Add("with_companies", sb.ToString());
			}
			else if (null != CompaniesAnyOf || null != CompaniesAnyOfIds)
			{
				sb.Clear();
				TmdbDiscoverUtility.AppendWith(sb, CompaniesAnyOf, CompaniesAnyOfIds, false);
				result.Add("with_companies", sb.ToString());
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
