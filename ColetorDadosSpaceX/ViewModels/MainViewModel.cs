using ColetorDadosSpaceX.Models;
using ColetorDadosSpaceX.Services;
using Microsoft.Data.Sqlite;    // Onde está a sua classe DataBase
using ColetorDadosSpaceX.Models;
using ColetorDadosSpaceX.Services; // Onde está o SpaceXApiService
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ColetorDadosSpaceX.Data;

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
                // 1. Tenta buscar os dados da API SpaceX
                var launchesData = await _apiService.GetLaunchesAsync();

                // 2. Salva no banco de dados local SQLite
                SalvarLancamentosNoBancoLocal(launchesData);

                // 3. Atualiza a interface do usuário
                AtualizarListasNaTela(launchesData);
            }
            catch (Exception ex)
            {
                // Se der erro de internet (API falhar), tenta carregar os dados salvos do SQLite
                Console.WriteLine($"Erro na API: {ex.Message}. Carregando dados locais...");
                var dadosLocais = CarregarLancamentosDoBancoLocal();
                AtualizarListasNaTela(dadosLocais);
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