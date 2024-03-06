using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading;

namespace seleniumTest.Tests
{
    [TestClass]
    public class ClientCrudTest
    {
        private IWebDriver driver;
        private string baseUrl = "https://localhost:44316";

        [TestInitialize]
        public void SetUp()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(baseUrl);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public void TestClientCRUD()
        {
            // Navigate to Client Index and Create a new Client
            driver.FindElement(By.LinkText("Client")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("Name")).SendKeys("Marge Simpson");
            driver.FindElement(By.Id("Address")).SendKeys("Springfield");
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Client in Index
            Assert.IsTrue(driver.PageSource.Contains("Marge Simpson"), "Client not found in Index after creation.");

            // Edit the Client
            driver.FindElement(By.LinkText("Edit")).Click();
            var nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys("Edited Marge Simpson");
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Client Name Change in Index
            driver.FindElement(By.LinkText("Client")).Click();
            Assert.IsTrue(driver.PageSource.Contains("Edited Marge Simpson"), "Client name not updated in Index view.");

            // Delete the Client
            driver.FindElement(By.LinkText("Delete")).Click();
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Deletion
            driver.FindElement(By.LinkText("Client")).Click();
            Assert.IsFalse(driver.PageSource.Contains("Edited Marge Simpson"), "Client was not removed from Index view.");
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}

