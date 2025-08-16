using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HPTClient
{
    internal class DBHelper
    {
        internal DataTable raceTable = new DataTable();
        internal DataTable markBetTable = new DataTable();

        internal void AddDataRow(HPTMarkBet markBet)
        {

        }

        internal void AddDataRow(HPTMarkBet markBet, HPTHorse horse)
        {
            var oa = new object[]
            {
            horse.ParentRace.PostTime.ToString("HH:mm"),
            horse.ParentRace.LegNr,
            horse.ParentRace.RaceNr,
            horse.StartNr,
            horse.StakeDistributionShare,
            horse.RankList.First(r => r.Name == "StakeDistributionShare").Rank,
            horse.VinnarOddsShare,
            horse.RankList.First(r => r.Name == "VinnarOdds").Rank,
            horse.PlatsOddsShare,
            horse.RankList.First(r => r.Name == "MaxPlatsOdds").Rank,
        };
            this.raceTable.LoadDataRow(oa, false);
        }

        internal static void InsertLogMessage(string logMessage, string logType)
        {
            var bulkCopy = new SqlBulkCopy(HPTSqlConnection);
            var columnMapping = new SqlBulkCopyColumnMapping();

            var command = HPTSqlCommand;
            try
            {
                command.CommandText = "RaceStatisticsInsert";

                command.Parameters.Add("@LogTime", SqlDbType.DateTime);
                command.Parameters[0].Value = DateTime.Now;

                command.Parameters.Add("@Message", SqlDbType.VarChar);
                command.Parameters[1].Value = logMessage;

                command.Parameters.Add("@LogType", SqlDbType.VarChar);
                command.Parameters[2].Value = logType;

                command.Connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                string s = exc.Message;
            }
            finally
            {
                if (command.Connection != null || command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                }
            }
        }

        #region Hjälp-properties

        private static string connectionString;
        private static SqlConnection HPTSqlConnection
        {
            get
            {
                if (connectionString == null)
                {
                    connectionString = GetConnectionString();
                }
                return new SqlConnection(connectionString);
            }
        }

        private static SqlCommand HPTSqlCommand
        {
            get
            {
                return new SqlCommand()
                {
                    Connection = HPTSqlConnection,
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 20
                };
            }
        }

        private static SqlConnection HPTSqlConnectionTEST
        {
            get
            {
                return new SqlConnection("Server=.\\SQLEXPRESS;Database=HPT50TEST;Trusted_Connection=True;");
            }
        }

        private static string GetConnectionString()
        {
            return "Server=.\\SQLEXPRESS;Database=HPT50;Trusted_Connection=True;";
        }

        #endregion
    }
}
