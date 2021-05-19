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

		public List<MovieRelation> MovieRelation { get; set; } = new List<MovieRelation>();
	}
}
