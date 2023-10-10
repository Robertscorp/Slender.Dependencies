using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class DependencyCollectionTests
    {

        #region - - - - - - AddTransient<TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(DependencyImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Transient()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddTransient<DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TImplementation> Tests

        #region - - - - - - AddTransient<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Transient()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddTransient<IDependency, DependencyImplementation>();

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TDependency, TImplementation> Tests

        #region - - - - - - AddTransient<TDependency> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
        {
            // Arrange
            var _Factory = (DependencyFactory f) => default(IDependency);
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = DependencyLifetime.Transient()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddTransient<IDependency>(_Factory);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TDependency> Tests

        #region - - - - - - AddTransient Tests - - - - - -

        [Fact]
        public void AddTransient_AddingDependencyWithNullConfigurationAction_RegistersDependency()
        {
            // Arrange
            var _Expected = new[] { new Dependency(typeof(IDependency)) { Lifetime = DependencyLifetime.Transient() } };

            // Act
            _ = this.m_DependencyCollection.AddTransient(typeof(IDependency));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddTransient_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    ImplementationTypes = new List<Type> { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Transient()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddTransient(typeof(IDependency), d => d.HasImplementationType<DependencyImplementation>());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient Tests

    }

}
