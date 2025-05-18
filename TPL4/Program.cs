using System;
using System.Threading.Tasks;
using TPL4.Services;

namespace TPL4
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("1. Выполнить сериализацию в 2 файла");
            Console.WriteLine("2. Объединить файлы в третий");
            Console.WriteLine("3. Асинхронно прочитать объединенный файл");
            Console.Write("Выберите пункт: ");

            string path1 = "file1.json";
            string path2 = "file2.json";
            string merged = "merged.json";

            switch (Console.ReadLine())
            {
                case "1":
                    var drinks = FileHandler.GenerateDrinks();
                    var task1 = FileHandler.SerializeDrinksAsync(drinks.GetRange(0, 10), path1);
                    var task2 = FileHandler.SerializeDrinksAsync(drinks.GetRange(10, 10), path2);
                    await Task.WhenAll(task1, task2);
                    Console.WriteLine("Файлы записаны.");
                    break;

                case "2":
                    await FileHandler.MergeFilesAsync(path1, path2, merged);
                    Console.WriteLine("Файлы объединены.");
                    break;

                case "3":
                    await FileHandler.PrintMergedFileAsync(merged);
                    break;

                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
}