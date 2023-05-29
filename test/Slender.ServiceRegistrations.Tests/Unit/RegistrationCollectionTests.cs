using FluentAssertions;
using Moq;
using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAssemblyScan> m_MockAssemblyScan = new();
        private readonly Mock<IRegistrationBehaviour> m_MockRegistrationBehaviour = new();

        private readonly List<Type> m_AssemblyTypes = new() { typeof(ServiceImplementation) };
        private readonly RegistrationCollection m_RegistrationCollection = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationCollectionTests()
        {
            _ = this.m_MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes.ToArray());

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<RegistrationContext>()))
                    .Callback((RegistrationContext context) => context.AllowScannedImplementationTypes = true);
        }

        #endregion Constructors

        #region - - - - - - AddAssemblyScan Tests - - - - - -

        [Fact]
        public void AddAssemblyScan_AddingScanWithRegistrationThatAllowsScannedImplementations_ScannedImplementationsAdded()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<RegistrationContext>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingEmptyScanWithRegistrationThatAllowsScannedImplementations_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - AddScopedService Tests - - - - - -

        [Fact]
        public void AddScopedService_AddingRegistrationWithAllowScanAndMatchingPreScannedImplementations_AddsRegistrationAndImplementations()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Act
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<RegistrationContext>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddScopedService_AddingRegistrationWithAllowScanAndNoPreScannedImplementations_AddsRegistration()
        {
            // Arrange

            // Act
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Assert
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddScopedService Tests

        #region - - - - - - ConfigureService Tests - - - - - -

        [Fact]
        public void ConfigureService_EnablingAllowScanOnRegistrationWithPreScannedImplementations_EnablesAllowScanAndAddsImplementations()
        {
            // Arrange

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IService), r => r.ScanForImplementations());

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<RegistrationContext>()));
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<RegistrationContext>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ConfigureService_EnablingAllowScanOnRegistrationWithNoPreScannedImplementations_EnablesAllowScan()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IService), r => r.ScanForImplementations());

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<RegistrationContext>()));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion ConfigureService Tests

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_AllServicesAndPackagesResolved_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => _ = r.AddImplementationType<ServiceImplementation>());
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package").ResolveRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractServiceRegisteredAsImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped<IService>();

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractServiceRegisteredAsImplementation_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped<ServiceImplementation>();

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractServiceUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractServiceUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(ServiceImplementation), r => r.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NotAllServicesAndPackagesResolved_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => { });
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        #endregion Validate Tests

    }

    public interface IService { }

    public class ServiceImplementation : IService { }

}
