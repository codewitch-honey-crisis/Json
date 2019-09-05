using System;
using System.Collections.Generic;
using System.Text;

namespace TmdbApi
{
	public enum TmdbImageType
	{
		Unknown = 0,
		Backdrop,
		Poster,
		Logo
	}
	public class TmdbImage : TmdbEntity
	{
		public TmdbImage(IDictionary<string,object> json) : base(json)
		{
		}
		
		public int Width {
			get {
				return GetField("width",0);
			}
		}
		public int Height {
			get {
				return GetField("height", 0);
			}
		}
		public double AspectRatio {
			get {
				return GetField("aspect_ratio", 0);
			}
		}
		public string Path {
			get {
				return GetField<string>("file_path");
			}
		}
		public string Language {
			get {
				return GetField<string>("iso_639_1");
			}
		}
		public double VoteAverage {
			get {
				return GetField("vote_average",0d);
			}
		}
		public int VoteCount {
			get {
				return GetField("vote_count", 0);
			}
		}
		public TmdbImageType ImageType {
			get {
				switch(GetField<string>("image_type", null))
				{
					case "poster":
						return TmdbImageType.Poster;
					case "backdrop":
						return TmdbImageType.Backdrop;
					case "logo":
						return TmdbImageType.Logo;
				}
				return TmdbImageType.Unknown;
			}
		}
		public string FileType => GetField<string>("file_type");
	}
}
