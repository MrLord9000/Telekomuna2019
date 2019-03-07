using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelekomunikacjaZadanie1;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelekomunikacjaZadanie1.Tests
{
    [TestClass()]
    public class BitMatrixTests
    {
        [TestMethod()]
        public void BitMatrixTest()
        {
            // Arrange =================================
            BitMatrix testMatrix = new BitMatrix(10, 10);

            // Act =====================================
            testMatrix[0, 0] = true;
            testMatrix[7, 4] = true;

            // Assert ==================================
            Assert.AreEqual(true, testMatrix[0, 0]);
            Assert.AreEqual(true, testMatrix[7, 4]);
            Assert.AreEqual(false, testMatrix[0, 1]);
            Assert.AreEqual(false, testMatrix[1, 0]);
        }

        [TestMethod()]
        public void GetRowTest()
        {
            // Arrange =================================
            BitMatrix testMatrix = new BitMatrix(6, 4);
            BitArray testArray = new BitArray(4);

            // Act =====================================
            testMatrix[0, 0] = true;
            testMatrix[0, 1] = true;
            testMatrix[0, 2] = true;

            testArray[0] = true;
            testArray[1] = true;
            testArray[2] = true;

            // Assert ==================================
            Assert.AreEqual(testArray[0], testMatrix.GetRow(0)[0]);
            Assert.AreEqual(testArray[1], testMatrix.GetRow(0)[1]);
            Assert.AreEqual(testArray[2], testMatrix.GetRow(0)[2]);
            Assert.AreEqual(testArray[3], testMatrix.GetRow(0)[3]);

        }

        [TestMethod()]
        public void PrintTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void PrintRowTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void PrintRowTest1()
        {
            //Assert.Fail();
        }
    }
}