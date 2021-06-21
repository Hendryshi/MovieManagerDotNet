﻿using System;
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
	class ScanMovieMagnet
	{
		public const string CallingArg = "smm";
		public const string JobName = "ScanMovieMagnet";
		public int ScanCount = 10000;

		public ScanMovieMagnet(int scanCount = 0)
		{
			LogService.ConfigureSerilog(JobName);
			if(scanCount != 0)
				this.ScanCount = scanCount;
		}

		public void RunJob()
		{
			Log.Information($"Job {JobName} Start");
			List<Movie> lstFailedMovie = new List<Movie>();

			try
			{
				//JavLibaryService.GetJavCookieChromeProcess();
				//Task.Run(() => JavLibaryService.RefreshCookie(60));
				//List<Movie> lstMovie = JavLibaryService.LoadAllMovieByStatus(MovieStatus.NotScanned).Take(ScanCount).ToList();

				//Log.Information($"{lstMovie.Count} movies will be treated");

				//foreach(Movie movie in lstMovie)
				//{
				//	if(JavLibaryService.ScanAndDownloadMovieInfo(movie))
				//		Log.Debug($"Movie {movie.Number} has been treated");
				//}

				//Log.Information($"{lstMovie.FindAll(x => x.IdStatus == (int)MovieStatus.Scanned).Count} movies have been saved into DB");
				//Log.Information($"Job {JobName} finised.");

				//foreach(Movie movie in lstMovie.FindAll(m => m.IdStatus == (int)MovieStatus.InError))
				//	Log.Warning($"Movie {movie.Number} Url [{movie.Url}] failed. Please check");


				List<Movie> lstMovie = JavLibaryService.LoadAllMovieByStatus(MovieStatus.Scanned).Take(100).ToList();
				foreach(Movie movie in lstMovie)
					JavBusService.ScanMovieMagnet(movie);
				Console.ReadKey();
			}
			catch(Exception ex)
			{
				Log.Fatal(ex, $"Error occurred when doing job {JobName}");
			}
		}
	}
}
