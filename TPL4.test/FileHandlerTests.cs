using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TPL4.Models;
using TPL4.Services;

namespace TPL4.Tests
{
    [TestClass]
    public class FileHandlerTests
    {
        private const string TestFile1 = "test1.json";
        private const string TestFile2 = "test2.json";
        private const string MergedFile = "testMerged.json";

        [TestMethod]
        public async Task TestSerializationAndDeserialization()
        {
            var drinks = FileHandler.GenerateDrinks();
            await FileHandler.SerializeDrinksAsync(drinks, TestFile1);

            Assert.IsTrue(File.Exists(TestFile1));

            var deserialized = await FileHandler.DeserializeDrinksAsync(TestFile1);
            Assert.AreEqual(drinks.Count, deserialized.Count);
        }

        [TestMethod]
        public async Task TestMergeFilesAsync()
        {
            var drinks = FileHandler.GenerateDrinks();
            await FileHandler.SerializeDrinksAsync(drinks.GetRange(0, 10), TestFile1);
            await FileHandler.SerializeDrinksAsync(drinks.GetRange(10, 10), TestFile2);

            await FileHandler.MergeFilesAsync(TestFile1, TestFile2, MergedFile);

            var mergedDrinks = await FileHandler.DeserializeDrinksAsync(MergedFile);
            Assert.AreEqual(20, mergedDrinks.Count);
        }
    }
}
