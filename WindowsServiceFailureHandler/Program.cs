namespace WindowsServiceFailureHandler
{
    using System;
    using System.Diagnostics;
    using WindowsServiceFailureHandler.Models;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var service = new FailureHandlerService();
                string serviceName = service.GetServiceNameArgument(args);

                if (!string.IsNullOrWhiteSpace(serviceName))
                {
                    var failureDetail = new FailureDetail
                    {
                        ServiceName = serviceName
                    };

                    EventLogEntry latestLogEntry = service.GetLatestEventEntry(serviceName);

                    if (latestLogEntry != null)
                    {
                        failureDetail.ErrorMessage = latestLogEntry.Message;
                    }

                    string logContent = service.GetLogContent(failureDetail);

                    Fanex.Logging.LogService.Error(logContent);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.ToString());
            }
        }
    }
}
