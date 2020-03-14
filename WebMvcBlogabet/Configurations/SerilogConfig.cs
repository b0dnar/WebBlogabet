using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;
using Serilog.Events;

namespace WebMvcBlogabet.Configurations
{
    public static class SerilogConfig
    {
        public static void Configure(String appName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", appName);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Logger(lc => lc.Filter
                    .ByIncludingOnly(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
                    .WriteTo.File(Path.Combine(path, "err_log-.log"),
                        rollingInterval: RollingInterval.Day,
                        encoding: Encoding.UTF8,
                        retainedFileCountLimit: 31, fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                )
                .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                    .WriteTo.File(Path.Combine(path, "wrn_log-.log"),
                        rollingInterval: RollingInterval.Day,
                        encoding: Encoding.UTF8,
                        retainedFileCountLimit: 31, fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                )
                .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                    .WriteTo.File(Path.Combine(path, "dbg_log-.log"),
                        rollingInterval: RollingInterval.Day,
                        encoding: Encoding.UTF8,
                        retainedFileCountLimit: 31, fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                )
                .WriteTo.File(Path.Combine(path, "vrb_log-.log"),
                    rollingInterval: RollingInterval.Hour,
                    encoding: Encoding.UTF8,
                    retainedFileCountLimit: 100, fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")

                .CreateLogger();
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
