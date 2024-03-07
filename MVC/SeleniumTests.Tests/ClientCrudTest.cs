using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
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

        [TestMethod, Priority(0)]
        public void TestClientCreation()
        {
            // Navigate to Client Index and Create a new Client
            driver.FindElement(By.LinkText("Client")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("Name")).SendKeys("Marge Simpson");
            driver.FindElement(By.Id("Address")).SendKeys("Springfield");
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Client in Index
            Assert.IsTrue(driver.PageSource.Contains("Marge Simpson"), "Client not found in Index after creation.");
        }

        [TestMethod, Priority(1)]
        public void TestClientUpdate()
        {
            // Navigate to Client's Edit page
            driver.FindElement(By.LinkText("Client")).Click();
            // This requires identifying the correct Edit link for "Marge Simpson"; implementation may vary
            driver.FindElement(By.LinkText("Edit")).Click();
            var nameField = driver.FindElement(By.Id("Name"));
            nameField.Clear();
            nameField.SendKeys("Edited Marge Simpson");
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Client Name Change in Index
            driver.FindElement(By.LinkText("Client")).Click();
            Assert.IsTrue(driver.PageSource.Contains("Edited Marge Simpson"), "Client name not updated in Index view.");
        }

        [TestMethod, Priority(2)]
        public void TestClientDeletion()
        {
            // Navigate to Client's Delete page
            driver.FindElement(By.LinkText("Client")).Click();
            // This requires identifying the correct Delete link for "Edited Marge Simpson"; implementation may vary
            driver.FindElement(By.LinkText("Delete")).Click();
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();

            // Verify Deletion
            driver.FindElement(By.LinkText("Client")).Click();
            Assert.IsFalse(driver.PageSource.Contains("Edited Marge Simpson"), "Client was not removed from Index view.");
        }

        [TestMethod, Priority(3)]
        public void TestCreateAndDeleteMultipleClients()
        {
            // Create Client #1
            CreateClient("Client One", "Address One");

            // Create Client #2
            CreateClient("Client Two", "Address Two");

            // Ensure both clients are present in the index view
            Assert.IsTrue(IsClientPresentInIndex("Client One"), "Client One not found in Index after creation.");
            Assert.IsTrue(IsClientPresentInIndex("Client Two"), "Client Two not found in Index after creation.");

            // Perform the deletion of Client #2
            DeleteClient("Client Two");

            // Add a short wait to ensure any asynchronous operations related to the deletion have completed
            Thread.Sleep(2000); // Consider using WebDriverWait for specific conditions instead of Thread.Sleep

            // Refresh the page to ensure it reflects the most current state
            driver.Navigate().Refresh();

            // Re-check the presence of Client #2 in the index view
            Assert.IsFalse(IsClientPresentInIndex("Client Two"), "Client Two was not removed from Index view.");
        }

        private void CreateClient(string name, string address)
        {
            driver.FindElement(By.LinkText("Client")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();
            driver.FindElement(By.Id("Name")).SendKeys(name);
            driver.FindElement(By.Id("Address")).SendKeys(address);
            driver.FindElement(By.XPath("//input[@type='submit']")).Click();
        }

        private bool IsClientPresentInIndex(string clientName)
        {
            driver.FindElement(By.LinkText("Client")).Click();
            return WaitForElement(By.XPath($"//td[contains(text(), '{clientName}')]"), 10);
        }

        private void DeleteClient(string clientName)
        {
            driver.FindElement(By.LinkText("Client")).Click();
            if (WaitForElementToBeClickable(By.XPath($"//tr[td[contains(text(), '{clientName}')]]/td/a[contains(text(),'Delete')]"), 10))
            {
                var deleteLinkForClientTwo = driver.FindElement(By.XPath($"//tr[td[contains(text(), '{clientName}')]]/td/a[contains(text(),'Delete')]"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", deleteLinkForClientTwo);
                Thread.Sleep(500); // Allow time for any overlays to disappear or the page to adjust

                deleteLinkForClientTwo.Click();
                if (WaitForElementToBeClickable(By.XPath("//input[@type='submit']"), 5))
                {
                    driver.FindElement(By.XPath("//input[@type='submit']")).Click();
                    // Wait for deletion confirmation or for the client to no longer be present in the index
                    Thread.Sleep(2000); // Adjust based on your app's response time
                }
            }
            else
            {
                Assert.Fail($"Delete link for '{clientName}' was not clickable.");
            }
        }

        private bool WaitForElement(By by, int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            try
            {
                wait.Until(drv => drv.FindElement(by).Displayed);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        private bool WaitForElementToBeClickable(By by, int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            try
            {
                wait.Until(drv =>
                {
                    var element = drv.FindElement(by);
                    return element != null && element.Displayed && element.Enabled;
                });
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}

