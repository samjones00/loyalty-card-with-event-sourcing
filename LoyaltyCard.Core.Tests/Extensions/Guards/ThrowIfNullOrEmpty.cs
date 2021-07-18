using FluentAssertions;
using LoyaltyCard.Domain.Extensions;
using NUnit.Framework;
using System;

namespace LoyaltyCard.Core.Tests.Extensions.Guards
{
    public class ThrowIfNullOrEmpty
    {
        [TestCase("")]
        [TestCase(null)]
        public void GivenNullValueShouldThrowException(string exampleString)
        {
            // Arrange            
            string variableToSet;

            // Act
            Action act = () =>
            {
                variableToSet = exampleString.ThrowIfNullOrEmpty("example");
            };

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Value cannot be null. (Parameter 'example')");
        }

        [Test]
        public void GivenValueShouldNotThrowException()
        {
            // Arrange            
            string variableToSet;
            string parameterValue = "hi";

            // Act
            Action act = () =>
            {
                variableToSet = parameterValue.ThrowIfNull("paramName");
            };

            // Assert
            act.Should().NotThrow<ArgumentNullException>();
        }
    }

}
