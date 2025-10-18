using ATGDownloader;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATGDownloader
{
    public class DBHelper
    {
        // Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;
        private const string _connectionString = @"Server=.\SQLEXPRESS;Database=HPTLight;Trusted_Connection=True;TrustServerCertificate=true;";

        public static ATGGameBase SaveGame(ATGGameInfoBase gameInfo, string jsonData)
        {
            var game = ATGJsonParser.ParseGameJson(jsonData, gameInfo);

            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "[dbo].[SaveGame]";
            sqlCommand.Parameters.AddWithValue("@atgGameId", gameInfo.Id);
            sqlCommand.Parameters.AddWithValue("@code", gameInfo.Code);
            sqlCommand.Parameters.AddWithValue("@date", gameInfo.StartTime.Date);
            sqlCommand.Parameters.AddWithValue("@track1", gameInfo.Tracks.FirstOrDefault().TrackId);
            sqlCommand.Parameters.AddWithValue("@track2", gameInfo.Tracks.Length > 1 ? gameInfo.Tracks.LastOrDefault().TrackId : null);
            sqlCommand.Parameters.AddWithValue("@status", gameInfo.Status);
            sqlCommand.Parameters.AddWithValue("@timestamp", game.ATGTimestamp);
            sqlCommand.Parameters.AddWithValue("@version", game.Version);
            sqlCommand.Parameters.AddWithValue("@jsonData", jsonData);

            int dbId = Convert.ToInt32(sqlCommand.ExecuteScalar());

            return game;
        }

        private static SqlCommand GetSaveGameCommand()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "[dbo].[SaveGame]";
            return new SqlCommand();
        }
    }
}
