using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homework_with_string
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //путь к файлу !!!ЕЩЕ ОДИН ПУТЬ УКАЗАН В МЕТОДЕ Task3!!!!!
            var path = @"C:\Users\wladi\source\repos\Homework with string\Homework with string\";

            //строка принимает в себя данные файла.
            var readText = await ReadFileAsync(path + "sample.txt");

            //метод для подготовки строки
            LinePreparation(ref readText);

            //метод для парса на предложения
            var parse = ParseBySentence(readText);

            //запишем в файл разбитый по предложениям текст
            File.WriteAllLines(path +"Sentence.txt", parse);

            //читаем файл по предложениям
            readText = await ReadFileAsync(path + "Sentence.txt");

            //метод 3 задания
            Task3(readText,parse);

            //парсим предложения на знаки пунктуации
            parse = ParseByPunctuation(ref readText);

            //записываем в файл по пунктуации
            File.WriteAllLines(path + "Punctuation.txt", parse);

            //читаем файл разбитый по пунктуации
            readText = await ReadFileAsync(path + "Punctuation.txt");

            //парсим по тире дефисы оставляем
            parse = ParseByPunctuationDash(ref readText);

            //перезаписываем файл
            File.WriteAllLines(path + "Punctuation.txt", parse);

            //читаем файл
             readText = await ReadFileAsync(path + "Punctuation.txt");

            //разбиваем на слова удаляя все знаки и цифры
             parse = ParseByWord(ref readText);
             
            //записываем отсортированный список слов  с кол вом использования слова
            await File.WriteAllLinesAsync(path +"Sorted.txt", SortedAndCount(parse));

        }

        public static async Task<string> ReadFileAsync(string path)
        {
            //асинхронное чтение файла
            using (StreamReader reader = new StreamReader(path))
            {
                string result = await reader.ReadToEndAsync();
                return result;
            }
        }

        public static string LinePreparation(ref string str)
        {
            //удаляем спецсимволы заменив на пробелы
            char[] ch = {'@', '*', '/', '+', '&', '^', '%', '$', '#', '\\', '\n', '\"','=','_'};
            for (int i = 0; i < ch.Length; i++)
            {
                str = str.Replace(ch[i], ' ');
            }

            //меняем аббревиатуры обращения на такие же без точки что бы не делить предложения
            string[] abbreviation = {"Dr.", "Mrs.","Mr."};
            string[] abbreviationNonDot = {"Dr", "Mrs","Mr"};
            for (int i = 0; i < abbreviation.Length; i++)
            {
                str = str.Replace(abbreviation[i], abbreviationNonDot[i]);
            }

            //если есть участки где подряд один и более пробелов то заменяет их 1, т.к. спец символы мы заменили на пробелы.
            str = Regex.Replace(str, "\\s+", " ");
            return str;
        }

        public static string[] ParseBySentence(string str)
        {
            //разбиваем строку на предложения. з.ы. предложения до точки и тд. сохраняя символ
            string[] parseBySentence = Regex.Split(str, @"(?<=[\.!\?])\s+");

            //удаляем пробелы в начале и конце каждого предложения
            for (int i = 0; i < parseBySentence.Length; i++)
            {
                parseBySentence[i] = parseBySentence[i].Trim();
            }

            return parseBySentence;
        }

        public static string[] ParseByPunctuation(ref string textSentence)
        {
            string[] str = Regex.Split(textSentence, @"(?<=[\,\;\:])\s+", RegexOptions.Multiline);
            return str;
        }
        public static string[] ParseByPunctuationDash(ref string textPunctuation)
        {
            string[] str = Regex.Split(textPunctuation, @"(?<=[\-])\-", RegexOptions.Multiline);
            return str;
        }

        public static string[] ParseByWord(ref string text)
        {

            string[] words = Regex.Split(text, "[\\d\\W]+",RegexOptions.Multiline);
                
                
               // text.Split(new char[] {' ', '.', ',', '!', '?', ':', ';', '\n', '\r', '-', '(', ')', '{', '}', '[', ']','\''}, StringSplitOptions.RemoveEmptyEntries);

            //for (int i = 0; i < words.Length; i++)
            //{
            //    words[i] = words[i].Trim();
            //}

            return words;
        }

        public static HashSet<string> SortedAndCount(string[] parse)
        {
            //сортируем по алфавиту
            var orderby = parse.OrderBy(i => i).ToArray();
            // что бы посчитать кол во слов одинаковых
            var list = new List<string>();
            list.AddRange(orderby);
            var hash = new HashSet<string>();
            int count = 1;

            for (int i = 0; i < list.Count; i++)
            {
                if (hash.Contains(list[i]))
                {
                    count++;
                    hash.Add(list[i] + " - " + count);
                    list.RemoveAt(i);
                }
                else
                {
                    count = 1;
                    hash.Add(list[i]);
                }
            }

            return hash;
        }

        public static void Task3(string readText, string[] parse)
        {
            //находим самую часую букву
            char ch = default;
            int count = default;
            int x = default;
            for (int i = 0; i < readText.Length; i++)
            {
                count = 1;
                for (int j = 0; j < readText.Length; j++)
                {
                    if (ch == readText[j])
                    {
                        count++;
                    }
                }
                if (count > x)
                {
                    x = count;
                    ch = readText[i];
                }
            }

            //ищем самое длинное предложение
            string longestSentence = "";
            string shortestSentence = "                 ";
            for (int i = 0; i < parse.Length; i++)
            {
                if (parse[i].Length > longestSentence.Length)
                {
                    longestSentence = parse[i];
                }
                else if (parse[i].Length < shortestSentence.Length)
                {
                    shortestSentence = parse[i];
                }

            }
            //3 заданиe запись в файл
            using (FileStream fstream = new FileStream(@"C:\Users\wladi\source\repos\Homework with string\Homework with string\3task.txt", FileMode.Append))
            {
                // преобразуем строку в байты
                byte[] arrayLongest = System.Text.Encoding.Default.GetBytes(longestSentence);
                byte[] arrayShortest = System.Text.Encoding.Default.GetBytes(shortestSentence);
                byte[] letter = System.Text.Encoding.Default.GetBytes(ch.ToString());

                // запись массива байтов в файл
                fstream.Write(arrayLongest, 0, arrayLongest.Length);
                fstream.Write(arrayShortest, 0, arrayShortest.Length);
                fstream.Write(letter, 0, letter.Length);
            }

        }

    }
}

