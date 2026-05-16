using ColetorDadosSpaceX.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColetorDadosSpaceX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
        }

        private async void BtnSincronizar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null) btn.IsEnabled = false; // Trava o botão para não clicar 2x

            await _viewModel.LoadDataAsync();

            if (btn != null) btn.IsEnabled = true; // Destrava o botão
            MessageBox.Show("Sincronização concluída!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}