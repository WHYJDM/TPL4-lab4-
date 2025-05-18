using System;
using System.Threading.Tasks;
using TPL4.Services;

namespace TPL4
{
    class Program
    {
        // Главная точка входа в программу используется асинхронный метод Main.
        static async Task Main(string[] args)
        {
            // Отображение консольного меню
            Console.WriteLine("1. Выполнить сериализацию в 2 файла");
            Console.WriteLine("2. Объединить файлы в третий");
            Console.WriteLine("3. Асинхронно прочитать объединенный файл");
            Console.Write("Выберите пункт: ");

            // Пути к файлам, с которыми будет работать программа
            string path1 = "file1.json";
            string path2 = "file2.json";
            string merged = "merged.json";

            // Чтение пользовательского ввода и выполнение соответствующего действия
            switch (Console.ReadLine())
            {
                case "1":
                    // Генерация 20 объектов типа Drink
                    var drinks = FileHandler.GenerateDrinks();

                    // Сериализация первых 10 объектов в первый файл
                    var task1 = FileHandler.SerializeDrinksAsync(drinks.GetRange(0, 10), path1);

                    // Сериализация оставшихся 10 объектов во второй файл
                    var task2 = FileHandler.SerializeDrinksAsync(drinks.GetRange(10, 10), path2);

                    // Ожидание завершения обеих задач
                    await Task.WhenAll(task1, task2);

                    Console.WriteLine("Файлы записаны.");
                    break;

                case "2":
                    // Объединение двух файлов в третий (последовательность: file1 + file2)
                    await FileHandler.MergeFilesAsync(path1, path2, merged);
                    Console.WriteLine("Файлы объединены.");
                    break;

                case "3":
                    // Асинхронное чтение объединенного файла и вывод его содержимого на экран
                    await FileHandler.PrintMergedFileAsync(merged);
                    break;

                default:
                    // Обработка некорректного ввода
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
}
