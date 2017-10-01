using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShangZhu.Tests
{
    [TestClass]
    public class ConfigiusTests
    {
        [Ignore]
        [TestMethod]
        public void Get_Should_Return_Empty()
        {
            IConfigius configius = Spirit.Connect("263a4cf4-b19c-47c7-a6bd-0c52b27641f8", "ZmVkNzk1ZTctMzdmMy00M2I3LWFkNGEtNmM1MTEyZDdiMzY5", "test", "http://localhost:51311/");
            string value = configius.Get("super_config");
        }
    }
}
