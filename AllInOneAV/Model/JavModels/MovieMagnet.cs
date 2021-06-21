using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JavModels
{
	[Table(Name = "J_MovieMagnet")]
	public class MovieMagnet
	{
		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int IdMovieMagnet { get; set; }
		[Column]
		public int IdMovie { get; set; }
		[Column]
		public string Number { get; set; }
		[Column]
		public string MagnetName { get; set; }
		[Column]
		public string MagnetLink { get; set; }
		[Column]
		public float Size { get; set; }
		[Column]
		public DateTime? DtMagnet { get; set; }
		[Column]
		public bool IsHD { get; set; }
		[Column]
		public bool HasSubtitle { get; set; }
		[Column]
		public DateTime? DtUpdate { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
