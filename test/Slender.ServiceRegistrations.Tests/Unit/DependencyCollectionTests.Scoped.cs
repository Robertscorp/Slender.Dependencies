using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class DependencyCollectionTests
    {

        #region - - - - - - AddScoped<TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(DependencyImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddScoped<DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TImplementation> Tests

        #region - - - - - - AddScoped<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddScoped<IDependency, DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TDependency, TImplementation> Tests

        #region - - - - - - AddScoped<TDependency> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
        {
            // Arrange
            var _Factory = (DependencyFactory f) => default(IDependency);
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddScoped<IDependency>(_Factory);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TDependency> Tests

        #region - - - - - - AddScoped Tests - - - - - -

        [Fact]
        public void AddScoped_AddingDependencyWithNullConfigurationAction_RegistersDependency()
        {
            // Arrange
            var _Expected = new[] { new Dependency(typeof(IDependency)) { Lifetime = DependencyLifetime.Scoped() } };

            // Act
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddScoped_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.AddImplementationType<DependencyImplementation>());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddScoped_AddingDependencyWithAllowScanAndMatchingPreScannedImplementations_AddsDependencyAndImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Dependency>(), typeof(DependencyImplementation)));
            this.m_MockDependencyBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()));
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddScoped_AddingDependencyWithAllowScanAndNoPreScannedImplementations_AddsDependency()
        {
            // Arrange

            // Act
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            // Assert
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddScoped Tests

    }

}
