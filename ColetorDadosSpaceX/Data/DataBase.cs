using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace ColetorDadosSpaceX.Data
{
    public static class DataBase
    {
        private static readonly string pastaBase =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpaceXApp");

        private static readonly string caminhoBanco =
            Path.Combine(pastaBase, "spacex_data.db");

        private static readonly string connectionString =
            $"Data Source={caminhoBanco};";

        static DataBase()
        {
           
            if (!Directory.Exists(pastaBase))
                Directory.CreateDirectory(pastaBase);

            if (!File.Exists(caminhoBanco))
            {
                CriarTabelas();
            }
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        private static void CriarTabelas()
        {
            using (var connection = GetConnection())
            {
                connection.Open();


                var commandLaunch = connection.CreateCommand();
                commandLaunch.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Launch (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        DateUtc TEXT,
                        Success INTEGER, -- SQLite não tem booleano, usamos INTEGER (0 ou 1)
                        Details TEXT
                    );";
                commandLaunch.ExecuteNonQuery();


                var commandRocket = connection.CreateCommand();
                commandRocket.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Rocket (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        Description TEXT,
                        Active INTEGER, -- SQLite não tem booleano, usamos INTEGER (0 ou 1)
                        SuccessRatePct REAL
                    );";
                commandRocket.ExecuteNonQuery();
            }
        }
    }
}