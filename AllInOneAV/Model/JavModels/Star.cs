using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace Model.JavModels
{
	[Table(Name = "J_Star")]
	public class Star
    {
		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int idStar { get; set; }
		[Column]
		public string Name { get; set; }
		[Column]
		public string Url { get; set; }
		[Column]
		public DateTime DtUpdate { get; set; }
    }
}
