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
	class DownloadSingleMovie
	{
		public const string CallingArg = "single-movie";
		public const string JobName = "DownloadSingleMovie";

		public DownloadSingleMovie()
		{
			LogService.ConfigureSerilog("DownloadSingleMovie");
		}

		public void RunJob()
		{
			Log.Information($"Job {JobName} Start");
			string domain = "https://www.javlibrary.com/cn/";
			List<Movie> lstFailedMovie = new List<Movie>();

			try
			{
				JavLibraryHelper.GetJavCookieChromeProcess();
				Task.Run(() => JavLibraryHelper.RefreshCookie(15));
				List<Movie> lstMovie = JavLibraryHelper.LoadAllMovieByStatus(MovieStatus.NotScanned).Take(5000).ToList();

				Log.Information($"{lstMovie.Count} movies will be treated");

				foreach(Movie movie in lstMovie)
				{
					if(!JavLibraryHelper.ScanAndDownloadMovieInfo(movie))
						lstFailedMovie.Add(movie);
				}

				Log.Information($"{lstMovie.FindAll(x => x.IdStatus == (int)MovieStatus.Scanned).Count} movies have been saved into DB");
				
				Log.Information($"Job {JobName} finised.");

				foreach(Movie movie in lstFailedMovie)
					Log.Warning($"Movie {movie.Number} Url [{movie.Url}] failed. Please check");

			}
			catch(Exception ex)
			{
				Log.Fatal(ex, "Error occurred when doing job");
			}
		}
	}
}
