using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class DependencyCollectionTests
    {

        #region - - - - - - AddSingleton<TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(DependencyImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton<DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TImplementation> Tests

        #region - - - - - - AddSingleton<TImplementation>(Instance) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingImplementationInstance_RegistersAsDependencyWithImplementationInstance()
        {
            // Arrange
            var _Instance = new DependencyImplementation();
            var _Expected = new[]
            {
                new Dependency(typeof(DependencyImplementation))
                {
                    ImplementationInstance = _Instance,
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton(_Instance);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TImplementation>(Instance) Tests

        #region - - - - - - AddSingleton<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton<IDependency, DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TDependency, TImplementation> Tests

        #region - - - - - - AddSingleton<TDependency>(Factory) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
        {
            // Arrange
            var _Factory = (DependencyFactory f) => default(IDependency);
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton<IDependency>(_Factory);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TDependency>(Factory) Tests

        #region - - - - - - AddSingleton<TDependency>(Instance) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingDependencyAndImplementationInstance_RegistersDependencyWithImplementationInstance()
        {
            // Arrange
            var _Instance = new DependencyImplementation();
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationInstance = _Instance,
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton<IDependency>(_Instance);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TDependency>(Instance) Tests

        #region - - - - - - AddSingleton Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingDependencyWithNullConfigurationAction_RegistersDependency()
        {
            // Arrange
            var _Expected = new[] { new Dependency(typeof(IDependency)) { Lifetime = DependencyLifetime.Singleton() } };

            // Act
            _ = this.m_DependencyCollection.AddSingleton(typeof(IDependency), null);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddSingleton_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddSingleton(typeof(IDependency), r => r.AddImplementationType<DependencyImplementation>());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton Tests

    }

}
