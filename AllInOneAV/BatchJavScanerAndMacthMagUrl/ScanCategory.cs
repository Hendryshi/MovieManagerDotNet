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
	class DownloadCategory
	{
		public const string CallingArg = "category";
		public const string JobName = "DownloadCategory";

		public DownloadCategory()
		{
			LogService.ConfigureSerilog(JobName);
		}

		public void RunJob()
		{
			Log.Information($"Start Job {JobName}");

			JavLibaryService.GetJavCookieChromeProcess();

			List<Category> lstCategory = JavLibaryService.ScanCatogery();

			Log.Information($"{lstCategory.Count} category found. Now saving into DB");

			foreach(Category c in lstCategory)
			{
				JavLibaryService.SaveCategory(c);
			}

			Log.Information($"Job {JobName} finised");
		}
	}
}
