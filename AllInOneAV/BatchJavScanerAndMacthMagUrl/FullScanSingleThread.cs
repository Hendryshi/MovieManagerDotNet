using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service;
using Serilog;
using Model.JavModels;
using System.Transactions;
using DataBaseManager.JavDataBaseHelper;

namespace BatchJavScaner
{
	class FullScanSingleThread
	{
		public const string CallingArg = "single";
		public const string JobName = "FullScanByCategory";
		public bool scanMovie = false;

		public FullScanSingleThread()
		{
			LogService.ConfigureSerilog(JobName);
		}

		public void RunJob()
		{
			Log.Information($"Job {JobName} Start");
			string domain = "https://www.javlibrary.com/cn/";

			try
			{
				JavLibraryHelper.GetJavCookieChromeProcess();
				Task.Run(() => JavLibraryHelper.RefreshCookie(15));
				List<Category> lstCategory = JavLibraryHelper.LoadAllCategory().FindAll(l => l.IdCategory >= 241 && l.IdCategory <= 280 && l.IdCategory != 268);
				List<Movie> lstMovie = new List<Movie>();
				
				Parallel.ForEach(lstCategory, new ParallelOptions { MaxDegreeOfParallelism = 5 }, category =>
				{
					int pageCount = JavLibraryHelper.GetPageCount(domain + category.Url);
					List<Movie> lstCurrentCategory = new List<Movie>();

					if(pageCount > 0)
					{
						for(int currentPage = 1; currentPage <= pageCount; currentPage++)
						{
							List<Movie> lstMovieCurrentPage = JavLibraryHelper.ScanPageList(domain + category.Url + $"&page={currentPage}");
							lstCurrentCategory.AddRange(lstMovieCurrentPage);
						}

						lstMovie.AddRange(lstCurrentCategory);
						Log.Information($"Category {category.Name} found {pageCount} pages, having {lstCurrentCategory.Count} movies");
					}
					else
						Log.Warning($"Category {category.Name} did not find any movie, please check: Url [{category.Url}] !");
				});

				Log.Information($"Scanning page finised. Found {lstMovie.Count} movies. Now removing the duplicated movies");

				lstMovie = lstMovie.GroupBy(x => x.Url).Select(x => x.First()).ToList();

				Log.Information($"{lstMovie.Count} movies rest and ready to scan the detailed information & saved in DB");

				foreach(Movie movie in lstMovie)
				{
					JavLibraryHelper.SaveMovie(movie);
				}

				Log.Information($"Job {JobName} Finished");
				
			}
			catch(Exception ex)
			{
				Log.Fatal(ex, "Error occurred when doing job");
			}
		}
	}
}
