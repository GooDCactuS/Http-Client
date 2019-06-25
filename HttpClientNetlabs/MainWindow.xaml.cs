using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpClientNetlabs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HttpClient client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            client = new HttpClient(tb_address.Text, tb_port.Text, tb_path.Text, (bool)cb_https.IsChecked);
            List<string> list = client.Connect();
            tb_file.Text = "";
            foreach (var item in list)
            {
                tb_file.Text += "\n" + item;
            }
        }

        private void GetClick(object sender, RoutedEventArgs e)
        {
            // Регулярное выражение для поиска css-файлов
            Regex regex = new Regex(@"<link href=[""|'][^""']+[""|']  rel=""stylesheet"" type=""text/css"" />", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(tb_file.Text);
            regex = new Regex(@"<link href=[\""|']", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@"[\""|']  rel=""stylesheet"" type=""text/css"" />", RegexOptions.IgnoreCase);

            // Перенос информации из этих css-файлов в файлы на компьютере
            for (int ii = 0; ii < matches.Count; ii++)
            {
                string path = matches[ii].Value;
                path = regex.Replace(path, "");
                path = regex2.Replace(path, "");
                client.GetPageIntoFile(path, "file"+ii+".txt");
            }
            
        }
    }
}
