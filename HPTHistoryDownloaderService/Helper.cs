namespace HPTHistoryDownloaderService;

public class Helper
{
    private void UpdateTrendsFromATG()
    {
        try
        {
            // var gameBase = ATGDownloader.ATGObjectGetter.UpdateGame(MarkBet.BetType.GameInfoBase);
            // string fileName = HPTSerializer.SerializeHPTRaceDayInfoHistory(gameBase, MarkBet.SaveDirectory);
            // ChangeUpdateTimer();
        }
        catch (Exception exc)
        {
            string s = exc.Message;
        }
    }

    internal IEnumerable<DateTime> CreateTriggerTimes(DateTime startTime, DateTime endTime, int numberOfTimes, double factor)
    {
        var totalSeconds = (startTime - endTime).TotalSeconds;
        var totalSum = Enumerable.Range(1, numberOfTimes)
            .Select(n => Math.Pow(factor, n))
            .Sum();

        var secondsToAdd = totalSeconds / totalSum;
        var triggerTimes = new List<DateTime>();
        var timeToAdd = endTime;

        while (timeToAdd >= startTime)
        {
            triggerTimes.Add(timeToAdd);
            secondsToAdd *= factor;
            timeToAdd = timeToAdd.AddSeconds(secondsToAdd);
        }
        return triggerTimes;
    }
}