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
		public static void ConfigureSerilog()
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithExceptionDetails()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File(
					System.Configuration.ConfigurationManager.AppSettings["logDebugPath"],
					 rollingInterval: RollingInterval.Day
				)
				.WriteTo.File(
					System.Configuration.ConfigurationManager.AppSettings["logErrorPath"],
					 rollingInterval: RollingInterval.Day,
					 restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
				)
				.CreateLogger();
		}
	}
}
