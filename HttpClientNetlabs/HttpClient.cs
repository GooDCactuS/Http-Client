using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace HttpClientNetlabs
{
    public class HttpClient
    {
        private string address;
        private string port;
        private string path;
        private bool https;

        public HttpClient(string address, string port, string path, bool https)
        {
            this.address = address;
            this.port = port;
            this.path = path;
            this.https = https;
        }

        public List<String> Connect()
        {
            string uri = "";
            // При номера порта = 443 устанавливаем https соединение
            if (https || port == "443")
            {
                uri = "https" + "://" + address + "/" + path;
            }
            else 
            {
                if (port == "")
                    uri += "http://" + address + "/" + path;
                else
                    uri += "http://" + address + ":" + port + "/" + path;

            }
            List<string> list = new List<string>();
            try
            {
                // Создаем запрос на получение файла, таймаут - 7 секунд
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 7000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Читаем тело ответа
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                        list.Add(reader.ReadLine());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return list;
            
           
            
        }

        public void GetIMGIntoFile(string path, string fileName)
        {
            PutIntoFile(fileName, GetIMG(path));
        }

        private string GetIMG(string pagePath)
        {
            // Папка, в которой лежит изначальный файл
            int index = this.path.LastIndexOf('/');
            string temp = path.Substring(0, index);
            string uri = "";

            int n = 4;
            // Регулярные выражения для всех видов путей к файлам
            Regex[] regex = new Regex[n];
            regex[0] = new Regex(@"^[.]{1}[/]{1}", RegexOptions.IgnoreCase);
            regex[1] = new Regex(@"^[.]{2}[/]{1}", RegexOptions.IgnoreCase);
            regex[2] = new Regex(@"^[/]{1}("+temp+")", RegexOptions.IgnoreCase);
            regex[3] = new Regex(@"^[/]{2}", RegexOptions.IgnoreCase);

            bool flag = true;

            // Установка абсолютных адресов до файлов
            for (int ii = 0; ii < n; ii++)
            {
                if(regex[ii].IsMatch(pagePath))
                {
                    pagePath = regex[ii].Replace(pagePath, "");
                    if (https)
                    {
                        uri = uri + "https";
                    }
                    else
                    {
                        uri = uri + "http";
                    }
                    if (ii == 1)
                        uri += "://" + address + "/" + pagePath;
                    else if (ii == 2)
                        uri += "://" + address + "/" + temp + pagePath;
                    else if (ii == 3)
                        uri += "://" + pagePath;
                    else
                        uri += "://" + address + "/" + temp + "/" + pagePath;
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                Regex regex1 = new Regex(@"^(http|https)", RegexOptions.IgnoreCase);
                if (regex1.IsMatch(pagePath))
                    uri = pagePath;
                else
                {
                    if (https)
                    {
                        uri = uri + "https";
                    }
                    else
                    {
                        uri = uri + "http";
                    }
                    uri += "://" + address + "/" + temp + "/" + pagePath;
                }
            }

            
            // Запрос на получение файла, таймаут - 10 сек.
            string text = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
               
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            return text;
        }

        private void PutIntoFile(string path, string text)
        {
            // Запись строки в файл
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = Encoding.GetEncoding(1251).GetBytes(text);
                fstream.Write(array, 0, array.Length);
            }
        }
    }
}
