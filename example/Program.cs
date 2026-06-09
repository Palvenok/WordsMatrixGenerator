using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using WordGameLogic;

namespace ConsoleTest
{

    class Program
    {
        const string FILE_PATH = "russian-mnemonic-words.txt";
        private static List<string> dictionary = new List<string>();


        private static int width = 20;
        private static int height = 20;

        static void Main(string[] args)
        {
            Generator generator = new Generator();

            if (!TryLoadDictionary()) return;


            Console.WriteLine($"Генерация сетки {width}x{height}...");

            List<WordData> placedWords;
            char[,] grid;

            if (generator.TryGenerate(width, height, dictionary, out grid, out placedWords))
            {
                // Успех!
            }
            else
            {
                Console.WriteLine("Генерация заняла слишком много времени, пробуем снова...");
            }

            if (grid != null)
            {
                PrintGrid(grid, width, height, placedWords);

                Console.WriteLine("\nСписок вписанных слов:");
                foreach (var word in placedWords)
                {
                    Console.WriteLine($"- {word.Word} (Длина: {word.Path.Count})");
                }
                Console.WriteLine($"Всего слов: {placedWords.Count}");
            }
            else
            {
                Console.WriteLine("Ошибка: Не удалось заполнить сетку на 100%.");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static bool TryLoadDictionary()
        {
            // 1. Проверяем наличие файла и читаем слова
            if (!File.Exists(FILE_PATH))
            {
                Console.WriteLine($"Ошибка: Файл {FILE_PATH} не найден!");
                return false;
            }

            // Читаем все строки, убираем пробелы и пустые строки
            foreach (var line in File.ReadAllLines(FILE_PATH))
            {
                string trimmed = line.Trim().ToUpper();
                if (!string.IsNullOrEmpty(trimmed)) dictionary.Add(trimmed);
            }

            if (dictionary.Count == 0)
            {
                Console.WriteLine("Словарь пуст!");
                return false;
            }

            return true;
        }

        static void PrintGrid(char[,] grid, int w, int h, List<WordData> words)
        {
            // Создаем массив цветов для наглядности (только для консоли)
            ConsoleColor[] colors = {
                ConsoleColor.White, ConsoleColor.Magenta, ConsoleColor.Yellow,
                ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Cyan,
                ConsoleColor.Blue, ConsoleColor.DarkBlue
            };

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Определяем, какому слову принадлежит буква, чтобы раскрасить
                    var owner = words.FindIndex(word => word.Path.Exists(p => p.X == x && p.Y == y));

                    Console.ForegroundColor = owner >= 0 ? colors[owner % colors.Length] : ConsoleColor.Gray;
                    Console.Write(grid[x, y] + " ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
}
