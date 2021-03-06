﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK;
using NCDK.Fingerprints;
using System.Collections;

namespace NCDKExcel
{
    [TestClass()]
    public class UtilityTests
    {
        [TestMethod()]
        public void ToExcelStringObjectTest()
        {
            Assert.AreEqual("#N/A", Utility.ToExcelString(double.NaN));
            Assert.AreEqual("#NUM!", Utility.ToExcelString(double.NegativeInfinity));
            Assert.AreEqual("#NUM!", Utility.ToExcelString(double.PositiveInfinity));
        }

        [TestMethod()]
        public void ToExcelStringIBitFingerprintTest()
        {
            IBitFingerprint fp;
            fp = new BitSetFingerprint(4);
            Assert.AreEqual("0000", Utility.ToExcelString(fp));
            fp = new BitSetFingerprint(new BitArray(new bool[] { true, false, true }));
            Assert.AreEqual("101", Utility.ToExcelString(fp));
        }

        [TestMethod()]
        public void ToExcelStringDescriptorValueTest()
        {
            var result = new NCDK.QSAR.Descriptors.Moleculars.ALogPDescriptor.Result(1, 2, 3);
            Assert.AreEqual("1, 2, 3", Utility.ToExcelString(result));
        }

        [TestMethod()]
        public void ParseTest()
        {
            Assert.IsNotNull(Utility.Parse("C"));
            const string InvalidValue = "qwertyuop";
            Assert.IsNull(Utility.Parse(InvalidValue));
        }

        [TestMethod()]
        public void ToMolBlockTest()
        {
            var methane = CDK.SmilesParser.ParseSmiles("C");
            var molBlock = Utility.ToMolBlock(methane);
            Assert.IsTrue(molBlock.Contains("M  END"));
        }
    }
}
