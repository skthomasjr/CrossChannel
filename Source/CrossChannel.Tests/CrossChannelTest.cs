using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossChannel.Tests
{
    [TestClass]
    public class CrossChannelTest
    {
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(2750);
        private const string channelName = "Test";
        private readonly IChannel localTestChannel = new Channel(channelName);
        private readonly IChannel meshTestChannel = new Channel(channelName, ChannelMode.Mesh);

        [TestMethod]
        public void WhenSendingAndReceivingSimpleMessages_WithLocalChannels_ExpectMessagesToBeReceived()
        {
            const string outboundMessage = "Ping";
            var inboundMessage = SendAndReceiveMessage(outboundMessage, localTestChannel, localTestChannel);

            Assert.AreEqual(outboundMessage, inboundMessage);
        }

        [TestMethod]
        public void WhenSendingAndReceivingSimpleMessages_WithMeshChannels_ExpectMessagesToBeReceived()
        {
            const string outboundMessage = "Ping";
            var inboundMessage = SendAndReceiveMessage(outboundMessage, meshTestChannel, meshTestChannel);

            Assert.AreEqual(outboundMessage, inboundMessage);
        }

        [TestMethod]
        public void WhenSendingSimpleMessages_WithNoReceivers_ExpectMessagesToSendWithoutException()
        {
            const string outboundMessage = "Ping";
            var sender = new BroadcastSender<string>();
            sender.Open(localTestChannel);
            sender.SendAsync(outboundMessage).Wait(timeout);
        }

        [TestMethod]
        public void WhenSendingAndReceivingSimpleMessages_WithMismatchedLocalChannel_ExpectMessagesToBeLost()
        {
            const string outboundMessage = "Ping";
            var inboundMessage = SendAndReceiveMessage(outboundMessage, localTestChannel, new Channel("wrong", localTestChannel.Mode));

            Assert.AreNotEqual(outboundMessage, inboundMessage);
        }

        [TestMethod]
        public void WhenSendingAndReceivingSimpleMessages_WithMismatchedMeshChannel_ExpectMessagesToBeLost()
        {
            const string outboundMessage = "Ping";
            var inboundMessage = SendAndReceiveMessage(outboundMessage, meshTestChannel, new Channel("wrong", meshTestChannel.Mode));

            Assert.AreNotEqual(outboundMessage, inboundMessage);
        }

        private TMessage SendAndReceiveMessage<TMessage>(TMessage message, IChannel sendChannel, IChannel receiveChannel)
        {
            var autoResetEvent = new AutoResetEvent(false);
            var inboundMessage = default(TMessage);

            using (var receiver = new BroadcastReceiver<TMessage>())
            {
                receiver.Open(receiveChannel);
                receiver.MessageReceived = messageReceived =>
                {
                    inboundMessage = messageReceived;
                    autoResetEvent.Set();
                };

                var sender = new BroadcastSender<TMessage>();
                sender.Open(sendChannel);
                sender.ExceptionThrown = (message1, exception) =>
                {
                    autoResetEvent.Set();
                };
                sender.SendAsync(message).Wait(timeout);
            }

            autoResetEvent.WaitOne(timeout);
            return inboundMessage;
        }
    }
}