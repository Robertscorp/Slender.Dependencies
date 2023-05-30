using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - AddSingleton<TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingImplementationAsService_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(ServiceImplementation))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton<ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TImplementation> Tests

        #region - - - - - - AddSingleton<TImplementation>(Instance) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingImplementationInstance_RegistersAsServiceWithImplementationInstance()
        {
            // Arrange
            var _Instance = new ServiceImplementation();
            var _Expected = new[]
            {
                new Registration(typeof(ServiceImplementation))
                {
                    ImplementationInstance = _Instance,
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton(_Instance);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TImplementation>(Instance) Tests

        #region - - - - - - AddSingleton<TService, TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingServiceAndImplementation_RegistersServiceWithImplementation()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton<IService, ServiceImplementation>();

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TService, TImplementation> Tests

        #region - - - - - - AddSingleton<TService>(Factory) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingServiceWithImplementationFactory_RegistersServiceWithImplementationFactory()
        {
            // Arrange
            var _Factory = (ServiceFactory f) => default(IService);
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton<IService>(_Factory);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TService>(Factory) Tests

        #region - - - - - - AddSingleton<TService>(Instance) Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingServiceAndImplementationInstance_RegistersServiceWithImplementationInstance()
        {
            // Arrange
            var _Instance = new ServiceImplementation();
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationInstance = _Instance,
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton<IService>(_Instance);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton<TService>(Instance) Tests

        #region - - - - - - AddSingleton Tests - - - - - -

        [Fact]
        public void AddSingleton_AddingServiceWithNullConfigurationAction_RegistersService()
        {
            // Arrange
            var _Expected = new[] { new Registration(typeof(IService)) { Lifetime = RegistrationLifetime.Singleton() } };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton(typeof(IService), null);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddSingleton_AddingServiceWithConfigurationAction_RegistersServiceAndInvokesConfigurationAction()
        {
            // Arrange
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    ImplementationTypes = new List<Type> { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddSingleton(typeof(IService), r => r.AddImplementationType<ServiceImplementation>());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddSingleton Tests

    }

}
