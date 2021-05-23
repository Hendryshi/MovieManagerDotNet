using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Common
{
	internal class Common
	{
		public static string ReadSettingString(string keyName, string defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(string.IsNullOrEmpty(defaultValue))
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue;
			}
			return keyValue;
		}

		public static int ReadSettingInt(string keyName, int? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				int resultValue;

				if(int.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}

		public static short ReadSettingShort(string keyName, short? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				short resultValue;

				if(short.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}

		public static bool ReadSettingBool(string keyName, bool? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				bool resultValue;

				if(bool.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}
	}

	public class JavLibrary
	{
		public static string Domain
		{
			get
			{
				return Common.ReadSettingString("JavLibrary.Domain", "http://www.javlibrary.com/cn/");
			}
		}

		public static string CategoryUrl
		{
			get
			{
				return Common.ReadSettingString("JavLibrary.CategoryUrl", $"{Domain}genres.php");
			}
		}
		public static string NewReleaseUrl
		{
			get
			{
				return Common.ReadSettingString("JavLibrary.NewReleaseUrl", $"{Domain}vl_newrelease.php");
			}
		}

		

		public static bool IsTest
		{
			get
			{
				return Common.ReadSettingBool("Game.Test", false);
			}
		}

	}
}
