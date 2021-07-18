using FluentAssertions;
using LoyaltyCard.Domain.Extensions;
using NUnit.Framework;
using System;

namespace LoyaltyCard.Core.Tests.Extensions.Guards
{
    public interface IExampleInterface { }
    public class ExampleService : IExampleInterface { }

    public class ThrowIfNull
    {
        private IExampleInterface _exampleClass;

        [Test]
        public void GivenNullValueShouldThrowException()
        {
            // Arrange            
            IExampleInterface reference;

            _exampleClass = null;

            // Act
            Action act = () =>
            {
                reference = _exampleClass.ThrowIfNull("example");
            };

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'example')");
        }

        [Test]
        public void GivenValueShouldNotThrowException()
        {
            // Arrange            
            IExampleInterface reference;

            _exampleClass = new ExampleService();

            // Act
            Action act = () =>
            {
                reference = _exampleClass.ThrowIfNull("paramName");
            };

            // Assert
            act.Should().NotThrow<ArgumentNullException>();
        }
    }

}
