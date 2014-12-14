using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLP_Data;
using MLP_Data.Entity;
using MLP_Data.Enums;

namespace Unit_Tests
{
    [TestClass]
    public class UnitTestsDataFrequencyFilter
    {
        [TestMethod]
        public void TestMethodDataFrequencyFilter()
        {
            var collection = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                collection.Add((double)i);
            }

            List<TestCase> result = CasesCreator.Create(collection,
                InputDataDateUnits.Week,
                InputDataDateUnits.Day).ToList();

            Assert.AreEqual(result.Count, 95);
            Assert.AreEqual(result[0].InputRawData.Count, 5);
            Assert.AreEqual(result[0].InputRawData[0], 95d);
            Assert.AreEqual(result[0].InputRawData[1], 96d);
            Assert.AreEqual(result[0].InputRawData[4], 99d);
            Assert.AreEqual(result[0].OutputRawData, 94d);
            Assert.AreEqual(result[94].InputRawData.Count, 5);
            Assert.AreEqual(result[94].InputRawData[0], 1d);
            Assert.AreEqual(result[94].InputRawData[1], 2d);
            Assert.AreEqual(result[94].InputRawData[4], 5d);
            Assert.AreEqual(result[94].OutputRawData, 0d);
            Assert.AreEqual(result[0].OutputInPreviousCase[0], 95d);
            Assert.AreEqual(result[94].OutputInPreviousCase[0], 1d);
        }

        [TestMethod]
        public void TestMethodDataFrequencyFilter2()
        {
            var collection = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                collection.Add((double)i);
            }

            List<TestCase> result = CasesCreator.Create(collection,
                InputDataDateUnits.Month,
                InputDataDateUnits.Week,
                InputDataDateUnits.Week,
                false).ToList();

            Assert.AreEqual(result.Count, 16);
            Assert.AreEqual(result[0].InputRawData.Count, 4);
            Assert.AreEqual(result[0].InputRawData[0], 1d);
            Assert.AreEqual(result[0].InputRawData[1], 6d);
            Assert.AreEqual(result[0].InputRawData[3], 16d);
            Assert.AreEqual(result[0].OutputRawData, 0d);
            Assert.AreEqual(result[15].InputRawData.Count, 4);
            Assert.AreEqual(result[15].InputRawData[0], 76d);
            Assert.AreEqual(result[15].InputRawData[1], 81d);
            Assert.AreEqual(result[15].InputRawData[3], 91d);
            Assert.AreEqual(result[15].OutputRawData, 75d);
            Assert.AreEqual(result[0].OutputInPreviousCase[0], 1d);
            Assert.AreEqual(result[15].OutputInPreviousCase[0], 76d);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void TestMethodDataFrequencyFilterArgumentException()
        {
            var collection = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                collection.Add((double)i);
            }

            CasesCreator.Create(collection,
                InputDataDateUnits.Year,
                InputDataDateUnits.Day).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMethodDataFrequencyFilterArgumentException2()
        {
            var collection = new List<object>();
            for (int i = 0; i < 100; i++)
            {
                collection.Add((double)i);
            }

            CasesCreator.Create(collection,
                InputDataDateUnits.Month,
                InputDataDateUnits.ThreeMonths).ToList();
        }

        [TestMethod]
        public void DebugToSeeHowItWorks()
        {
            var result = CasesCreator.Create(CsvReader.GetData(IndexName.SP500, typeof(StockExchangeListing)), 
                InputDataDateUnits.Week,
                InputDataDateUnits.Day,
                InputDataDateUnits.Day,
                false).ToList();
        }
    }
}
