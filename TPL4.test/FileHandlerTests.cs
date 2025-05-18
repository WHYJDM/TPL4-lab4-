// Подключаем необходимые пространства имён
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TPL4.Models;
using TPL4.Services;

namespace TPL4.Tests
{
    // Класс для модульных тестов
    [TestClass]
    public class FileHandlerTests
    {
        // Пути к временным файлам, используемым в тестах
        private const string TestFile1 = "test1.json";
        private const string TestFile2 = "test2.json";
        private const string MergedFile = "testMerged.json";

        // Тест проверки сериализации и десериализации
        [TestMethod]
        public async Task TestSerializationAndDeserialization()
        {
            // Генерация тестовых данных (список напитков)
            var drinks = FileHandler.GenerateDrinks();

            // Сохраняем их в файл
            await FileHandler.SerializeDrinksAsync(drinks, TestFile1);

            // Проверяем, что файл действительно создан
            Assert.IsTrue(File.Exists(TestFile1));

            // Считываем данные из файла обратно в объект
            var deserialized = await FileHandler.DeserializeDrinksAsync(TestFile1);

            // Сравниваем количество записей до и после, чтобы убедиться, что сериализация прошла корректно
            Assert.AreEqual(drinks.Count, deserialized.Count);
        }

        // Тест слияния двух файлов
        [TestMethod]
        public async Task TestMergeFilesAsync()
        {
            // Генерация тестовых данных
            var drinks = FileHandler.GenerateDrinks();

            // Сериализуем первую половину в первый файл
            await FileHandler.SerializeDrinksAsync(drinks.GetRange(0, 10), TestFile1);

            // Сериализуем вторую половину во второй файл
            await FileHandler.SerializeDrinksAsync(drinks.GetRange(10, 10), TestFile2);

            // Выполняем слияние двух файлов в один
            await FileHandler.MergeFilesAsync(TestFile1, TestFile2, MergedFile);

            // Десериализуем объединённый файл
            var mergedDrinks = await FileHandler.DeserializeDrinksAsync(MergedFile);

            // Проверяем, что итоговый файл содержит 20 записей
            Assert.AreEqual(20, mergedDrinks.Count);
        }
    }
}
