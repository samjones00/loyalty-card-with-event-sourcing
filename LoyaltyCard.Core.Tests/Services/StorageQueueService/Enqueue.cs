using AutoFixture;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using LoyaltyCard.Core.Services;
using LoyaltyCard.Domain.Contracts.Customer;
using LoyaltyCard.Domain.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoyaltyCard.Core.Tests
{
    public class Enqueue
    {
        private IQueueService _queueService;
        private Mock<ISerializer> _mockSerializer;
        private Mock<QueueClient> _mockQueueClient;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _mockSerializer = new Mock<ISerializer>();
            _mockSerializer.Setup(x => x.SerializeToJson(It.IsAny<object>())).Returns("jsonValue");

            _mockQueueClient = new Mock<QueueClient>(MockBehavior.Strict);
            _mockQueueClient.Setup(x => x.CreateIfNotExistsAsync(null, new CancellationToken()).Result).Returns(CreateResponse());
            _mockQueueClient.Setup(x => x.ExistsAsync(new CancellationToken()).Result).Returns(CreateResponse(true));
            _mockQueueClient.Setup(x => x.SendMessageAsync(It.IsAny<string>()).Result).Returns(CreateResponse<SendReceipt>());

            _queueService = new StorageQueueService(_mockSerializer.Object, _mockQueueClient.Object);
        }

        [Test]
        public async Task GivenQueueExistsShouldAddCreateCustomerMessageToQueue()
        {
            var message = new CreateCustomer
            {
                CustomerId = _fixture.Create<Guid>(),
            };

            var result = await _queueService.Enqueue(message);

            result.HasValue.Should().BeTrue();
            result.Value.Type.Should().Be(nameof(CreateCustomer));
        }

        [Test]
        public async Task GivenQueueDoesntExistsShouldReturnEmptyResult()
        {
            _mockQueueClient.Setup(x => x.ExistsAsync(new CancellationToken()).Result).Returns(CreateResponse(false));
            
            var message = new CreateCustomer
            {
                CustomerId = _fixture.Create<Guid>(),
            };

            var result = await _queueService.Enqueue(message);

            result.HasValue.Should().BeFalse();
            result.Value.Should().BeNull();
        }

        [Test]
        public void GivenNullContractShouldThrowArgumentNullException()
        {
            var message = new CreateCustomer
            {
                CustomerId = _fixture.Create<Guid>(),
            };

            _queueService.Invoking(y => y.Enqueue<CreateCustomer>(null))
                .Should().Throw<ArgumentNullException>();
        }

        public Response<bool> CreateResponse(bool value)
        {
            var response = new Mock<Response<bool>>();
            response.Setup(x => x.Value).Returns(value);

            return response.Object;
        }

        public Response<T> CreateResponse<T>() => new Mock<Response<T>>().Object;

        public Response CreateResponse() => new Mock<Response>().Object;
    }
}