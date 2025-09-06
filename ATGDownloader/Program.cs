using ATGDownloader;

Enumerable.Range(-100, 100)
    .ToList()
    .ForEach(i =>
{
    DateTime dateToDownload = DateTime.Today.AddDays(i);
    if (!FileHelper.FileExistForDate(dateToDownload))
    {
        var jsonCalendar = ATGDownloadHelper.DownloadCalendar(dateToDownload);
        var gameInfoBase = ATGJsonParser.ParseCalendar(jsonCalendar);
        string jsonGameData = ATGDownloadHelper.DownloadGameInfo(gameInfoBase.Id);
        var gameInfo = ATGJsonParser.ParseGameJson(jsonGameData, gameInfoBase);

        FileHelper.SaveFile(gameInfoBase, jsonGameData);
        DBHelper.SaveGame(gameInfoBase, jsonGameData);
    }
});


//var parser = new ATGJsonParser();
//var gameInfo = parser.ParseCalendarFile(@"Json\Calendar.json");
//var game = parser.ParseGameFile(@"Json\GameV75.json", gameInfo);

//DownloadHelper.DownloadCalendar(DateTime.Today);
//Console.WriteLine("Hello, World!");
