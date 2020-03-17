using NUnit.Framework;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Tests.Messaging
{
    [TestFixture]
    public class SqsConfigTests
    {
        SqsConfig sqsConfig;

        [SetUp]
        public void SetUp()
        {
            sqsConfig = new SqsConfig("endpoint", "queueName", "queueUrl", "accessKeyId", "accessKey", true);
        }

        [Test]
        public void Test_SqsConfig_Maps_Endpoint()
        {
            Assert.AreEqual("endpoint", sqsConfig.Endpoint);
        }

        [Test]
        public void Test_SqsConfig_Maps_QueueName()
        {
            Assert.AreEqual("queueName", sqsConfig.QueueName);
        }

        [Test]
        public void Test_SqsConfig_Maps_QueueUrl()
        {
            Assert.AreEqual("queueUrl", sqsConfig.QueueUrl);
        }

        [Test]
        public void Test_SqsConfig_Maps_AccessKeyId()
        {
            Assert.AreEqual("accessKeyId", sqsConfig.AccessKeyId);
        }

        [Test]
        public void Test_SqsConfig_Maps_AccessKey()
        {
            Assert.AreEqual("accessKey", sqsConfig.AccessKey);
        }

        [Test]
        public void Test_SqsConfig_Maps_CreateQueue()
        {
            Assert.IsTrue(sqsConfig.CreateQueue);
        }
    }
}
