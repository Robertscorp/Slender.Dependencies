using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - AddScoped<TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingImplementationAsService_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(ServiceImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddScoped<ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TImplementation> Tests

        #region - - - - - - AddScoped<TService, TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingServiceAndImplementation_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddScoped<IService, ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TService, TImplementation> Tests

        #region - - - - - - AddScoped<TService> Tests - - - - - -

        [Fact]
        public void AddScoped_AddingServiceWithImplementationFactory_RegistersServiceWithImplementationFactory()
        {
            // Arrange
            var _Factory = (ServiceFactory f) => default(IService);
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddScoped<IService>(_Factory);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped<TService> Tests

        #region - - - - - - AddScoped Tests - - - - - -

        [Fact]
        public void AddScoped_AddingServiceWithNullConfigurationAction_RegistersService()
        {
            // Arrange
            var _Expected = new[] { new Registration(typeof(IService)) { Lifetime = RegistrationLifetime.Scoped() } };

            // Act
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService));

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddScoped_AddingServiceWithConfigurationAction_RegistersServiceAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.AddImplementationType<ServiceImplementation>());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddScoped Tests

    }

}
