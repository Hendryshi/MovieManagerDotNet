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

				if(lqMovie != null)
				{
					using(TransactionScope ts = new TransactionScope())
					{
						lqMovie.CopyValue(movie);
						db.SubmitChanges();

						var delete = from mr in db.MovieRelation where mr.IdMovie == movie.IdMovie select mr;
						db.MovieRelation.DeleteAllOnSubmit(delete);
						db.SubmitChanges();

						foreach(MovieRelation mr in lqMovie.MovieRelation.GroupBy(x => new { x.IdMovie, x.IdRelation, x.IdTyRole }).Select(y => y.First()))
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
	}
}
