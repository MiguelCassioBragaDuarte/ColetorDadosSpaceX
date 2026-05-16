using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace ColetorDadosSpaceX.Data
{
    public static class DataBase
    {
        // Define o caminho dinâmico (AppData/Local) para funcionar em qualquer PC
        private static readonly string pastaBase =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpaceXApp");

        private static readonly string caminhoBanco =
            Path.Combine(pastaBase, "spacex_data.db");

        private static readonly string connectionString =
            $"Data Source={caminhoBanco};";

        // CONSTRUTOR ESTÁTICO: Executa automaticamente na primeira vez que a classe é usada
        static DataBase()
        {
            // 1. Cria a pasta se ela não existir
            if (!Directory.Exists(pastaBase))
                Directory.CreateDirectory(pastaBase);

            // 2. Verifica se o ficheiro do banco existe. Se não existir, cria as tabelas.
            if (!File.Exists(caminhoBanco))
            {
                CriarTabelas();
            }
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        // MÉTODO DE CRIAÇÃO: Aqui é onde o banco "ganha vida"
        private static void CriarTabelas()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // Comando SQL para criar a tabela de Lançamentos
                var commandLaunch = connection.CreateCommand();
                commandLaunch.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Launch (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        DateUtc TEXT,
                        Success INTEGER, 
                        Details TEXT
                    );";
                commandLaunch.ExecuteNonQuery();

                // Comando SQL para criar a tabela de Foguetes
                var commandRocket = connection.CreateCommand();
                commandRocket.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Rocket (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        Description TEXT,
                        Active INTEGER,
                        SuccessRatePct REAL
                    );";
                commandRocket.ExecuteNonQuery();

                // Opcional: Tabela para Stats se quiserem persistir os resumos
                var commandStats = connection.CreateCommand();
                commandStats.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Stats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        TotalLaunches INTEGER,
                        SuccessfulLaunches INTEGER,
                        FailedLaunches INTEGER,
                        SuccessRate REAL
                    );";
                commandStats.ExecuteNonQuery();
            }
        }
    }
}