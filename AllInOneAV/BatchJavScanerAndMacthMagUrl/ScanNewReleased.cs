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
	class ScanNewReleased
	{
		public const string CallingArg = "sr";
		public const string JobName = "ScanNewReleasedMovie";
		public bool scanMovie = false;

		public ScanNewReleased()
		{
			LogService.ConfigureSerilog(JobName);
		}

		public void RunJob()
		{
			Log.Information($"Job {JobName} Start");
			try
			{
				Log.Information("Get Javlibrary Cookie");
				JavLibraryHelper.GetJavCookieChromeProcess();
				//Task.Run(() => JavLibraryHelper.RefreshCookie(30));
				List<Movie> lstMovie = new List<Movie>();

				int pageCount = JavLibraryHelper.GetPageCount(JavLibrary.NewReleaseUrl);

				if(pageCount > 0)
				{
					Log.Information($"Found {pageCount} pages. Now scanning movie on each page");
					for(int currentPage = 1; currentPage <= pageCount; currentPage++)
					{
						List<Movie> lstMovieCurrentPage = JavLibraryHelper.ScanPageList(JavLibrary.NewReleaseUrl + $"?&mode=&page={currentPage}");
						lstMovie.AddRange(lstMovieCurrentPage);
					}
				}
				
				Log.Information($"Scanning page finised. Found {lstMovie.Count} movies. Now removing the duplicated movies");

				lstMovie = lstMovie.GroupBy(x => x.Url).Select(x => x.First()).ToList();
				lstMovie = lstMovie.FindAll(x => JavLibraryHelper.LoadMovieByNumber(x.Number) == null);

				Log.Information($"{lstMovie.Count} movies rest and ready to scan the detailed information & saved in DB");

				foreach(Movie movie in lstMovie)
				{
					JavLibraryHelper.SaveMovie(movie);
					Log.Debug($"Movie {movie.Number} has been saved into DB");
				}

				Log.Information($"Job {JobName} Finished");
			}
			catch(Exception ex)
			{
				Log.Fatal(ex, $"Error occurred when doing job {JobName}");
			}
		}
	}
}
