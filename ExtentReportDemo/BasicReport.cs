using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using Gurock.TestRail;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace ExtentReportDemo
{
    [TestFixture]
    public class BasicReport
    {
        public ExtentReports extent;
        public ExtentTest test;
        public APIClient client;
        public int testCaseID = 0;

        [OneTimeSetUp]
        protected void Setup()
        {
            client = new APIClient("https://rajeshpractice.testrail.io");
            client.User = "rajesh.yemul@gmail.com";
            client.Password = "testrail@1234";
            //var dir = TestContext.CurrentContext.TestDirectory + "\\";
            //var fileName = this.GetType().ToString() + ".html";
            //var htmlReporter = new ExtentHtmlReporter(dir + fileName);

            string path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string actualPath = path.Substring(0, path.LastIndexOf("bin"));
            string projectPath = new Uri(actualPath).LocalPath;
            string reportPath = projectPath + "Reports\\ExtentStepLogs.html";

            var htmlReporter = new ExtentHtmlReporter(reportPath);

            htmlReporter.Configuration().ChartVisibilityOnOpen = true;
            htmlReporter.Configuration().DocumentTitle = "Rajesh - Yemul";
            htmlReporter.Configuration().ChartLocation = ChartLocation.Top;


            extent = new ExtentReports();

            extent.AddSystemInfo("Host Name", "Rajesh");
            extent.AddSystemInfo("Environment", "QA");
            extent.AddSystemInfo("User", "Raj");

            extent.AttachReporter(htmlReporter);


        }

        [OneTimeTearDown]
        protected void TearDown()
        {
            extent.Flush();
        }


        [Test]
        public void DemoReportPass()
        {
            test = extent.CreateTest("DemoReportPass");
            Assert.IsTrue(true);
        }

        [Test]
        public void DemoReportFail()
        {
            test = extent.CreateTest("DemoReportFail");
            Assert.IsTrue(false);
        }


        [TearDown]
        public void AfterTest()
        {

            int testStatus = 5;
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                    ? ""
                    : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
            Status logstatus;
               
            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    testStatus = 5;
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    logstatus = Status.Pass;
                    testStatus = 1;
                    break;
            }

            test.Log(logstatus, "Test ended with " + logstatus + stacktrace);
            extent.Flush();

            var data = new Dictionary<string, object>
            {
                { "status_id", testStatus },
                { "comment", "This test worked fine!" }
            };


            JObject c = (JObject)client.SendGet("get_test/2");
            Console.WriteLine(c["title"]);

            JObject r = (JObject)client.SendPost("add_result_for_case/2/3", data);
        }
    }
}
