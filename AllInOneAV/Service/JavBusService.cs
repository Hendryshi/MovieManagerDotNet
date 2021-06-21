using HtmlAgilityPack;
using Model.JavModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace Service
{
	public class JavBusService
	{
		private static readonly string JavBusUrl = "https://www.javbus.com/{0}";

		public static void ScanMovieMagnet(Movie movie)
		{
			string javbusUrl = string.Format(JavBusUrl, movie.Number);
			HtmlDocument htmlDocument = TryGetHtmlDocument(javbusUrl);
			HtmlDocument magDocument = null;
			List<MovieMagnet> lstMovieMagnets = new List<MovieMagnet>();

			if(htmlDocument != null)
			{
				string outerHtml = htmlDocument.DocumentNode.InnerHtml;
				var gidPattern = "var gid = (.*?);";
				var ucPattern = "var uc = (.*?);";
				var picPattern = "var img = '(.*?)';";

				var gidMatch = Regex.Match(outerHtml, gidPattern);
				var ucMatch = Regex.Match(outerHtml, ucPattern);
				var picMatch = Regex.Match(outerHtml, picPattern);

				var gid = gidMatch.Groups[1].Value;
				var uc = ucMatch.Groups[1].Value;
				var pic = picMatch.Groups[1].Value;

				var magUrl = $"https://www.javbus.com/ajax/uncledatoolsbyajax.php?gid={gid}&lang=zh&img={pic}&uc={uc}&floor=552";
				magDocument = TryGetHtmlDocument(magUrl, new Dictionary<string, string>() { { "referer", javbusUrl } });
			}

			if(magDocument != null)
			{
				var magPattern = "//tr[@style=' border-top:#DDDDDD solid 1px']";
				HtmlNodeCollection nodes = magDocument.DocumentNode.SelectNodes(magPattern);
				if(nodes != null)
				{
					foreach(var node in nodes)
					{
						MovieMagnet movieMagnet = new MovieMagnet() { IdMovie = movie.IdMovie, Number = movie.Number };
						if(node.ChildNodes.Count >= 2)
						{
							if(node.ChildNodes[1].InnerText.Contains("高清"))
								movieMagnet.IsHD = true;

							if(node.ChildNodes[1].InnerText.Contains("字幕"))
								movieMagnet.HasSubtitle = true;

							movieMagnet.MagnetName = node.ChildNodes[1].ChildNodes[1].InnerText.Trim();
							movieMagnet.MagnetLink = node.ChildNodes[1].ChildNodes[1].Attributes["href"].Value;
						}

						if(node.ChildNodes.Count >= 4)
						{
							string sizePart = node.ChildNodes[3].InnerText.Trim();
							movieMagnet.Size = FileUtility.GetByteFromStr(sizePart);
						}

						if(node.ChildNodes.Count >= 5)
						{
							movieMagnet.DtMagnet = DateTime.Parse(node.ChildNodes[5].InnerText.Trim());
						}
						lstMovieMagnets.Add(movieMagnet);
					}
				}
			}

			lstMovieMagnets = lstMovieMagnets.OrderByDescending(x => x.HasSubtitle).ThenByDescending(x => x.IsHD).ThenByDescending(x => x.DtMagnet).ThenByDescending(x => x.Size).ToList();

			if(lstMovieMagnets.Count > 0)
				JavLibaryService.SaveMovieMagnet(lstMovieMagnets.First());
			else
				Log.Warning($"Cannot find mag for {movie.Number}");
		}



		public static HtmlDocument TryGetHtmlDocument(string requestUrl, Dictionary<String, String> headers = null)
		{
			int retry = 0;
			HtmlDocument html = null;
			while(retry < 2 && html == null)
			{
				html = HtmlManager.GetHtmlDocumentAsync(requestUrl, headers: headers).Result;

				if(html == null)
					retry++;
			}

			if(html == null && retry == 2)
				Log.Fatal($"Error when requesting page {requestUrl}");

			return html;
		}

	}
}
