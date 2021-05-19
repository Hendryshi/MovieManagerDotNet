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

		public FullScanSingleThread() { }

		public void RunJob()
		{
			Log.Information("Getting Javlibrary Cookie");
			string domain = "https://www.javlibrary.com/cn/";


			JavLibraryHelper.GetJavCookieChromeProcess();
			List<Category> lstCategory = JavLibraryHelper.LoadCategory().FindAll(l => l.IdCategory == 1);
			List<Movie> lstMovie = new List<Movie>();

			Parallel.ForEach(lstCategory, new ParallelOptions { MaxDegreeOfParallelism = 10 }, category =>
			{
				int pageCount = JavLibraryHelper.GetPageCount(domain + category.Url);

				for(int currentPage = 1; currentPage <= (pageCount < 1 ? pageCount : 1); currentPage ++)
				{
					List<Movie> lstMovieCurrentPage = JavLibraryHelper.ScanPageList(domain + category.Url + $"&page={currentPage}");
					lstMovie.AddRange(lstMovieCurrentPage);

					Console.WriteLine($"Category {category.Name} 的第{currentPage}页, 有{lstMovieCurrentPage.Count}部电影，第一部电影番号是: {lstMovieCurrentPage[0].Number}");
				}
			});

			Parallel.ForEach(lstMovie, new ParallelOptions { MaxDegreeOfParallelism = 10 }, movie =>
			{
				JavLibraryHelper.ScanAndDownloadMovieInfo(movie);
			});
			
		}
	}
}
