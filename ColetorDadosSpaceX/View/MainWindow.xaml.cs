using System;
using System.Windows;
using ColetorDadosSpaceX.ViewModels;

namespace ColetorDadosSpaceX
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        // Corrigido: Método que o botão "Sincronizar" estava procurando
        private async void BtnSincronizar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false;

            await _viewModel.LoadDataAsync();

            if (btn != null) btn.IsEnabled = true;
            MessageBox.Show("Dados atualizados localmente!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Corrigido: Método que o botão "Enviar para Nuvem" estava procurando
        private async void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false;

            bool sucesso = await _viewModel.EnviarDadosAoBackendAsync();

            if (btn != null) btn.IsEnabled = true;

            if (sucesso)
            {
                MessageBox.Show("Dados enviados com sucesso para a API do Aluno 2!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Não foi possível enviar os dados. Verifique se a API está online ou se há dados carregados na tela.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}