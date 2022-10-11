﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Health.FitOnFhir.Common.Interfaces;
using Microsoft.Health.FitOnFhir.Common.Providers;
using Microsoft.Health.FitOnFhir.Common.Services;
using Microsoft.Health.FitOnFhir.Common.Tests.Mocks;
using Microsoft.Health.FitOnFhir.GoogleFit.Common;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;
using QueueMessage = Microsoft.Health.FitOnFhir.Common.Models.QueueMessage;

namespace Microsoft.Health.FitOnFhir.Common.Tests
{
    public class QueueServiceTests
    {
        private IQueueClientProvider _queueClientProvider;
        private QueueClient _queueClient;
        private MockLogger<QueueService> _queueServiceLogger;
        private IQueueService _queueService;
        private readonly string _expectedUserId;

        public QueueServiceTests()
        {
            _expectedUserId = Guid.NewGuid().ToString();
            _queueClient = Substitute.For<QueueClient>();
            _queueClientProvider = Substitute.For<IQueueClientProvider>();
            _queueClientProvider.GetQueueClient(Arg.Any<string>()).Returns(_queueClient);
            _queueServiceLogger = Substitute.For<MockLogger<QueueService>>();

            _queueService = new QueueService(_queueClientProvider, _queueServiceLogger);
        }

        protected string ExpectedUserId => _expectedUserId;

        protected string ExpectedPlatformUserId => "me";

        protected string ExpectedPlatformName => GoogleFitConstants.GoogleFitPlatformName;

        protected string ExpectedMessageText => JsonConvert.SerializeObject(new QueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName));

        [Fact]
        public async Task GivenQueueExists_WhenInitQueueCalled_NoQueueIsCreated()
        {
            string loggerMsg = "Queue import-data created";
            _queueClient.CreateIfNotExistsAsync().ReturnsNull();

            await _queueService.SendQueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName, CancellationToken.None);

            _queueServiceLogger.DidNotReceive().Log(
                Arg.Is<LogLevel>(lvl => lvl == LogLevel.Information),
                Arg.Is<string>(msg => msg == loggerMsg));

            await _queueClient.Received(1).CreateIfNotExistsAsync();
        }

        [Fact]
        public async Task GivenQueueDoesNotExist_WhenInitQueueCalled_QueueIsCreated()
        {
            string loggerMsg = "Queue import-data created";
            _queueClient.CreateIfNotExistsAsync().Returns(Substitute.For<Response>());

            await _queueService.SendQueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName, CancellationToken.None);

            _queueServiceLogger.Received(1).Log(
                Arg.Is<LogLevel>(lvl => lvl == LogLevel.Information),
                Arg.Is<string>(msg => msg == loggerMsg));

            await _queueClient.Received(1).CreateIfNotExistsAsync();
        }

        [Fact]
        public async Task GivenCreateIfNotExistsAsyncThrowsRequestFailedException_WhenInitQueueCalled_ExceptionThrown()
        {
            _queueClient.CreateIfNotExistsAsync().Throws(new InvalidDataException());

            await Assert.ThrowsAsync<InvalidDataException>(() => _queueService.SendQueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName, CancellationToken.None));
        }

        [Fact]
        public async Task GivenSendMessageAsyncThrowsException_WhenSendQueueMessageCalled_ExceptionIsCaughtAndLogged()
        {
            _queueClient.SendMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Throws(new UnauthorizedAccessException());
            _queueClient.CreateIfNotExistsAsync().Returns(Substitute.For<Response>());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _queueService.SendQueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName, CancellationToken.None));
        }

        [Fact]
        public async Task GivenNoConditions_WhenSendQueueMessageCalled_SendMessageAsyncIsCalled()
        {
            _queueClient.SendMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Substitute.For<Response<SendReceipt>>());
            _queueClient.CreateIfNotExistsAsync().Returns(Substitute.For<Response>());

            await _queueService.SendQueueMessage(ExpectedUserId, ExpectedPlatformUserId, ExpectedPlatformName, CancellationToken.None);

            await _queueClient.Received(1).SendMessageAsync(Arg.Is<string>(str => str == ExpectedMessageText), Arg.Any<CancellationToken>());
        }
    }
}
