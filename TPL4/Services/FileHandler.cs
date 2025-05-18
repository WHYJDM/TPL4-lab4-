// Подключение необходимых пространств имён
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TPL4.Models;

namespace TPL4.Services
{
    // Статический класс, предоставляющий методы для работы с файлами и объектами Drink
    public static class FileHandler
    {
        // Семафор для синхронизации доступа к файлу при многопоточности
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        // Метод генерации списка напитков (20 штук)
        public static List<Drink> GenerateDrinks()
        {
            var drinks = new List<Drink>();
            for (int i = 1; i <= 20; i++)
            {
                drinks.Add(new Drink
                {
                    Id = i,
                    Name = $"Drink{i}",
                    SerialNumber = $"SN{i:0000}",
                    Type = i % 2 == 0 ? "Газированный" : "Без газа",
                    Manufacturer = new Manufacturer
                    {
                        Name = $"Manufacturer{i}",
                        Address = $"Address{i}",
                        IsAChildCompany = i % 2 == 0
                    }
                });
            }
            return drinks;
        }

        // Асинхронная сериализация списка напитков в JSON-файл
        public static async Task SerializeDrinksAsync(List<Drink> drinks, string filePath)
        {
            // Создание потока записи
            using FileStream stream = File.Create(filePath);
            // Запись объекта в файл в формате JSON
            await JsonSerializer.SerializeAsync(stream, drinks, new JsonSerializerOptions { WriteIndented = true });
        }

        // Асинхронная десериализация JSON-файла в список напитков
        public static async Task<List<Drink>> DeserializeDrinksAsync(string filePath)
        {
            // Открытие потока для чтения
            using FileStream stream = File.OpenRead(filePath);
            // Чтение и преобразование содержимого файла в список объектов
            return await JsonSerializer.DeserializeAsync<List<Drink>>(stream);
        }

        // Асинхронное объединение двух файлов с напитками в один
        public static async Task MergeFilesAsync(string file1, string file2, string mergedFile)
        {
            // Асинхронное чтение данных из двух файлов
            var task1 = DeserializeDrinksAsync(file1);
            var task2 = DeserializeDrinksAsync(file2);

            // Ожидание завершения обоих операций чтения
            await Task.WhenAll(task1, task2);

            // Объединение всех напитков в один список
            var allDrinks = new List<Drink>();
            allDrinks.AddRange(task1.Result);
            allDrinks.AddRange(task2.Result);

            // Синхронизированный доступ к файлу записи
            await semaphore.WaitAsync();
            try
            {
                await SerializeDrinksAsync(allDrinks, mergedFile); // Сериализация объединенного списка
            }
            finally
            {
                semaphore.Release(); // Освобождение блокировки
            }
        }

        // Асинхронная печать содержимого объединённого файла в консоль
        public static async Task PrintMergedFileAsync(string mergedFile)
        {
            var drinks = await DeserializeDrinksAsync(mergedFile); // Чтение объединённого файла
            var tasks = new List<Task>();

            // Параллельный вывод каждого напитка в отдельной задаче
            foreach (var drink in drinks)
            {
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine($"{drink.Id}: {drink.Name} ({drink.Type}), Производитель: {drink.Manufacturer.Name}");
                }));
            }

            // Ожидание завершения всех задач
            await Task.WhenAll(tasks);
        }
    }
}
