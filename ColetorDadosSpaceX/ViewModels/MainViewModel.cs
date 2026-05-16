using ColetorDadosSpaceX.Models;
using ColetorDadosSpaceX.Services;
using ColetorDadosSpaceX.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SpaceXApiService _apiService;

        // Propriedades ligadas à View (MainWindow.xaml)
        public ObservableCollection<Launch> Launches { get; set; }
        public ObservableCollection<Rocket> Rockets { get; set; }

        private Stats _launchStats;
        public Stats LaunchStats
        {
            get => _launchStats;
            set { _launchStats = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _apiService = new SpaceXApiService();
            Launches = new ObservableCollection<Launch>();
            Rockets = new ObservableCollection<Rocket>();
        }

        // Método principal chamado pelo botão da tela
        public async Task LoadDataAsync()
        {
            try
            {
                // 1. Tenta buscar os dados da API SpaceX (Lançamentos e Foguetes)
                var launchesData = await _apiService.GetLaunchesAsync();
                var rocketsData = await _apiService.GetRocketsAsync();

                // 2. Salva no banco de dados local SQLite
                SalvarLancamentosNoBancoLocal(launchesData);
                SalvarFoguetesNoBancoLocal(rocketsData);

                // 3. Atualiza a interface do usuário
                AtualizarListasNaTela(launchesData);
                AtualizarListasFoguetesNaTela(rocketsData);
            }
            catch (Exception ex)
            {
                // Se der erro de internet (API falhar), tenta carregar os dados salvos do SQLite
                Console.WriteLine($"Erro na API: {ex.Message}. Carregando dados locais...");

                var dadosLocais = CarregarLancamentosDoBancoLocal();
                AtualizarListasNaTela(dadosLocais);

                var foguetesLocais = CarregarFoguetesDoBancoLocal();
                AtualizarListasFoguetesNaTela(foguetesLocais);
            }
        }

        // --- MÉTODOS DE BANCO DE DADOS (SQLite Manual) ---

        private void SalvarLancamentosNoBancoLocal(List<Launch> launchesData)
        {
            using (var connection = DataBase.GetConnection())
            {
                connection.Open();

                // Usamos uma transação para deixar o insert em massa muito mais rápido
                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT OR REPLACE INTO Launch (Id, Name, DateUtc, Success, Details) 
                        VALUES ($id, $name, $date, $success, $details)";

                    // Adiciona os parâmetros uma vez e só muda os valores no loop
                    command.Parameters.Add("$id", SqliteType.Text);
                    command.Parameters.Add("$name", SqliteType.Text);
                    command.Parameters.Add("$date", SqliteType.Text);
                    command.Parameters.Add("$success", SqliteType.Integer);
                    command.Parameters.Add("$details", SqliteType.Text);

                    foreach (var launch in launchesData)
                    {
                        command.Parameters["$id"].Value = launch.Id;
                        command.Parameters["$name"].Value = launch.Name ?? "";
                        command.Parameters["$date"].Value = launch.DateUtc.ToString("o"); // Formato ISO para data

                        // Lida com o nulo do booleano e converte para Inteiro do SQLite (1 ou 0)
                        if (launch.Success.HasValue)
                            command.Parameters["$success"].Value = launch.Success.Value ? 1 : 0;
                        else
                            command.Parameters["$success"].Value = DBNull.Value;

                        command.Parameters["$details"].Value = launch.Details ?? "";

                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        private void SalvarFoguetesNoBancoLocal(List<Rocket> rocketsData)
        {
            using (var connection = DataBase.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT OR REPLACE INTO Rocket (Id, Name, Description, Active, SuccessRatePct) 
                        VALUES ($id, $name, $description, $active, $successRatePct)";

                    command.Parameters.Add("$id", SqliteType.Text);
                    command.Parameters.Add("$name", SqliteType.Text);
                    command.Parameters.Add("$description", SqliteType.Text);
                    command.Parameters.Add("$active", SqliteType.Integer);
                    command.Parameters.Add("$successRatePct", SqliteType.Real);

                    foreach (var rocket in rocketsData)
                    {
                        command.Parameters["$id"].Value = rocket.Id;
                        command.Parameters["$name"].Value = rocket.Name ?? "";
                        command.Parameters["$description"].Value = rocket.Description ?? "";
                        command.Parameters["$active"].Value = rocket.Active ? 1 : 0;
                        command.Parameters["$successRatePct"].Value = rocket.SuccessRatePct;

                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        private List<Launch> CarregarLancamentosDoBancoLocal()
        {
            var lista = new List<Launch>();

            using (var connection = DataBase.GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, DateUtc, Success, Details FROM Launch";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var launch = new Launch
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            DateUtc = DateTime.Parse(reader.GetString(2)),
                            Details = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };

                        if (!reader.IsDBNull(3))
                        {
                            launch.Success = reader.GetInt32(3) == 1;
                        }

                        lista.Add(launch);
                    }
                }
            }
            return lista;
        }

        private List<Rocket> CarregarFoguetesDoBancoLocal()
        {
            var lista = new List<Rocket>();

            using (var connection = DataBase.GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Description, Active, SuccessRatePct FROM Rocket";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rocket = new Rocket
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Active = reader.GetInt32(3) == 1,
                            SuccessRatePct = reader.GetDouble(4)
                        };

                        lista.Add(rocket);
                    }
                }
            }
            return lista;
        }

        // --- MÉTODOS AUXILIARES DA VIEWMODEL ---

        private void AtualizarListasNaTela(List<Launch> launchesData)
        {
            Launches.Clear();
            foreach (var launch in launchesData)
            {
                Launches.Add(launch);
            }

            CalcularEstatisticas(launchesData);
        }

        private void AtualizarListasFoguetesNaTela(List<Rocket> rocketsData)
        {
            Rockets.Clear();
            foreach (var rocket in rocketsData)
            {
                Rockets.Add(rocket);
            }
        }

        private void CalcularEstatisticas(List<Launch> launchesData)
        {
            int total = launchesData.Count;
            int successful = launchesData.Count(l => l.Success == true);
            int failed = launchesData.Count(l => l.Success == false);

            LaunchStats = new Stats
            {
                TotalLaunches = total,
                SuccessfulLaunches = successful,
                FailedLaunches = failed,
                SuccessRate = total > 0 ? (double)successful / total * 100 : 0
            };
        }

        // Implementação do INotifyPropertyChanged para atualizar a UI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}