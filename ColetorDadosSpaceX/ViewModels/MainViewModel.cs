using ColetorDadosSpaceX.Models;
using ColetorDadosSpaceX.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly SpaceXApiService _apiService;

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

        // Método que você pode chamar no carregamento da tela ou em um botão "Atualizar"
        public async Task LoadDataAsync()
        {
            var launchesData = await _apiService.GetLaunchesAsync();
            var rocketsData = await _apiService.GetRocketsAsync();

            Launches.Clear();
            foreach (var launch in launchesData) Launches.Add(launch);

            Rockets.Clear();
            foreach (var rocket in rocketsData) Rockets.Add(rocket);

            CalculateStats(launchesData);
        }

        private void CalculateStats(List<Launch> launchesData)
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
