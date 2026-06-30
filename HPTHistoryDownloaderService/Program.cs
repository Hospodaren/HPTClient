using HPTHistoryDownloaderService;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<HPTHistoryDownloaderWorker>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "HPT History Downloader";
});
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    // Define your job
    var jobKey = new JobKey("DownloadHistoryJob");
    q.AddJob<DownloadHistoryJob>(opts => opts.WithIdentity(jobKey));
    
    // Create a trigger that runs every 5 minutes
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DownloadHistoryTrigger")
        .StartAt(DateTime.Today.AddHours(8));
});

var host = builder.Build();
host.Run();