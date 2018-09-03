using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using SpysProxy.Infrastructure;

namespace SpysProxy
{
    public partial class MainWindow
    {
        private Parser _parser;
        private readonly ObservableCollection<LogItem> _dataItemsLog;
        private readonly SaveFileDialog _dlg;

        public MainWindow()
        {
            InitializeComponent();
            _dataItemsLog = new ObservableCollection<LogItem>();
            _dlg = new SaveFileDialog
            {
                DefaultExt = ".xlsx",
                Filter = "Text documents (.xlsx)|*.xlsx",
                FileName = "SpysOne"
            };
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
            if (_dlg.ShowDialog() == false) return;
            _parser = new Parser { Stop = false, FileName = _dlg.FileName };
            _parser.OnLogResult += OnLogResult;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            Task.Factory.StartNew(_parser.AllCountries);
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
