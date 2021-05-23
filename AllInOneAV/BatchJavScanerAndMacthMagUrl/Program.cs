using DataBaseManager.JavDataBaseHelper;
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
			if(args[0] == ScanNewReleased.CallingArg)
			{
				ScanNewReleased sr = new ScanNewReleased();
				sr.RunJob();

				if(args.Length > 1)
				{
					if(args[1] == ScanMovieFromDB.CallingArg)
					{
						ScanMovieFromDB sm = new ScanMovieFromDB();
						sm.RunJob();
					}
				}
			}

			if (args[0] == FullScanByCategory.CallingArg)
            {
				FullScanByCategory fsc = new FullScanByCategory();
				fsc.RunJob();
			}

			//TODO
			//Add Help Command
        }
    }
}
