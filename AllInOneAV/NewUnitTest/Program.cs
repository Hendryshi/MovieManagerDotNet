using Model.JavModels;
using Service;
using System;
using System.Threading;
using Service;
using Serilog;

namespace NewUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {

			//List<MovieRelation> lstMovieRelation = new List<MovieRelation>();

			//lstMovieRelation.Add(new MovieRelation() { IdMovie = 1, IdTyRole = 1, IdRelation = 1 });
			//lstMovieRelation.Add(new MovieRelation() { IdMovie = 1, IdTyRole = 1, IdRelation = 1 });
			//lstMovieRelation.Add(new MovieRelation() { IdMovie = 1, IdTyRole = 2, IdRelation = 1 });
			//lstMovieRelation.Add(new MovieRelation() { IdMovie = 1, IdTyRole = 3, IdRelation = 1 });

			//foreach(MovieRelation mr in lstMovieRelation.GroupBy(x => new { x.IdMovie, x.IdRelation, x.IdTyRole }).Select(y => y.First()))
			//{
			//	Console.WriteLine(mr.ToString());
			//}

			LogService.ConfigureSerilog("test");

			Console.Write("Performing some task... ");

			Log.Information("Performing some task... ");
			Log.Information("Performing so ");
			Log.Information("Performing some task... ");
			Log.Information("Performing some task... ");
			Log.Information("Performing some task... ");
			Log.Information("Performing so ");
			Log.Information("Performing some task... ");
			Log.Information("Performing some task... ");

			Console.SetCursorPosition(0, 4);
				Log.Information("Performing some task... ");
				Log.Information("Performing so ");
				Log.Information("Performing some task... ");
				Log.Information("Performing some task... ");
				
				Thread.Sleep(20);
			
			Console.WriteLine("Done.");


			Category cat = new Category() { Name = "偷窥", Url = "vl_genre.php?g=ju", DtUpdate = DateTime.Now };
			Console.WriteLine(cat.ToString());

            //var test = OneOneFiveService.RemoveDuplicated115Files();
            //Console.WriteLine(test);

            //OneOneFiveService.Match115AndMoveLocalFile();

            //OneOneFiveService.Insert115FileSha();

            //OneOneFiveService.MatchLocalAndOneOneFive();

            //OneOneFiveService.Match115AndMoveLocalFile();

            //TestFind115(@"d://up115");

            //var oneOneFilveFiles = OneOneFiveService.Get115FilesModel();
            //var localFileve = OneOneFiveService.GetAllLocalAvs();

            //var list = OneOneFiveService.InitLocalSha(true);

            //OneOneFiveService.SyncLocalAnd115FileStatus();

            //var list = OneOneFiveService.GetFileToBeDeletedBySize(2);
            //var deleteSize = FileSize.GetAutoSizeString(list.Sum(x => x.Length), 1);

            //JavBusDownloadHelper.AvatorMatch();

            //var extraFiles = OneOneFiveService.Get115HasButLocal();
            //OneOneFiveService.DeleteList(extraFiles, "1834397846621504875");

            //var ret = OneOneFiveService.GetLocalAndRemoteFiles(includeUpFolder: true, scope: FileSearchScope.Remote);

            //var local = ret.Where(x => x.IsLocal == true).ToList();
            //var remote = ret.Where(x => x.IsLocal == false).ToList();

            //var m3u8 = OneOneFiveService.GetM3U8("dv8lxbvp9vuqnx2ps");

            //RedisService.SetHash("avdb", "115cookie", JsonConvert.SerializeObject(OneOneFiveService.Get115FilesModel()));

            //var ret = RedisService.GetHash("avdb", "123");
            //var ret2 = RedisService.GetHashAll("avdb");

            //var filesInCloudDownload = OneOneFiveService.Get115FilesModel("2091444771508569858", OneOneFiveSearchType.Video);

            //var targetFiles = filesInCloudDownload.Where(x => x.s >= 500 * 1024 * 1024).ToList();
            //StringBuilder sb = new StringBuilder();

            //foreach (var file in targetFiles)
            //{
            //    sb.AppendLine(OneOneFiveService.GetM3U8(file.pc));
            //}

            //Console.WriteLine(sb.ToString());

            //Clipboard.SetText(sb.ToString());

            //2092131778538882474
            //2091444771508569858
            //OneOneFiveService.Rename(targetFiles, "2092131778538882474", "2091514445650838660", OneOneFiveService.Get115Cookie());

            //var ret = OneOneFiveService.Get115SearchFileResult(OneOneFiveService.Get115Cookie(), "KV-119-128分間ノンストップ撮影、ノーカット編集で生中出し29連発に長時間お掃除フェラ！！.mp4", "2091444771508569858");

            //var relatedFile = ret.Where(x => x.n.StartsWith("KV-119-128分間ノンストップ撮影、ノーカット編集で生中出し29連発に長時間お掃除フェラ！！")).OrderBy(x => x.n).ToList();

            //var count = relatedFile.Count;

            //foreach (var file in relatedFile)
            //{
            //    var fileName = file.n.Replace("." + file.ico, "");

            //    if (fileName.EndsWith(")"))
            //    {
            //        int number = -1;
            //        var numberStr = fileName.Substring(fileName.LastIndexOf("(") + 1);

            //        int.TryParse(numberStr.Substring(0, numberStr.Length - 1), out number);

            //        if (number > 0 && number < count)
            //        {
            //            fileName = fileName.Substring(0, fileName.LastIndexOf("("));
            //            fileName += "-" + (count - number) + "." + file.ico;
            //        }
            //    }
            //    else
            //    {
            //        fileName += "-" + count + "." + file.ico;
            //    }
            //}

            //OneOneFiveService.TestDownload();

            //var ret = OneOneFiveService.RecordDeleteFiles("s");

            //Console.WriteLine(ret.Count);

            //OneOneFiveService.Copy("2091258601285732082", "2092826214403000069", OneOneFiveService.Get115Cookie());

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
              

        
    }
}
