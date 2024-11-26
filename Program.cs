using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace AntiPlagiarismSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonFilePath = "hashes.json";

            // Загружаем существующие хэши и их исходные тексты из JSON файла
            Dictionary<string, string> existingHashes = LoadHashes(jsonFilePath);

            Console.WriteLine("Введите текст для хэширования (для завершения ввода введите пустую строку дважды):");

            // Считываем многострочный текст
            StringBuilder inputTextBuilder = new StringBuilder();
            string line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                inputTextBuilder.AppendLine(line);
            }
            string inputText = inputTextBuilder.ToString();

            // Разделяем текст на предложения
            string[] sentences = inputText.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            int totalSentences = sentences.Length;
            int alreadyHashedCount = 0;

            foreach (var sentence in sentences)
            {
                string trimmedSentence = sentence.Trim(); // Убираем лишние пробелы

                if (!string.IsNullOrEmpty(trimmedSentence))
                {
                    string hash = ComputeHash(trimmedSentence);

                    if (existingHashes.ContainsKey(hash))
                    {
                        // Если хэш уже существует, выводим его исходный текст
                        Console.WriteLine($"Повторяющееся предложение: \"{existingHashes[hash]}\"");
                        alreadyHashedCount++;
                    }
                    else
                    {
                        // Если хэша нет, добавляем его в список и сохраняем
                        existingHashes[hash] = trimmedSentence;
                        Console.WriteLine($"Новое предложение захэшировано: \"{trimmedSentence}\"");
                    }
                }
            }

            // Сохраняем обновленные хэши и предложения в JSON файл
            SaveHashes(jsonFilePath, existingHashes);

            // Вывод статистики
            Console.WriteLine($"\nОбщее количество предложений: {totalSentences}");
            Console.WriteLine($"Количество предложений, которые были захэшированы ранее: {alreadyHashedCount}");
        }

        // Метод для вычисления SHA256 хэша
        static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Метод для загрузки хэшей из JSON файла
        static Dictionary<string, string> LoadHashes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, string>();
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }

        // Метод для сохранения хэшей в JSON файл
        static void SaveHashes(string filePath, Dictionary<string, string> hashes)
        {
            string json = JsonConvert.SerializeObject(hashes, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}

//Не обращайте внимания сюда в эти кучу комментов я переодически закидывал текст для тестов
//
//
//
//
//
//
//
