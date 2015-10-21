namespace WindowsServiceFailureHandler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using RazorEngine;
    using RazorEngine.Templating;
    using WindowsServiceFailureHandler.Models;

    public class FailureHandlerService
    {
        private const string ServiceNameArgument = "/serviceName=";
        private const string EqualString = "=";
        private const string LogContentTemplateName = "LogContentTemplate.txt";
        private const string LogContentTemplateKey = "LogContentTemplate";

        public EventLogEntry GetLatestEventEntry(string serviceName)
        {
            EventLogEntry latestLogEntry = null;
            string serviceExecutableCondition = string.Format("Application: {0}.exe", serviceName);
            string logName = "Application";

            using (var log = new EventLog(logName))
            {
                latestLogEntry = log.Entries.OfType<EventLogEntry>()
                                .Where(x => x.EntryType == EventLogEntryType.Error
                                    && (x.Source.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase)
                                        || x.ReplacementStrings.Any(r => r.Contains(serviceExecutableCondition))
                                       )
                                )
                                .OrderByDescending(x => x.TimeWritten)
                                .FirstOrDefault();
            }

            return latestLogEntry;
        }

        public string GetServiceNameArgument(string[] args)
        {
            string serviceName = string.Empty;

            if (args != null && args.Length > 0)
            {
                foreach (string argument in args)
                {
                    if (argument.StartsWith(ServiceNameArgument, StringComparison.InvariantCultureIgnoreCase))
                    {
                        serviceName = GetArgumentValue(argument);

                        break;
                    }
                }
            }

            return serviceName;
        }

        public string GetLogContent(FailureDetail content)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string logTemplatePath = Path.Combine(appPath, LogContentTemplateName);
            string logContent = string.Empty;

            if (File.Exists(logTemplatePath))
            {
                string logTemplate = File.ReadAllText(logTemplatePath);

                Engine.Razor.AddTemplate(LogContentTemplateKey, logTemplate);

                logContent = Engine.Razor.RunCompile(LogContentTemplateKey, typeof(FailureDetail), content);
            }

            return logContent;
        }

        private string GetArgumentValue(string argument)
        {
            string value = argument.Substring(argument.IndexOf(EqualString) + 1);

            return value;
        }
    }
}
