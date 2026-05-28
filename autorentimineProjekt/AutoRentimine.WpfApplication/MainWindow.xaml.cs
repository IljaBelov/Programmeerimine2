using System.Windows;

namespace AutoRentimine.WpfApplication
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Создаем экземпляр нашей ViewModel
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // Подписываемся на событие полной загрузки окна (Пункт 7)
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Асинхронно стягиваем данные с сервера при старте приложения
            await _viewModel.LoadDataAsync();
        }
    }
}