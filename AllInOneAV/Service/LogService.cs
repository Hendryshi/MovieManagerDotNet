using Serilog;
using Serilog.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
	public class LogService
	{
		public static void ConfigureSerilog(string logName = "")
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithExceptionDetails()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File(
					String.Format(System.Configuration.ConfigurationManager.AppSettings["logDebugPath"], logName, DateTime.Now.ToString("yyyyMMddHHmm")),
					 rollingInterval: RollingInterval.Infinite
				)
				.WriteTo.File(
					String.Format(System.Configuration.ConfigurationManager.AppSettings["logErrorPath"], logName, DateTime.Now.ToString("yyyyMMddHHmm")),
					 rollingInterval: RollingInterval.Infinite,
					 restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
				)
				.CreateLogger();
		}
	}
}
