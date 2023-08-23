using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public class RegistrationBuilderTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<Action> m_MockOnScanForImplementationTypes = new();
        private readonly Mock<IRegistrationBehaviour> m_MockRegistrationBehaviour = new();

        private readonly RegistrationBuilder m_Builder = new(typeof(IService), RegistrationLifetime.Scoped());

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationBuilderTests()
        {
            _ = this.m_Builder.OnScanForImplementations = this.m_MockOnScanForImplementationTypes.Object;
            _ = this.m_Builder.Registration.Lifetime = RegistrationLifetime.Singleton();
            _ = this.m_Builder.Registration.Behaviour = this.m_MockRegistrationBehaviour.Object;
        }

        #endregion Constructors

        #region - - - - - - AddImplementationType<TImplementation> Tests - - - - - -

        [Fact]
        public void AddImplementationType_AnyGeneric_AddsImplementationTypeThroughRegistrationBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.AddImplementationType<object>();

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(this.m_Builder.Registration, typeof(object)), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddImplementationType_SameGenericAddedMultipleTimes_OnlyAddsTypeOnce()
        {
            // Arrange
            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Registration>(), It.IsAny<Type>()))
                    .Callback((Registration r, Type t) => r.ImplementationTypes.Add(t));

            // Act
            _ = this.m_Builder.AddImplementationType<object>();
            _ = this.m_Builder.AddImplementationType<object>();

            // Assert
            _ = this.m_Builder.Registration.ImplementationTypes.Should().BeEquivalentTo(new[] { typeof(object) });
        }

        #endregion AddImplementationType<TImplementation> Tests

        #region - - - - - - AddImplementationType Tests - - - - - -

        [Fact]
        public void AddImplementationType_NoTypeSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.AddImplementationType(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddImplementationType_AnyType_AddsImplementationTypeThroughRegistrationBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.AddImplementationType(typeof(object));

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(this.m_Builder.Registration, typeof(object)), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddImplementationType_SameTypeAddedMultipleTimes_OnlyAddsTypeOnce()
        {
            // Arrange
            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Registration>(), It.IsAny<Type>()))
                    .Callback((Registration r, Type t) => r.ImplementationTypes.Add(t));

            // Act
            _ = this.m_Builder.AddImplementationType(typeof(object));
            _ = this.m_Builder.AddImplementationType(typeof(object));

            // Assert
            _ = this.m_Builder.Registration.ImplementationTypes.Should().BeEquivalentTo(new[] { typeof(object) });
        }

        #endregion AddImplementationType Tests

        #region - - - - - - ScanForImplementations Tests - - - - - -

        [Fact]
        public void ScanForImplementations_OnInvocation_AllowsScanningOnRegistrationBehaviourAndScans()
        {
            // Arrange

            // Act
            _ = this.m_Builder.ScanForImplementations();

            // Assert
            this.m_MockOnScanForImplementationTypes.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(this.m_Builder.Registration), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion ScanForImplementations Tests

        #region - - - - - - WithImplementationFactory Tests - - - - - -

        [Fact]
        public void WithImplementationFactory_NoFactorySpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithImplementationFactory(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithImplementationFactory_AnyFactory_AddsFactoryThroughRegistrationBehaviour()
        {
            // Arrange
            Func<ServiceFactory, object> _Factory = serviceFactory => string.Empty;

            // Act
            _ = this.m_Builder.WithImplementationFactory(_Factory);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.UpdateImplementationFactory(this.m_Builder.Registration, _Factory), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithImplementationFactory Tests

        #region - - - - - - WithImplementationInstance Tests - - - - - -

        [Fact]
        public void WithImplementationInstance_NoInstanceSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithImplementationInstance(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithImplementationInstance_LifetimeDoesNotSupportImplementationInstances_ThrowsArgumentException()
        {
            // Arrange
            this.m_Builder.Registration.Lifetime = RegistrationLifetime.Scoped();

            // Act
            var _Expected = Record.Exception(() => this.m_Builder.WithImplementationInstance(string.Empty));

            // Assert
            _ = _Expected.Should().BeOfType<Exception>();
        }

        [Fact]
        public void WithImplementationInstance_LifetimeSupportsImplementationInstances_AddsInstanceThroughRegistrationBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.WithImplementationInstance(string.Empty);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.UpdateImplementationInstance(this.m_Builder.Registration, string.Empty), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithImplementationInstance Tests

        #region - - - - - - WithLifetime Tests - - - - - -

        [Fact]
        public void WithLifetime_NoLifetimeSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithLifetime(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithLifetime_AnyLifetime_AddsLifetimeThroughRegistrationBehaviour()
        {
            // Arrange
            var _Lifetime = RegistrationLifetime.Transient();

            // Act
            _ = this.m_Builder.WithLifetime(_Lifetime);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.UpdateLifetime(this.m_Builder.Registration, _Lifetime), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithLifetime Tests

        #region - - - - - - WithRegistration Tests - - - - - -

        [Fact]
        public void WithRegistration_NoRegistrationSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithRegistration(null, out _))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithRegistration_RegistrationIsForDifferentService_ThrowsArgumentException()
        {
            // Arrange
            var _Registration = new Registration(typeof(object));

            // Act
            var _Expected = Record.Exception(() => this.m_Builder.WithRegistration(_Registration, out _));

            // Assert
            _ = _Expected.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void WithRegistration_RegistrationIsForSameService_CreatesNewRegistrationAndReturnsOldRegistration()
        {
            // Arrange
            var _Initial = this.m_Builder.Registration;
            var _Expected = new Registration(typeof(IService))
            {
                AllowScannedImplementationTypes = true,
                Behaviour = this.m_MockRegistrationBehaviour.Object,
                ImplementationFactory = serviceFactory => string.Empty,
                ImplementationInstance = string.Empty,
                ImplementationTypes = new List<Type> { typeof(IService) },
                Lifetime = RegistrationLifetime.Transient()
            };

            // Act
            _ = this.m_Builder.WithRegistration(_Expected, out var _Actual);

            // Assert
            _ = _Actual.Should().Be(_Initial);
            _ = this.m_Builder.Registration.Should().BeEquivalentTo(_Expected);
        }

        #endregion WithRegistration Tests

        #region - - - - - - WithRegistrationBehaviour Tests - - - - - -

        [Fact]
        public void WithRegistrationBehaviour_NoBehaviourSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithRegistrationBehaviour(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithRegistrationBehaviour_AnyBehaviour_AddsBehaviourThroughRegistrationBehaviour()
        {
            // Arrange
            var _Behaviour = new Mock<IRegistrationBehaviour>().Object;

            // Act
            _ = this.m_Builder.WithRegistrationBehaviour(_Behaviour);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.UpdateBehaviour(this.m_Builder.Registration, _Behaviour), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithRegistrationBehaviour Tests

    }

}
