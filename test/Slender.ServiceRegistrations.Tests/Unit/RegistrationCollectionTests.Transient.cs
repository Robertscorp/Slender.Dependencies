using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - AddTransient<TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingImplementationAsService_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(ServiceImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Transient()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddTransient<ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TImplementation> Tests

        #region - - - - - - AddTransient<TService, TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingServiceAndImplementation_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Transient()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddTransient<IService, ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TService, TImplementation> Tests

        #region - - - - - - AddTransient<TService> Tests - - - - - -

        [Fact]
        public void AddTransient_AddingServiceWithImplementationFactory_RegistersServiceWithImplementationFactory()
        {
            // Arrange
            var _Factory = (ServiceFactory f) => default(IService);
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = RegistrationLifetime.Transient()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddTransient<IService>(_Factory);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient<TService> Tests

        #region - - - - - - AddTransient Tests - - - - - -

        [Fact]
        public void AddTransient_AddingServiceWithNullConfigurationAction_RegistersService()
        {
            // Arrange
            var _Expected = new[] { new Registration(typeof(IService)) { Lifetime = RegistrationLifetime.Transient() } };

            // Act
            _ = this.m_RegistrationCollection.AddTransient(typeof(IService));

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddTransient_AddingServiceWithConfigurationAction_RegistersServiceAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Transient()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddTransient(typeof(IService), r => r.AddImplementationType<ServiceImplementation>());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransient Tests

    }

}
