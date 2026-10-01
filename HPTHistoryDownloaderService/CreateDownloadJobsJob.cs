using Quartz;
using Microsoft.Extensions.Logging;

namespace HPTHistoryDownloaderService
{
    public class CreateDownloadJobsJob : IJob
    {
        private readonly ILogger<CreateDownloadJobsJob> _logger;

        public CreateDownloadJobsJob(ILogger<CreateDownloadJobsJob> logger)
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
