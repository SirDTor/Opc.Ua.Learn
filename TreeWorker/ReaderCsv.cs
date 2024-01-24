using System.Collections.ObjectModel;
using System.Text;

namespace TreeWorker
{
    public static class ReaderCsv
    {
        /// <summary>
        /// Поле хранящее путь до файла конфигурации
        /// </summary>
        private static string? _path = @"E:\Projects\КонвертерIFix\ConverterIFix\тестовые данные\Сервер истории (History).csv";

        /// <summary>
        /// Задает или возвращает значение пути
        /// </summary>
        public static string? Path
        {
            get => _path;
            set => _path = value;
        }

        /// <summary>
        /// Метод считывания тегов с файла конфигурации
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<NodeInfo>> ReadTagsFromFileAsync()
        {
            ObservableCollection<NodeInfo> project = new ObservableCollection<NodeInfo>();
            if (Path != null)
            {
                using (StreamReader sr = new StreamReader(Path, Encoding.UTF8))
                {
                    string historyCSV = await sr.ReadLineAsync();
                    while (historyCSV != null)
                    {
                        historyCSV = await sr.ReadLineAsync();
                        if (historyCSV != null)
                        {
                            string[]? splitHistoryCSV = historyCSV.Split(',');
                            if (splitHistoryCSV[1] == "101")
                            {
                                NodeInfo integrityTags = new NodeInfo(splitHistoryCSV[0], splitHistoryCSV[3], "");
                                project.Add(integrityTags);
                            }
                            else switch (splitHistoryCSV[1])
                                {
                                    case "1":
                                        NodeInfo integrityTags = new(splitHistoryCSV[0], "", "");
                                        project.Add(integrityTags);
                                        break;
                                    default: continue;
                                }
                        }
                    }
                    sr.Close();
                }
                return project;
            }
            else return project;
        }
    }
}

