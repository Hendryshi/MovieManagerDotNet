using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service;
using Serilog;
using Model.JavModels;
using Model.Common;

namespace BatchJavScaner
{
	class FullScanByCategory
	{
		public const string CallingArg = "fsc";
		public const string JobName = "FullScanByCategory";
		public bool scanMovie = false;

		public FullScanByCategory()
		{
			LogService.ConfigureSerilog(JobName);
		}

		public void RunJob()
		{
			Log.Information($"Job {JobName} Start");
			try
			{
				JavLibraryHelper.GetJavCookieChromeProcess();
				Task.Run(() => JavLibraryHelper.RefreshCookie(15));
				List<Category> lstCategory = JavLibraryHelper.LoadAllCategory(); //.FindAll(l => l.IdCategory >= 241 && l.IdCategory <= 280 && l.IdCategory != 268);
				List<Movie> lstMovie = new List<Movie>();
				
				Parallel.ForEach(lstCategory, new ParallelOptions { MaxDegreeOfParallelism = 5 }, category =>
				{
					int pageCount = JavLibraryHelper.GetPageCount(JavLibrary.Domain + category.Url);
					List<Movie> lstCurrentCategory = new List<Movie>();

					if(pageCount > 0)
					{
						for(int currentPage = 1; currentPage <= pageCount; currentPage++)
						{
							List<Movie> lstMovieCurrentPage = JavLibraryHelper.ScanPageList(JavLibrary.Domain + category.Url + $"&page={currentPage}");
							lstCurrentCategory.AddRange(lstMovieCurrentPage);
						}

						lstMovie.AddRange(lstCurrentCategory);
						Log.Information($"Category {category.Name} found {pageCount} pages, having {lstCurrentCategory.Count} movies");
					}
					else
						Log.Warning($"Category {category.Name} did not find any movie: Url [{category.Url}] !");
				});

				Log.Information($"Scanning page finised. Found {lstMovie.Count} movies. Now removing the duplicated movies");

				lstMovie = lstMovie.GroupBy(x => x.Url).Select(x => x.First()).ToList();
				lstMovie = lstMovie.FindAll(x => JavLibraryHelper.LoadMovieByNumber(x.Number) == null);

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
