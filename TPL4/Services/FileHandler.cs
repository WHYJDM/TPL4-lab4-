using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TPL4.Models;

namespace TPL4.Services
{
    public static class FileHandler
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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

        public static async Task SerializeDrinksAsync(List<Drink> drinks, string filePath)
        {
            using FileStream stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, drinks, new JsonSerializerOptions { WriteIndented = true });
        }

        public static async Task<List<Drink>> DeserializeDrinksAsync(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<List<Drink>>(stream);
        }

        public static async Task MergeFilesAsync(string file1, string file2, string mergedFile)
        {
            var task1 = DeserializeDrinksAsync(file1);
            var task2 = DeserializeDrinksAsync(file2);

            await Task.WhenAll(task1, task2);
            var allDrinks = new List<Drink>();
            allDrinks.AddRange(task1.Result);
            allDrinks.AddRange(task2.Result);

            await semaphore.WaitAsync();
            try
            {
                await SerializeDrinksAsync(allDrinks, mergedFile);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task PrintMergedFileAsync(string mergedFile)
        {
            var drinks = await DeserializeDrinksAsync(mergedFile);
            var tasks = new List<Task>();

            foreach (var drink in drinks)
            {
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine($"{drink.Id}: {drink.Name} ({drink.Type}), Производитель: {drink.Manufacturer.Name}");
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}