#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using Cauldron.Core;
using Cauldron.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Test
{
    public enum TestEnum
    {
        [DisplayName("Nom nom")]
        TheFirstValue,

        [DisplayName("Meme")]
        TheSecondValue,

        [DisplayName("A long text")]
        TheThird,

        [DisplayName("Nothing new")]
        TheLast
    }

    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void Enum_GetCustomAttribute_GetAttribute_Test()
        {
            var enumValue = TestEnum.TheSecondValue;
            var attrib = enumValue.GetCustomAttribute<DisplayNameAttribute, TestEnum>();

            Assert.AreEqual(attrib.DisplayName, "Meme");
        }

        [TestMethod]
        public void Enum_GetDisplayName_Test()
        {
            var enumValue = TestEnum.TheSecondValue;
            var attrib = enumValue.GetDisplayName();

            Assert.AreEqual(attrib, "Meme");
        }
    }
}