using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JavModels
{
	[Table(Name = "J_Category")]
    public class Category
    {
		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int IdCategory { get; set; }
		[Column]
		public string Name { get; set; }
		[Column]
		public string Url { get; set; }
		[Column]
		public DateTime? DtUpdate { get; set; }
    }
}
