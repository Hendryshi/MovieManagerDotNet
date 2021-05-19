using DataBaseManager.Common;
using Model.JavModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utils;

namespace DataBaseManager.JavDataBaseHelper
{
    public class JavDataBaseManager : SqlHelper
	{
		public JavDataBaseManager()
		{
			
		}
		
		public void InsertAV()
		{
			Movie av1 = new Movie() { Number = "3", DtUpdate = DateTime.Now, DtRelease = DateTime.Today };
			Movie av2 = new Movie() { Number = "4", DtUpdate = DateTime.Now, DtRelease = DateTime.Today };

			List<Movie> lstAv = new List<Movie>() { av1, av2 };

			using(TransactionScope ts = new TransactionScope())
			{
				foreach(Movie a in lstAv)
				{
					db.Movie.InsertOnSubmit(a);
					db.SubmitChanges();
				}
				ts.Complete();
			}

			Console.WriteLine(av1.IdMovie);
		}

		public void InsertCategory(Category category, bool checkExist = true)
		{
			if(!checkExist || !ExistCategory(category.Name))
			{
				db.Category.InsertOnSubmit(category);
				db.SubmitChanges();
			}
		}

		public bool ExistCategory(string name)
		{
			var query = from category in db.Category
						where category.Name == name
						select category;

			return query.ToList().Count > 0;
		}

		public List<Category> LoadCategory(string name = "")
		{

			var query = from category in db.Category select category;

			if(string.IsNullOrEmpty(name))
				query.Where(c => c.Name == name);

			return query.ToList();
		}

		public void InsertCompany(Company company, bool checkExist = true)
		{
			if(!checkExist || !ExistCompany(company.Name))
			{
				db.Company.InsertOnSubmit(company);
				db.SubmitChanges();
			}
		}

		public bool ExistCompany(string name)
		{
			var query = from company in db.Company
						where company.Name == name
						select company;

			return query.ToList().Count > 0;
		}

		public void InsertDirector(Director director, bool checkExist = true)
		{
			if(!checkExist || !ExistDirector(director.Name))
			{
				db.Director.InsertOnSubmit(director);
				db.SubmitChanges();
			}
		}

		public bool ExistDirector(string name)
		{
			var query = from director in db.Director
						where director.Name == name
						select director;

			return query.ToList().Count > 0;
		}

		public void InsertPublisher(Publisher ublisher, bool checkExist = true)
		{
			if(!checkExist || !ExistPublisher(ublisher.Name))
			{
				db.Publisher.InsertOnSubmit(ublisher);
				db.SubmitChanges();
			}
		}

		public bool ExistPublisher(string name)
		{
			var query = from publisher in db.Publisher
						where publisher.Name == name
						select publisher;

			return query.ToList().Count > 0;
		}

		public void InsertStar(Star star, bool checkExist = true)
		{
			if(!checkExist || !ExistStar(star.Name))
			{
				db.Star.InsertOnSubmit(star);
				db.SubmitChanges();
			}
		}

		public bool ExistStar(string name)
		{
			var query = from star in db.Star
						where star.Name == name
						select star;

			return query.ToList().Count > 0;
		}

		public void InsertMovie(Movie movie, bool checkExist = true)
		{
			if(!checkExist || !ExistMovie(movie.Number))
			{
				using(TransactionScope ts = new TransactionScope())
				{
					movie.DtUpdate = DateTime.Now;
					db.Movie.InsertOnSubmit(movie);
					db.SubmitChanges();

					foreach(MovieRelation mr in movie.MovieRelation)
					{
						mr.IdMovie = movie.IdMovie;
						db.MovieRelation.InsertOnSubmit(mr);
						db.SubmitChanges();
					}

					ts.Complete();
				}
			}
		}

		public bool ExistMovie(string number)
		{
			var query = from movie in db.Movie
						where movie.Number == number
						select movie;

			return query.ToList().Count > 0;
		}

		
		public Company LoadCompanyByName(string name)
		{
			var query = from company in db.Company where company.Name == name select company;

			return query.Count() > 0 ? query.First() : null;
		}

		public Category LoadCategoryByName(string name)
		{
			var query = from category in db.Category where category.Name == name select category;

			return query.Count() > 0 ? query.First() : null;
		}
		public Director LoadDirectorByName(string name)
		{
			var query = from director in db.Director where director.Name == name select director;

			return query.Count() > 0 ? query.First() : null;
		}
		public Publisher LoadPublisherByName(string name)
		{
			var query = from publisher in db.Publisher where publisher.Name == name select publisher;

			return query.Count() > 0 ? query.First() : null;
		}

		public Star LoadStarByName(string name)
		{
			var query = from star in db.Star where star.Name == name select star;

			return query.Count() > 0 ? query.First() : null;
		}

		public static List<Category> GetCategories()
        {
            var sql = @"SELECT * FROM Category";

            return Query<Category>(ConnectionStrings.Jav, sql);
        }

        public static List<Star> GetActress()
        {
            var sql = @"SELECT * FROM Actress";

            return Query<Star>(ConnectionStrings.Jav, sql);
        }

        public static List<Company> GetCompany()
        {
            var sql = @"SELECT * FROM Company";

            return Query<Company>(ConnectionStrings.Jav, sql);
        }

        public static List<Publisher> GetPublisher()
        {
            var sql = @"SELECT * FROM Publisher";

            return Query<Publisher>(ConnectionStrings.Jav, sql);
        }

        public static List<Director> GetDirector()
        {
            var sql = @"SELECT * FROM Director";

            return Query<Director>(ConnectionStrings.Jav, sql);
        }


        public static int InsertScanURL(ScanURL entity)
        {
            var sql = @"INSERT INTO ScanURL (Category, URL, ID, Title, IsDownload, CreateTime) VALUES (@Category, @URL, @ID, @Title, @IsDownload, GETDATE())";

            return Execute(ConnectionStrings.Jav, sql, entity);
        }

        public static bool HasScan(ScanURL entity)
        {
            var sql = @"SELECT * FROM ScanURL WHERE Url = @URL AND IsDownload = 1";

            return Query<ScanURL>(ConnectionStrings.Jav, sql, entity).Count > 0;
        }

        public static List<ScanURL> GetScanURL()
        {
            var sql = @"SELECT * FROM ScanURL";

            return Query<ScanURL>(ConnectionStrings.Jav, sql);
        }

        public static int InsertAV(Movie av)
        {
            var sql = @"INSERT INTO AV (ID, Name, Company, Director, Publisher, Category, Actress, ReleaseDate, AvLength, CreateTime, PictureURL, URL) 
                        VALUES (@ID, @Name, @Company, @Director, @Publisher, @Category, @Actress, @ReleaseDate, @AvLength, GETDATE(), @PictureURL, @URL)";

            return Execute(ConnectionStrings.Jav, sql, av);
        }

        public static bool HasAv(string url)
        {
            var sql = @"SELECT * FROM AV WHERE Url = @url";

            return Query<Movie>(ConnectionStrings.Jav, sql, new { url }).Count > 0;
        }

        public static bool HasAv(string id, string name)
        {
            var sql = @"SELECT * FROM AV WHERE Id = @id AND Name = @name";

            return Query<Movie>(ConnectionStrings.Jav, sql, new { id, name }).Count > 0;
        }

        public static List<Movie> GetAllAV()
        {
            var sql = @"SELECT * FROM AV";

            return Query<Movie>(ConnectionStrings.Jav, sql);
        }

        public static List<Movie> GetAllAV(string id)
        {
            var sql = @"SELECT * FROM AV WHERE ID = @id";

            return Query<Movie>(ConnectionStrings.Jav, sql, new { id });
        }

        public static Movie GetAV(int avid)
        {
            var sql = @"SELECT TOP 1 * FROM AV WHERE AvID = @avid";

            return QuerySingle<Movie>(ConnectionStrings.Jav, sql, new { avid });
        }

        public static bool HasCompany(string url)
        {
            var sql = @"SELECT * FROM Company WHERE URL = @url";

            return Query<Company>(ConnectionStrings.Jav, sql, new { url }).Count > 0;
        }

        public static int InsertCompany(Company entity)
        {
            var sql = @"INSERT INTO Company (Name, URL, CreateTime) VALUES (@Name, @URL, GETDATE())";

            return Execute(ConnectionStrings.Jav, sql, entity);
        }

        public static bool HasDirector(string url)
        {
            var sql = @"SELECT * FROM Director WHERE URL = @url";

            return Query<Director>(ConnectionStrings.Jav, sql, new { url }).Count > 0;
        }

        public static int InsertDirector(Director entity)
        {
            var sql = @"INSERT INTO Director (Name, URL, CreateTime) VALUES (@Name, @URL, GETDATE())";

            return Execute(ConnectionStrings.Jav, sql, entity);
        }

        public static bool HasPublisher(string url)
        {
            var sql = @"SELECT * FROM Publisher WHERE URL = @url";

            return Query<Publisher>(ConnectionStrings.Jav, sql, new { url }).Count > 0;
        }

        public static int InsertPublisher(Publisher entity)
        {
            var sql = @"INSERT INTO Publisher (Name, URL, CreateTime) VALUES (@Name, @URL, GETDATE())";

            return Execute(ConnectionStrings.Jav, sql, entity);
        }

        public static bool HasActress(string url)
        {
            var sql = @"SELECT * FROM Actress WHERE URL = @url";

            return Query<Star>(ConnectionStrings.Jav, sql, new { url }).Count > 0;
        }

        public static int InsertActress(Star entity)
        {
            var sql = @"INSERT INTO Actress (Name, URL, CreateTime) VALUES (@Name, @URL, GETDATE())";

            return Execute(ConnectionStrings.Jav, sql, entity);
        }

        public static int UpdateScanURL(string url)
        {
            var sql = "update scanurl set isdownload = 1 where url = @url";
            return Execute(ConnectionStrings.Jav, sql, new { url });
        }

        public static int DeleteJavBusCategory()
        {
            var sql = @"TRUNCATE TABLE JavBusCategoryMapping";

            return Execute(ConnectionStrings.Jav, sql);
        }

        public static int InsertJavBusCategory(string javBusCategory, string javLibCategory)
        {
            var sql = @"INSERT INTO JavBusCategoryMapping (JavBusCategory, JavLibCategory) VALUES (@javBusCategory, @javLibCategory)";

            return Execute(ConnectionStrings.Jav, sql, new { javBusCategory, javLibCategory });
        }

        public static Dictionary<string, string> GetJavBusCategoryMapping()
        {
            var sql = @"SELECT JavBusCategory, JavLibCategory FROM JavBusCategoryMapping";

            return SqlHelper.ExecuteDataset(ConnectionStrings.Jav, CommandType.Text, sql).Tables[0].ToDictionary<string, string>("JavBusCategory", "JavLibCategory");
        }

        public static bool HasActressByName(string name)
        {
            var sql = @"SELECT * FROM Actress WHERE NAME = @name";

            return Query<Star>(ConnectionStrings.Jav, sql, new { name }).Count > 0;
        }

        public static bool HasDirectorByName(string name)
        {
            var sql = @"SELECT * FROM Director WHERE NAME = @name";

            return Query<Director>(ConnectionStrings.Jav, sql, new { name }).Count > 0;
        }

        public static bool HasCompanyByName(string name)
        {
            var sql = @"SELECT * FROM Company WHERE NAME = @name";

            return Query<Company>(ConnectionStrings.Jav, sql, new { name }).Count > 0;
        }

        public static bool HasCategoryByName(string name)
        {
            var sql = @"SELECT * FROM Category WHERE NAME = @name";

            return Query<Category>(ConnectionStrings.Jav, sql, new { name }).Count > 0;
        }

        public static List<CommonModel> GetSimilarContent(string table, string content)
        {
            var sql = @"SELECT Name FROM " + table + " WHERE NAME LIKE ('%" + content + "%')";

            return Query<CommonModel>(ConnectionStrings.Jav, sql);
        }

        public static List<CommonModel> GetSimilarContent(string table)
        {
            var sql = @"SELECT Name FROM " + table;

            return Query<CommonModel>(ConnectionStrings.Jav, sql);
        }

        public static List<Star> GetAllValidMap(string table)
        {
            var sql = @"SELECT Name, Url FROM " + table + " WHERE Url <> ''";

            return Query<Star>(ConnectionStrings.Jav, sql);
        }

		public void Dispose()
		{
			db.Dispose();
		}
	}
}
