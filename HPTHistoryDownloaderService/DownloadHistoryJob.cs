using Quartz;
using Microsoft.Extensions.Logging;

namespace HPTHistoryDownloaderService
{
    public class DownloadHistoryJob : IJob
    {
        private readonly ILogger<DownloadHistoryJob> _logger;

        public DownloadHistoryJob(ILogger<DownloadHistoryJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Starting history download job at {Timestamp}", DateTime.UtcNow);
                
                // Your existing download logic would go here
                
                _logger.LogInformation("History download job completed at {Timestamp}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in history download job");
                throw;
            }
        }
    }
}
