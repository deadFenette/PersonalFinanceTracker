using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PersonalFinanceTracker.Data
{
    /// <summary>
    /// Очень простая загрузка данных из JSON файла
    /// </summary>
    public class FileDataService
    {
        private readonly string _filePath = "finance_data.json";


        public void SaveData(List<Wallet> wallets)
        {
            try
            {
                // Превращаем наши объекты в JSON строку
                string json = JsonSerializer.Serialize(wallets, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });

                // Записываем строку в файл
                File.WriteAllText(_filePath, json);

                Console.WriteLine("1 Данные успешно сохранены в файл!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"0 Ошибка при сохранении: {ex.Message}");
            }
        }

        public List<Wallet> LoadData()
        {
            try
            {
                // Проверяем существует ли файл
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("Файл с данными не найден. Будет создан новый.");
                    return new List<Wallet>(); // Возвращаем пустой список
                }

                // Читаем весь файл
                string json = File.ReadAllText(_filePath);

                // Превращаем JSON обратно в объекты
                List<Wallet> wallets = JsonSerializer.Deserialize<List<Wallet>>(json);

                Console.WriteLine("Данные успешно загружены из файла!");
                return wallets ?? new List<Wallet>(); // Если null, возвращаем пустой список
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                return new List<Wallet>();
            }
        }

        /// <summary>
        /// Проверяем есть ли сохраненные данные
        /// </summary>
        public bool HasSavedData()
        {
            return File.Exists(_filePath);
        }
    }
}