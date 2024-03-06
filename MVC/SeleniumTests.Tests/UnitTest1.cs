using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace seleniumTest.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //string url = "http://localhost:49198/Clients/Create";
            //string url = "https://localhost:44373";
            string url = "https://localhost:44316";
            //string url = "http://localhost:49198";
            ChromeDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.LinkText("Client")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.Id("Name")).SendKeys("Marge Simpson");
            driver.FindElement(By.Id("Address")).SendKeys("Springfield");
            driver.FindElement(By.XPath("//Input[@type='submit']")).Click();
        }
    }
}
