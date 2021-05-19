using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace Model.JavModels
{
	[Table(Name = "J_MovieRelation")]
	public class MovieRelation
	{
		[Column(IsPrimaryKey = true)]
		public int IdMovie { get; set; }

		[Column(IsPrimaryKey = true)]
		public int IdTyRole { get; set; }

		[Column(IsPrimaryKey = true)]
		public int IdRelation { get; set; }
    }

	public enum RoleType
	{
		Category = 1,
		Company,
		Director,
		Publisher,
		Star
	}
}
