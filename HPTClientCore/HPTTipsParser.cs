using System.Text.RegularExpressions;

namespace HPTClient
{
    internal class HPTTipsParser
    {
        internal string IsTipsRegExString { get; set; }

        internal string ParseTipsRegExString { get; set; }

        internal string[] HorseSeparatorString { get; set; }

        internal string RaceSeparatorString { get; set; }

        internal virtual bool IsTips(string tips)
        {
            var rexIsTips = new Regex(IsTipsRegExString, RegexOptions.IgnoreCase);
            return rexIsTips.IsMatch(tips);
        }

        internal virtual bool ParseTips(string tips, HPTMarkBet markBet)
        {
            try
            {
                var rexParseTips = new Regex(this.ParseTipsRegExString, RegexOptions.IgnoreCase);
                var matches = rexParseTips.Matches(tips);
                foreach (Match m in matches)
                {
                    int legNr = Convert.ToInt32(m.Groups[1].Value);
                    var race = markBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == legNr);
                    if (race != null)
                    {
                        foreach (var horse in race.HorseList)
                        {
                            horse.SelectedFromTip = false;
                            horse.RankTip = 15;
                        }
                        int rank = 1;
                        string horseNumbersString = m.Groups[2].Value;
                        var horseNumberStrings = horseNumbersString.Split(this.HorseSeparatorString, StringSplitOptions.RemoveEmptyEntries);
                        if (horseNumberStrings.Length > 0)
                        {
                            foreach (var horseNumberString in horseNumberStrings)
                            {
                                int horseNumber = 0;
                                if (!int.TryParse(horseNumberString, out horseNumber))  // Spik med text eller "ALLA"
                                {
                                    var rexSpike = new Regex(@"\d{1,2}");
                                    if (rexSpike.IsMatch(horseNumbersString))
                                    {
                                        horseNumber = Convert.ToInt32(rexSpike.Match(horseNumbersString).Value);
                                    }
                                    else
                                    {
                                        var rexAll = new Regex("alla", RegexOptions.IgnoreCase);    // Alla hästar
                                        if (rexAll.IsMatch(horseNumbersString))
                                        {
                                            foreach (var h in race.HorseList)
                                            {
                                                h.SelectedFromTip = true;
                                            }
                                        }
                                    }
                                }
                                var horse = race.HorseList.FirstOrDefault(h => h.StartNr == horseNumber);
                                if (horse != null)
                                {
                                    horse.SelectedFromTip = true;
                                    horse.RankTip = rank++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    internal class HPTTipsParserAftonbladet : HPTTipsParser
    {
        //Avd 1: 2 For Life (3-1)
        //Avd 2: 5 New York U.S. (1-11)
        //Avd 3: 4-5-10-1 (8-9)
        //Avd 4: 5-7-6-1-12-9 (8-3)
        //Avd 5: 3-1-9-12-10-4-2-7 (6-5)
        //Avd 6: 1-8-4-9-7-11-13 (6-14)
        //Avd 7: 9 Candor Hall (10-12)
        //Avd 8: 2-11-10 (6-1)
        internal HPTTipsParserAftonbladet()
        {
            this.IsTipsRegExString = @"Avd\s(\d):";
            this.ParseTipsRegExString = @"Avd\s(\d):\s([\d\w\s\.\–-]+?)\((\d{1,2})[\–-](\d{1,2})\)";
            this.HorseSeparatorString = new string[] { "-", "–" };
            this.RaceSeparatorString = "\r\n";
        }
    }

    internal class HPTTipsParserStandard1 : HPTTipsParser
    {
        //V64-1: 8-9-12
        //V64-2: 1-2-5-8-10
        //V64-3: 5-8-10-11
        //V64-4: 1-3-6-9-10-11-12-13-14
        //V64-5: 1-9-10
        //V64-6: 1 Pampa Cato
        internal HPTTipsParserStandard1()
        {
            this.IsTipsRegExString = @"V\d{1,2}[\–-](\d):*\s([\d\w\s\.\–-]+)";
            this.ParseTipsRegExString = @"V\d{1,2}[\–-](\d):*\s([^\n]+)";
            //this.ParseTipsRegExString = @"V\d{1,2}-(\d):*\s([\d\w\s\.\–-]+)";
            this.HorseSeparatorString = new string[] { "-", "–", "," };
            this.RaceSeparatorString = "\r\n";
        }
    }

    internal class HPTTipsParserStandard2 : HPTTipsParser
    {
        //V75-1: 2, 10
        //V75-2: 1, 2, 3, 4, 7, 8
        //V75-3: 3, 7
        //V75-4: 9 Zätabubben
        //V75-5: 3, 5, 13
        //V75-6: 1, 5, 6, 10, 11
        //V75-7: 1, 2, 5, 10
        internal HPTTipsParserStandard2()
        {
            this.IsTipsRegExString = @"V\d{1,2}-(\d):*\s([\d\s,]+)";
            this.HorseSeparatorString = new string[] { "-", "–", "," };
            this.RaceSeparatorString = "\r\n";
        }
    }

    internal class HPTTipsParserHptABC : HPTTipsParser
    {
        //V64-1 A:7 B:1 C:5
        //V64-2 6
        //V64-3 A:1 B:3, 5 C:2
        //V64-4 A:6 B:3 C:12, 13, 14, 15
        //V64-5 A:5 B:1, 2, 10
        //V64-6 A:8 B:7 C:2, 9, 12
        internal HPTTipsParserHptABC()
        {
            this.IsTipsRegExString = @"V\d{1,2}-(\d)\s(((([A-F]):)*(\d{1,2},*\s*)*)*)";
            this.ParseTipsRegExString = @"V\d{1,2}-(\d)\s(((([A-F]):)*(\d{1,2},*\s*)*)*)";
            this.HorseSeparatorString = new string[] { "-", "–", "," };
            this.RaceSeparatorString = "\r\n";
        }

        internal override bool ParseTips(string tips, HPTMarkBet markBet)
        {
            try
            {
                var rexParseTips = new Regex(this.ParseTipsRegExString, RegexOptions.IgnoreCase);
                var matches = rexParseTips.Matches(tips);
                foreach (Match m in matches)
                {
                    int legNr = Convert.ToInt32(m.Groups[1].Value);
                    var race = markBet.RaceDayInfo.RaceList.FirstOrDefault(r => r.LegNr == legNr);
                    if (race != null)
                    {
                        string raceSelectedHorsesString = m.Groups[2].Value;
                        var rexMPrioHorses = new Regex(@"V\d{1,2}-(\d)\s([\d,\s]+)");
                        if (rexMPrioHorses.IsMatch(raceSelectedHorsesString))
                        {
                            Match mPrioHorses = rexMPrioHorses.Match(raceSelectedHorsesString);
                            string horseNumbersString = mPrioHorses.Groups[2].Value;
                            SelectFromString(race, HPTPrio.M, horseNumbersString);
                        }

                        var rexXPrioHorses = new Regex(@"([A-F]):([\d,\s]+)");
                        foreach (Match mXPrioHorses in rexXPrioHorses.Matches(raceSelectedHorsesString))
                        {
                            var prio = EnumHelper.GetHPTPrioFromShortString(mXPrioHorses.Groups[1].Value);
                            string horseNumbersString = mXPrioHorses.Groups[2].Value;
                            SelectFromString(race, prio, horseNumbersString);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal void SelectFromString(HPTRace race, HPTPrio prio, string horseNumbersString)
        {
            var horseNumberStrings = horseNumbersString.Split(this.HorseSeparatorString, StringSplitOptions.RemoveEmptyEntries);
            if (horseNumberStrings.Length > 0)
            {
                foreach (var horseNumberString in horseNumberStrings)
                {
                    int horseNumber = 0;
                    if (!int.TryParse(horseNumberString, out horseNumber))  // Spik med text eller "ALLA"
                    {
                        var rexSpike = new Regex(@"\d{1,2}");
                        if (rexSpike.IsMatch(horseNumbersString))
                        {
                            horseNumber = Convert.ToInt32(rexSpike.Match(horseNumbersString).Value);
                        }
                    }
                    var horse = race.HorseList.FirstOrDefault(h => h.StartNr == horseNumber);
                    if (horse != null)
                    {
                        horse.SelectedFromTip = true;
                        if (prio != HPTPrio.M)
                        {
                            horse.HorseXReductionList.First(xr => xr.Prio == prio).Selected = true;
                        }
                    }
                }
            }
        }
    }
}
