using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using SpysProxy.Infrastructure;

namespace SpysProxy
{
    public partial class MainWindow
    {
        private Parser _parser;
        private readonly ObservableCollection<LogItem> _dataItemsLog = new ObservableCollection<LogItem>();

        public MainWindow()
        {
            InitializeComponent();
            DataGridLog.ItemsSource = _dataItemsLog;
        }

        private async void OnLogResult(LogItem logItem)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => _dataItemsLog.Insert(0, logItem)));
            if (logItem.Result != "Сканирование остановлено.") return;
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                StartButton.IsEnabled = true;
            }));
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _parser = new Parser();
            _parser.OnLogResult += OnLogResult;
            _parser.Stop = false;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            Task.Factory.StartNew(_parser.AllCountrys);
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            _parser.Stop = true;
            StopButton.IsEnabled = false;
        }

        private void LaunchSpysParserOnGitHub(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/mazanuj/SpysProxy");
        }
    }
}
