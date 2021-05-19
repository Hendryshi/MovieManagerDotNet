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

		public DownloadCategory() { }

		public void RunJob()
		{
			Log.Information("Downloading Javlibrary Category");

			JavLibraryHelper.GetJavCookieChromeProcess();

			List<Category> lstCategory = JavLibraryHelper.ScanCatogery();

			foreach(Category c in lstCategory)
			{
				JavLibraryHelper.SaveCategory(c);
			}

			foreach(Category c in lstCategory)
			{
				Console.WriteLine(c.IdCategory);
			}
		}
	}
}
