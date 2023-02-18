using FluentAssertions;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public class RegistrationCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly RegistrationCollection m_RegistrationCollection = new();

        #endregion Fields

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_AllServicesAndPackagesResolved_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScopedService<IService>(r => _ = r.AddImplementationType<Implementation>());
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package").ResolveRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_NotAllServicesAndPackagesResolved_ThrowsException()
        {
            // Arrange
            //_ = this.m_RegistrationCollection.AddScopedService<IService>();
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        #endregion Validate Tests

    }

    public interface IService { }

    public class Implementation : IService { }

}
