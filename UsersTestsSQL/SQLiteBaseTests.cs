using Microsoft.VisualStudio.TestTools.UnitTesting;
using Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Tests
{
    [TestClass()]
    public class SQLiteBaseTests
    {
        [TestMethod()]
        public void GetLocationsByNameTest()
        {
            int? location_id = SQLiteBase.GetLocationsByName("Deskin");
            Assert.AreEqual(3, location_id);
        }
    }
}