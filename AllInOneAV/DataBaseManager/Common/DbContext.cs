using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.JavModels;


namespace DataBaseManager.Common
{
	internal class DbContext : DataContext
	{
		public DbContext(string conStr)
			: base(conStr)
		{ }

		public Table<Movie> Movie;
		public Table<Category> Category;
		public Table<Company> Company;
		public Table<Publisher> Publisher;
		public Table<Star> Star;
		public Table<Director> Director;
		public Table<MovieRelation> MovieRelation;
		public Table<MovieMagnet> MovieMagnet;
	}
}
