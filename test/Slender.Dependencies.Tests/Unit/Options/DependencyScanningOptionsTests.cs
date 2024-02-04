using FluentAssertions;
using Slender.Dependencies.Options;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit.Options
{

    public class DependencyScanningOptionsTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly DependencyScanningOptions m_Options = new();

        #endregion Fields

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_OnUnregisteredDependencyTypeFoundIsSet_DoesNotThrowException()
        {
            // Arrange
            this.m_Options.OnUnregisteredDependencyTypeFound = (dependencies, dependencyType) => { };

            // Act
            var _Exception = Record.Exception(() => this.m_Options.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_OnUnregisteredImplementationTypeFoundIsSet_DoesNotThrowException()
        {
            // Arrange
            this.m_Options.OnUnregisteredImplementationTypeFound = (dependency, implementationType) => { };

            // Act
            var _Exception = Record.Exception(() => this.m_Options.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_OnUnregisteredTypeFoundIsSet_DoesNotThrowException()
        {
            // Arrange
            this.m_Options.OnUnregisteredTypeFound = (dependencies, type) => { };

            // Act
            var _Exception = Record.Exception(() => this.m_Options.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_NoRegistrationActionsSet_ThrowsException()
            => Record.Exception(() => this.m_Options.Validate()).Should().BeOfType<Exception>();

        #endregion Validate Tests

    }
}
