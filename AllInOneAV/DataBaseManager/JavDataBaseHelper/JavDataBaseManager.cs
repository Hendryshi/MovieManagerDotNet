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
			try
			{
				if(!checkExist || !ExistCategory(category))
				{
					db.Category.InsertOnSubmit(category);
					db.SubmitChanges();
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting category : {category.ToString()}");
				throw ex;
			}
		}

		public bool ExistCategory(Category cat, bool checkName = true, bool checkUrl = true)
		{
			var query = from category in db.Category
						select category;

			if(checkName)
				query = query.Where(c => c.Name == cat.Name);

			if(checkUrl)
				query = query.Where(c => c.Url == cat.Url);

			return query.ToList().Count > 0;
		}

		public List<Category> LoadAllCategory()
		{
			var query = from category in db.Category select category;
			return query.ToList();
		}

		public List<Movie> LoadAllMovieByStatus(MovieStatus status)
		{
			var query = from movie in db.Movie where movie.IdStatus == (int)status select movie;
			return query.ToList();
		}

		public void InsertCompany(Company company, bool checkExist = true)
		{
			try
			{
				if(!checkExist || !ExistCompany(company))
				{
					db.Company.InsertOnSubmit(company);
					db.SubmitChanges();
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting company : {company.ToString()}");
				throw ex;
			}
		}

		public bool ExistCompany(Company com, bool checkName = true, bool checkUrl = true)
		{
			var query = from company in db.Company
						select company;

			if(checkName)
				query = query.Where(c => c.Name == com.Name);

			if(checkUrl)
				query = query.Where(c => c.Url == com.Url);

			return query.ToList().Count > 0;
		}

		public void InsertDirector(Director director, bool checkExist = true)
		{
			try
			{
				if(!checkExist || !ExistDirector(director))
				{
					db.Director.InsertOnSubmit(director);
					db.SubmitChanges();
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting director : {director.ToString()}");
				throw ex;
			}
		}

		public bool ExistDirector(Director dir, bool checkName = true, bool checkUrl = true)
		{
			var query = from director in db.Director
						select director;

			if(checkName)
				query = query.Where(c => c.Name == dir.Name);

			if(checkUrl)
				query = query.Where(c => c.Url == dir.Url);

			return query.ToList().Count > 0;
		}

		public void InsertPublisher(Publisher publisher, bool checkExist = true)
		{
			try
			{
				if(!checkExist || !ExistPublisher(publisher))
				{
					db.Publisher.InsertOnSubmit(publisher);
					db.SubmitChanges();
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting publisher : {publisher.ToString()}");
				throw ex;
			}
		}

		public bool ExistPublisher(Publisher pub, bool checkName = true, bool checkUrl = true)
		{
			var query = from publisher in db.Publisher
						select publisher;

			if(checkName)
				query = query.Where(c => c.Name == pub.Name);

			if(checkUrl)
				query = query.Where(c => c.Url == pub.Url);

			return query.ToList().Count > 0;
		}

		public void InsertStar(Star star, bool checkExist = true)
		{
			try
			{
				if(!checkExist || !ExistStar(star))
				{
					db.Star.InsertOnSubmit(star);
					db.SubmitChanges();
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting star : {star.ToString()}");
				throw ex;
			}
		}

		public bool ExistStar(Star sta, bool checkName = true, bool checkUrl = true)
		{
			var query = from star in db.Star
						select star;

			if(checkName)
				query = query.Where(c => c.Name == sta.Name);

			if(checkUrl)
				query = query.Where(c => c.Url == sta.Url);

			return query.ToList().Count > 0;
		}

		public void InsertMovie(Movie movie, bool checkExist = true)
		{
			try
			{
				if(!checkExist || !ExistMovie(movie.Number))
				{
					using(TransactionScope ts = new TransactionScope())
					{
						movie.DtUpdate = DateTime.Now;
						db.Movie.InsertOnSubmit(movie);
						db.SubmitChanges();

						foreach(MovieRelation mr in movie.MovieRelation.GroupBy(x => new { x.IdMovie, x.IdRelation, x.IdTyRole }).Select(y => y.First()))
						{
							mr.IdMovie = movie.IdMovie;
							db.MovieRelation.InsertOnSubmit(mr);
							db.SubmitChanges();
						}

						ts.Complete();
					}
				}
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when inserting movie : {movie.ToString()}");
				throw ex;
			}
		}

		public void UpdateMovie(Movie movie)
		{
			try
			{
				Movie lqMovie = db.Movie.Single(x => x.IdMovie == movie.IdMovie);
				lqMovie.CopyValue(movie);
				db.SubmitChanges();
			}
			catch(Exception ex)
			{
				Log.Fatal($"Error when updating movie : {movie.ToString()}");
				throw ex;
			}
		}

		public bool ExistMovie(string number)
		{
			var query = from movie in db.Movie
						where movie.Number == number
						select movie;

			return query.ToList().Count > 0;
		}

		public Movie LoadMovieByNumber(string number)
		{
			var query = from movie in db.Movie where movie.Number == number select movie;

			return query.Count() > 0 ? query.First() : null;
		}

		public Company LoadCompany(string name, string url)
		{
			var query = from company in db.Company where company.Name == name && company.Url == url select company;

			return query.Count() > 0 ? query.First() : null;
		}

		public Category LoadCategory(string name, string url)
		{
			var query = from category in db.Category where category.Name == name && category.Url == url select category;

			return query.Count() > 0 ? query.First() : null;
		}
		public Director LoadDirector(string name, string url)
		{
			var query = from director in db.Director where director.Name == name && director.Url == url select director;

			return query.Count() > 0 ? query.First() : null;
		}
		public Publisher LoadPublisher(string name, string url)
		{
			var query = from publisher in db.Publisher where publisher.Name == name && publisher.Url == url select publisher;

			return query.Count() > 0 ? query.First() : null;
		}

		public Star LoadStar(string name, string url)
		{
			var query = from star in db.Star where star.Name == name && star.Url == url select star;

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
