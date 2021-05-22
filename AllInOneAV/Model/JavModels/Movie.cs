using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JavModels
{
	[Table(Name = "J_Movie")]
	public class Movie
    {
		[Column(Name = "idMovie", IsPrimaryKey = true, IsDbGenerated = true)]
		public int IdMovie { get; set; }
		[Column]
		public string Number { get; set; }
		[Column]
		public string Title { get; set; }
		[Column]
		public string Company { get; set; }
		[Column]
		public string Director { get; set; }
		[Column]
		public string Publisher { get; set; }
		[Column]
		public string Category { get; set; }
		[Column]
		public string Star { get; set; }
		[Column]
		public DateTime? DtRelease { get; set; }
		[Column]
		public int? Duration { get; set; }
		[Column]
		public DateTime DtUpdate { get; set; }
		[Column]
		public string PictureUrl { get; set; }
		[Column]
		public string Url { get; set; }
		[Column]
		public int IdStatus { get; set; }

		public List<MovieRelation> MovieRelation { get; set; } = new List<MovieRelation>();

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}

		public void CopyValue(Movie movie)
		{
			this.Number = movie.Number;
			this.Title = movie.Title;
			this.Company = movie.Company;
			this.Director = movie.Director;
			this.Publisher = movie.Publisher;
			this.Category = movie.Category;
			this.Star = movie.Star;
			this.DtRelease = movie.DtRelease;
			this.Duration = movie.Duration;
			this.DtUpdate = movie.DtUpdate;
			this.PictureUrl = movie.PictureUrl;
			this.Url = movie.Url;
			this.IdStatus = movie.IdStatus;
			this.MovieRelation = movie.MovieRelation;
		}
	}

	public enum MovieStatus
	{
		NotScanned = 0,
		Scanned = 1,
		Downloading = 2,
		Downloaded = 3,
		InError = 4
	}
}
