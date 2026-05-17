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

            // CORREÇÃO CRUCIAL: Retirado o 'if File.Exists'. 
            // Chamamos o método direto e o 'IF NOT EXISTS' do SQL resolve com segurança.
            CriarTabelas();
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

                // Unificamos os comandos em uma única execução para melhor performance e limpeza de código
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Launch (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        DateUtc TEXT,
                        Success INTEGER, 
                        Details TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Rocket (
                        Id TEXT PRIMARY KEY,
                        Name TEXT,
                        Description TEXT,
                        Active INTEGER,
                        SuccessRatePct REAL
                    );

                    CREATE TABLE IF NOT EXISTS Stats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        TotalLaunches INTEGER,
                        SuccessfulLaunches INTEGER,
                        FailedLaunches INTEGER,
                        SuccessRate REAL
                    );";

                command.ExecuteNonQuery();
            }
        }
    }
}