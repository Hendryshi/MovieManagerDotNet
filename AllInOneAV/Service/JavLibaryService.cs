using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using DataBaseManager.JavDataBaseHelper;
using HtmlAgilityPack;
using Model.Common;
using Model.JavModels;
using Model.ScanModels;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using Utils;
using Publisher = Model.JavModels.Publisher;

namespace Service
{
	public class JavLibraryHelper
	{
		private static CookieContainer cc = new CookieContainer();
		private static readonly string ImgFolder = JavINIClass.IniReadValue("Jav", "imgFolder");
		private static readonly string Mode = JavINIClass.IniReadValue("Jav", "cookieMode");
		private static readonly string UserAgent = JavINIClass.IniReadValue("Html", "UserAgent");

		private static readonly string ChromeLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";



		public static void GetJavCookie(bool showConsole = true)
		{
			cc = new CookieContainer();

			GetJavCookieChromeProcess();
		}

		public static void GetJavCookieChromeProcess()
		{
			cc = new CookieContainer();
			var process = System.Diagnostics.Process.Start(ChromeLocation, "http://www.javlibrary.com/cn/");
			Thread.Sleep(1000);
			if(process != null && !process.HasExited)
			{
				process.Kill();
				process.Dispose();
			}

			var cookies = new ChromeCookieReader().ReadChromeCookies(".javlibrary.com");

			foreach(var c in cookies)
			{
				System.Net.Cookie coo = new System.Net.Cookie();

				if(c.Name.Contains("cf"))
				{
					coo.Name = c.Name;
					coo.Value = c.Value;
					coo.Domain = "www.javlibrary.com";

					cc.Add(coo);
				}
			}

			//Broswer.CloseBroswer();
		}

		
		#region Load
		public static List<Category> LoadAllCategory()
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadAllCategory();
		}

		public static List<Movie> LoadAllMovieByStatus(MovieStatus status)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadAllMovieByStatus(status);
		}

		public static Movie LoadMovieByNumber(string number)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadMovieByNumber(number);
		}


		public static Category LoadCategory(string name, string url)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadCategory(name, url);
		}

		public static Company LoadCompany(string name, string url)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadCompany(name, url);
		}

		public static Director LoadDirector(string name, string url)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadDirector(name, url);
		}

		public static Publisher LoadPublisher(string name, string url)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadPublisher(name, url);
		}

		public static Star LoadStar(string name, string url)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
				return db.LoadStar(name, url);
		}

		#endregion

		#region Scrape data for Javlibrary

		public static int GetPageCount(string pageUrl)
		{
			int lastPage = 0;
			HtmlDocument htmlDocument = TryGetHtmlDocument(pageUrl);
			if(htmlDocument != null)
			{
				var lastPagePath = "//a[@class='page last']";
				var videoPath = "//div[@class='video']";

				var lastPageNode = htmlDocument.DocumentNode.SelectSingleNode(lastPagePath);

				if(lastPageNode != null)
				{
					var pageStr = lastPageNode.Attributes["href"].Value.Trim();

					if(!string.IsNullOrEmpty(pageStr))
					{
						pageStr = pageStr.Substring(pageStr.LastIndexOf("=") + 1);
						int.TryParse(pageStr, out lastPage);
					}
				}
				else
				{
					var videoNodes = htmlDocument.DocumentNode.SelectNodes(videoPath);
					if(videoNodes != null && videoNodes.Count > 0)
						lastPage = 1;
				}
			}

			return lastPage;
		}

		/// <summary>
		///		Scan the page of movie & return the initial list of av
		/// </summary>
		/// <returns></returns>
		public static List<Movie> ScanPageList(string pageUrl)
		{
			List<Movie> lstMovie = new List<Movie>();

			try
			{
				HtmlDocument htmlDocument = TryGetHtmlDocument(pageUrl);

				if(htmlDocument != null)
				{
					var videoPath = "//div[@class='video']";
					var videoNodes = htmlDocument.DocumentNode.SelectNodes(videoPath);

					if(videoNodes != null)
					{
						foreach(var node in videoNodes)
						{
							var urlAndTitle = node.ChildNodes[0];
							if(urlAndTitle != null && urlAndTitle.ChildNodes.Count >= 3)
							{
								var id = urlAndTitle.ChildNodes[0].InnerText.Trim();
								var name = FileUtility.ReplaceInvalidChar(urlAndTitle.ChildNodes[2].InnerText.Trim());
								var avUrl = urlAndTitle.Attributes["href"].Value.Trim().Replace("./", "");

								if(!string.IsNullOrEmpty(avUrl) && !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(id))
									lstMovie.Add(new Movie() { Number = id, Title = name, Url = avUrl });
								else
									Log.Error($"Movie missing information important: id: [{id}] name [{name}] movieUrl [{avUrl}]");
							}
						}
					}
				}
				return lstMovie;
			}
			catch(Exception ex)
			{
				Log.Fatal(ex, $"Error occurred when scanning page {pageUrl}, some movies may not be scanned");
				return lstMovie;
			}
		}


		/// <summary>
		///		Scan the movie details & save in DB
		/// </summary>
		/// <returns></returns>
		public static bool ScanAndDownloadMovieInfo(Movie movie)
		{
			try
			{
				string domain = "https://www.javlibrary.com/cn/";
				HtmlDocument htmlDocument = TryGetHtmlDocument(domain + movie.Url);

				if(htmlDocument != null)
				{
					GenerateAVModel(htmlDocument, movie);
					movie.IdStatus = (int)MovieStatus.Scanned;
					UpdateMovie(movie);
				}
				return true;
			}
			catch(Exception ex)
			{
				movie.IdStatus = (int)MovieStatus.InError;
				Log.Fatal(ex, $"Error occurred when scanning movie {movie.Number}");
				return false;
			}
		}

		public static List<Category> ScanCatogery()
		{
			List<Category> lstCategory = new List<Category>();
			string categoryUrl = "http://www.javlibrary.com/cn/genres.php";
			HtmlDocument htmlDocument = TryGetHtmlDocument(categoryUrl);

			if(htmlDocument != null)
			{
				var genrePath = "//div[@class='genreitem']";
				var genreNodes = htmlDocument.DocumentNode.SelectNodes(genrePath);

				foreach(var node in genreNodes)
				{
					var aTagHref = node.ChildNodes[0].Attributes["href"].Value.Trim();
					var aTagTitle = node.ChildNodes[0].InnerText.Trim();

					lstCategory.Add(new Category { Name = aTagTitle, Url = aTagHref, DtUpdate = DateTime.Now });
				}
			}
			
			return lstCategory;
		}

		public static void ScanStars()
		{

		}

		public static List<string> GetPageUrl()
		{
			return null;
		}

		#endregion

		#region Save Data

		public static void SaveCategory(Category category)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertCategory(category);
			}
		}

		public static void SaveMovie(Movie movie)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertMovie(movie);
			}
		}

		public static void UpdateMovie(Movie movie)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.UpdateMovie(movie);
			}
		}

		public static void SaveCompany(Company company)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertCompany(company);
			}
		}

		public static void SaveDirector(Director director)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertDirector(director);
			}
		}

		public static void SavePublisher(Publisher publisher)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertPublisher(publisher);
			}
		}

		public static void SaveStar(Star star)
		{
			using(JavDataBaseManager db = new JavDataBaseManager())
			{
				db.InsertStar(star);
			}
		}

		#endregion

		public static HtmlDocument TryGetHtmlDocument(string requestUrl)
		{
			int retry = 0;
			HtmlDocument html = null;
			while(retry < 2 && html == null)
			{
				html = HtmlManager.GetHtmlDocumentAsync(requestUrl, cc).Result;

				if(html == null)
				{
					GetJavCookieChromeProcess();
					retry++;
				}
			}

			if(html == null && retry == 2)
				Log.Fatal($"Error when requesting page {requestUrl}");

			return html;
		}


		private static void GenerateAVModel(HtmlDocument htmlDocument, Movie movie)
		{
			var titlePath = "//h3[@class='post-title text']";
			var picPath = "//img[@id='video_jacket_img']";

			var dtReleasePath = "//div[@id='video_date']//td[@class='text']";
			var lengthPath = "//div[@id='video_length']//span[@class='text']";

			var dirPath = "//span[@class='director']//a";
			var comPath = "//span[@class='maker']//a";
			var pubPath = "//span[@class='label']//a";

			var catPath = "//span[@class='genre']//a";
			var starPath = "//span[@class='star']//a";

			var titleNode = htmlDocument.DocumentNode.SelectSingleNode(titlePath);
			var title = titleNode.InnerText.Trim();
			var number = title.Substring(0, title.IndexOf(" "));
			title = FileUtility.ReplaceInvalidChar(title.Substring(title.IndexOf(" ") + 1));
			var picUrl = htmlDocument.DocumentNode.SelectSingleNode(picPath);

			movie.PictureUrl = picUrl.Attributes["src"].Value;
			movie.PictureUrl = movie.PictureUrl.StartsWith("http") ? movie.PictureUrl : "http:" + movie.PictureUrl;

			if(movie.Title == null) 
				movie.Title = title;
			
			if(movie.Number == null)
			   movie.Number = number;

			var release = htmlDocument.DocumentNode.SelectSingleNode(dtReleasePath);
			DateTime rDate = DateTime.MinValue;
			if(release != null && !string.IsNullOrEmpty(release.InnerText))
			{
				if(DateTime.TryParse(release.InnerText.Trim(), out rDate))
					movie.DtRelease = rDate;
			}

			var duration = htmlDocument.DocumentNode.SelectSingleNode(lengthPath);
			if(duration != null && !string.IsNullOrEmpty(duration.InnerText))
			{
				movie.Duration = int.Parse(duration.InnerText.Trim());
			}

			var dirNode = htmlDocument.DocumentNode.SelectNodes(dirPath);
			if(dirNode != null)
			{
				foreach(var dir in dirNode)
				{
					var name = dir.InnerHtml.Trim();
					var url = dir.Attributes["href"].Value;
					movie.Director += name + ",";

					Director d = LoadDirector(name, url);
					if(d == null)
					{
						Log.Information($"Found new Director: {name}");
						d = new Director() { Name = name, Url = url, DtUpdate = DateTime.Now };
						SaveDirector(d);
					}
					movie.MovieRelation.Add(new MovieRelation() { IdRelation = d.IdDirector, IdTyRole = (int)RoleType.Director });
				}
				movie.Director = movie.Director.Trim(',');
			}

			var comNode = htmlDocument.DocumentNode.SelectNodes(comPath);
			if(comNode != null)
			{
				foreach(var com in comNode)
				{
					var name = com.InnerHtml.Trim();
					var url = com.Attributes["href"].Value;
					movie.Company += name + ",";

					Company d = LoadCompany(name, url);
					if(d == null)
					{
						Log.Information($"Found new Company: {name}; Insert it into DB");
						d = new Company() { Name = name, Url = url, DtUpdate = DateTime.Now };
						SaveCompany(d);
					}
					movie.MovieRelation.Add(new MovieRelation() { IdRelation = d.IdCompany, IdTyRole = (int)RoleType.Company });
				}
				movie.Company = movie.Company.Trim(',');
			}

			var pubNode = htmlDocument.DocumentNode.SelectNodes(pubPath);
			if(pubNode != null)
			{
				foreach(var pub in pubNode)
				{
					var name = pub.InnerHtml.Trim();
					var url = pub.Attributes["href"].Value;
					movie.Publisher += name + ",";

					Publisher d = LoadPublisher(name, url);
					if(d == null)
					{
						Log.Information($"Found new Publisher: {name}; Insert it into DB");
						d = new Publisher() { Name = name, Url = url, DtUpdate = DateTime.Now };
						SavePublisher(d);
					}
					movie.MovieRelation.Add(new MovieRelation() { IdRelation = d.IdPublisher, IdTyRole = (int)RoleType.Publisher });
				}
				movie.Publisher = movie.Publisher.Trim(',');
			}

			var catNodes = htmlDocument.DocumentNode.SelectNodes(catPath);
			if(catNodes != null)
			{
				foreach(var cat in catNodes)
				{
					var name = cat.InnerHtml.Trim();
					var url = cat.Attributes["href"].Value;
					movie.Category += name + ",";

					Category d = LoadCategory(name, url);
					if(d == null)
					{
						Log.Information($"Found new Category: {name}; Insert it into DB");
						d = new Category() { Name = name, Url = url, DtUpdate = DateTime.Now };
						SaveCategory(d);
					}
					movie.MovieRelation.Add(new MovieRelation() { IdRelation = d.IdCategory, IdTyRole = (int)RoleType.Category });
				}
				movie.Category = movie.Category.Trim(',');
			}

			var starNodes = htmlDocument.DocumentNode.SelectNodes(starPath);
			if(starNodes != null)
			{
				foreach(var star in starNodes)
				{
					var name = star.InnerHtml.Trim();
					var url = star.Attributes["href"].Value;
					movie.Star += name + ",";

					Star d = LoadStar(name, url);
					if(d == null)
					{
						Log.Information($"Found new Star: {name}; Insert it into DB");
						d = new Star() { Name = name, Url = url, DtUpdate = DateTime.Now };
						SaveStar(d);
					}
					movie.MovieRelation.Add(new MovieRelation() { IdRelation = d.idStar, IdTyRole = (int)RoleType.Star });
				}
				movie.Star = movie.Star.Trim(',');
			}
		}

		
		public static void RefreshCookie(int mintutes)
		{
			while(true)
			{
				Thread.Sleep(1000 * 60 * (mintutes - 5));

				GetJavCookie(false);

				Console.WriteLine("*********************更新Cookie*********************");
			}
		}
	}
}
