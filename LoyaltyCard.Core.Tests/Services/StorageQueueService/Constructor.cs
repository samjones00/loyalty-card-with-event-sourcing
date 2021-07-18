using Azure.Storage.Queues;
using FluentAssertions;
using LoyaltyCard.Core.Services;
using LoyaltyCard.Domain.Interfaces;
using Moq;
using NUnit.Framework;
using System;

namespace LoyaltyCard.Core.Tests.Services.StorageQueueServiceTests
{
    public class Constructor
    {
        [Test]
        public void GivenNoNullParametersShouldNotThrow()
        {
            // Arrange & Act
            Action act = () => new StorageQueueService(Mock.Of<ISerializer>(), Mock.Of<QueueClient>());

            // Assert
            act.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void GivenNullSerializerShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new StorageQueueService(null, Mock.Of<QueueClient>());

            // Assert
            act.Should().Throw<ArgumentNullException>()
              .WithMessage("Value cannot be null. (Parameter 'serializer')");
        }

        [Test]
        public void GivenNullQueueClientShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new StorageQueueService(Mock.Of<ISerializer>(), null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
              .WithMessage("Value cannot be null. (Parameter 'queueClient')");
        }
    }
}
