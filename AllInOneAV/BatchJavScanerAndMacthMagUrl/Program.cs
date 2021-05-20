using DataBaseManager.JavDataBaseHelper;
using Model.Common;
using Model.JavModels;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace BatchJavScaner
{
    class Program
    {
        public static void Main(string[] args)
        {
            LockModel lockModel = new LockModel();
            if (args[0] == "daily")
            {
                //JavLibraryHelper.DoDailyUpdate(200, true);
            }

            if (args[0] == "full")
            {
                //JavLibraryHelper.DoFullScan(true);
            }

            if (args[0] == FullScanSingleThread.CallingArg)
            {
				//JavLibraryHelper.DoFullScanSingleThread(true);
				FullScanSingleThread fs = new FullScanSingleThread();
				fs.RunJob();
				Console.ReadKey();
			}

			if(args[0] == DownloadSingleMovie.CallingArg)
			{
				//JavLibraryHelper.DoFullScanSingleThread(true);
				DownloadSingleMovie fs = new DownloadSingleMovie();
				fs.RunJob();
				Console.ReadKey();
			}

			if(args[0] == DownloadCategory.CallingArg)
			{
				//JavLibraryHelper.DoFullScanSingleThread(true);
				DownloadCategory dc = new DownloadCategory();
				dc.RunJob();
				Console.ReadKey();
			}

			
        }
    }
}
