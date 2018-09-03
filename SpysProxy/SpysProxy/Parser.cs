using HtmlAgilityPack;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SpysProxy.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace SpysProxy
{
    internal class Parser
    {
        public bool Stop { private get; set; }

        public event Action<LogItem> OnLogResult;

        private int _indexRow;

        private readonly WebClient _webClient;
        private readonly XSSFWorkbook _xssf;
        private readonly ISheet _sheet;

        public Parser()
        {
            _webClient = new WebClient();
            _xssf = new XSSFWorkbook();
            _sheet = _xssf.CreateSheet("Spys.one");
            var row = _sheet.CreateRow(_indexRow);
            row.CreateCell(0).SetCellValue("Proxy address and port");
            _sheet.AutoSizeColumn(0);
        }

        public void AllCountrys()
        {
            OnLogResult?.Invoke(new LogItem { Status = "Ok", Result = "Начинаю сканирование." });
            var html = _webClient.DownloadString("http://spys.one/proxys");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var items = htmlDocument
                .DocumentNode.Descendants("td")
                .Where(node => node.GetAttributeValue("class", "")
                    .Contains("menu")).ToArray();
            foreach (var item in items)
            {
                var url = item.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", null);
                if (url == null) continue;
                SomeFunc("http://spys.one" + url);
                OnLogResult?.Invoke(new LogItem { Status = "Ok",
                    Result = "Готова страна " +
                             $"{item.InnerText.Trim().Substring(0, item.InnerText.Trim().IndexOf(")", StringComparison.Ordinal) + 1)}" });
                if (!Stop) continue;
                OnLogResult?.Invoke(new LogItem { Status = "Warning", Result = "Сканирование остановлено." });
                break;
            }
            using (var writer = File.Create("test.xlsx"))
            {
                _xssf.Write(writer);
            }
            if (!Stop) OnLogResult?.Invoke(new LogItem { Status = "Ok",Result = "Сканирование завершено." });
        }

        private void SomeFunc(string url)
        {
            var html = _webClient.DownloadString(url);
            var algorithm = html.Substring(html.IndexOf("eval", StringComparison.Ordinal),
                html.IndexOf("}))", StringComparison.Ordinal) + 3 - html.IndexOf("eval", StringComparison.Ordinal));
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var items = htmlDocument
                .DocumentNode.Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "")
                    .Contains("spy1x")).ToArray();
            foreach (var item in items)
            {
                var someText = item.InnerText.Trim();
                var ipAddress = ParseHelper.DoRegexIp(someText);
                if(ipAddress == null) continue;
                var portCrypt = item.InnerHtml.Substring(item.InnerHtml.IndexOf("\"+(", StringComparison.Ordinal) + 2,
                    item.InnerHtml.IndexOf("))<", StringComparison.Ordinal)
                    - item.InnerHtml.IndexOf("\"+(", StringComparison.Ordinal) - 1);
                var port = ParseHelper.DoJs(algorithm, portCrypt);
                var row = _sheet.CreateRow(++_indexRow);
                row.CreateCell(0).SetCellValue(ipAddress + ':' + port);
                _sheet.AutoSizeColumn(0);
            }
        }
    }
}
