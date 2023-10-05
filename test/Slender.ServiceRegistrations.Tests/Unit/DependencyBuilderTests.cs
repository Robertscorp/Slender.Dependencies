using FluentAssertions;
using Moq;
using Slender.ServiceRegistrations.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public class DependencyBuilderTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IDependencyBehaviour> m_MockDependencyBehaviour = new();
        private readonly Mock<Action> m_MockOnScanForImplementationTypes = new();

        private readonly DependencyBuilder m_Builder = new(typeof(IDependency));

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public DependencyBuilderTests()
        {
            _ = this.m_Builder.OnScanForImplementations = this.m_MockOnScanForImplementationTypes.Object;
            _ = this.m_Builder.Dependency.Lifetime = TestDependencyLifetime.Instance(true);
            _ = this.m_Builder.Dependency.Behaviour = this.m_MockDependencyBehaviour.Object;
        }

        #endregion Constructors

        #region - - - - - - AddImplementationType<TImplementation> Tests - - - - - -

        [Fact]
        public void AddImplementationType_AnyGeneric_AddsImplementationTypeThroughBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.AddImplementationType<object>();

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AddImplementationType(this.m_Builder.Dependency, typeof(object)), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddImplementationType_SameGenericAddedMultipleTimes_OnlyAddsTypeOnce()
        {
            // Arrange
            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Dependency>(), It.IsAny<Type>()))
                    .Callback((Dependency d, Type t) => d.ImplementationTypes.Add(t));

            // Act
            _ = this.m_Builder.AddImplementationType<object>();
            _ = this.m_Builder.AddImplementationType<object>();

            // Assert
            _ = this.m_Builder.Dependency.ImplementationTypes.Should().BeEquivalentTo(new[] { typeof(object) });
        }

        #endregion AddImplementationType<TImplementation> Tests

        #region - - - - - - AddImplementationType Tests - - - - - -

        [Fact]
        public void AddImplementationType_NoTypeSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.AddImplementationType(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddImplementationType_AnyType_AddsImplementationTypeThroughBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.AddImplementationType(typeof(object));

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AddImplementationType(this.m_Builder.Dependency, typeof(object)), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddImplementationType_SameTypeAddedMultipleTimes_OnlyAddsTypeOnce()
        {
            // Arrange
            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Dependency>(), It.IsAny<Type>()))
                    .Callback((Dependency d, Type t) => d.ImplementationTypes.Add(t));

            // Act
            _ = this.m_Builder.AddImplementationType(typeof(object));
            _ = this.m_Builder.AddImplementationType(typeof(object));

            // Assert
            _ = this.m_Builder.Dependency.ImplementationTypes.Should().BeEquivalentTo(new[] { typeof(object) });
        }

        #endregion AddImplementationType Tests

        #region - - - - - - ScanForImplementations Tests - - - - - -

        [Fact]
        public void ScanForImplementations_BehaviourEnablesScanning_ScansForImplementations()
        {
            // Arrange
            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()))
                    .Callback((Dependency d) => d.AllowScannedImplementationTypes = true);

            // Act
            _ = this.m_Builder.ScanForImplementations();

            // Assert
            this.m_MockOnScanForImplementationTypes.Verify(mock => mock.Invoke(), Times.Once());
            this.m_MockDependencyBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(this.m_Builder.Dependency), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForImplementations_BehaviourPreventsScanning_DoesNotScanForImplementations()
        {
            // Arrange
            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()))
                    .Callback((Dependency d) => d.AllowScannedImplementationTypes = false);

            // Act
            _ = this.m_Builder.ScanForImplementations();

            // Assert
            this.m_MockOnScanForImplementationTypes.Verify(mock => mock.Invoke(), Times.Never());
            this.m_MockDependencyBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(this.m_Builder.Dependency), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion ScanForImplementations Tests

        #region - - - - - - WithImplementationFactory Tests - - - - - -

        [Fact]
        public void WithImplementationFactory_NoFactorySpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithImplementationFactory(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithImplementationFactory_AnyFactory_AddsFactoryThroughBehaviour()
        {
            // Arrange
            Func<DependencyFactory, object> _Factory = factory => string.Empty;

            // Act
            _ = this.m_Builder.WithImplementationFactory(_Factory);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.UpdateImplementationFactory(this.m_Builder.Dependency, _Factory), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
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
            this.m_Builder.Dependency.Lifetime = TestDependencyLifetime.Instance(false);

            // Act
            var _Expected = Record.Exception(() => this.m_Builder.WithImplementationInstance(string.Empty));

            // Assert
            _ = _Expected.Should().BeOfType<Exception>();
        }

        [Fact]
        public void WithImplementationInstance_LifetimeSupportsImplementationInstances_AddsInstanceThroughBehaviour()
        {
            // Arrange

            // Act
            _ = this.m_Builder.WithImplementationInstance(string.Empty);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.UpdateImplementationInstance(this.m_Builder.Dependency, string.Empty), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithImplementationInstance Tests

        #region - - - - - - WithLifetime Tests - - - - - -

        [Fact]
        public void WithLifetime_NoLifetimeSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithLifetime(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithLifetime_AnyLifetime_AddsLifetimeThroughBehaviour()
        {
            // Arrange
            var _Lifetime = TestDependencyLifetime.Instance(true);

            // Act
            _ = this.m_Builder.WithLifetime(_Lifetime);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.UpdateLifetime(this.m_Builder.Dependency, _Lifetime), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void WithLifetime_LifetimeSupportsImplementationInstances_RetainsAssignedImplementationInstance()
        {
            // Arrange
            this.m_Builder.Dependency.Lifetime = TestDependencyLifetime.Instance(true);
            this.m_Builder.Dependency.ImplementationInstance = 123;

            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.UpdateLifetime(It.IsAny<Dependency>(), It.IsAny<DependencyLifetime>()))
                    .Callback((Dependency d, DependencyLifetime l) => d.Lifetime = l);

            // Act
            _ = this.m_Builder.WithLifetime(TestDependencyLifetime.Instance(true));

            // Assert
            _ = this.m_Builder.Dependency.ImplementationInstance.Should().Be(123);
        }

        [Fact]
        public void WithLifetime_LifetimeDoesntSupportImplementationInstances_ClearsAssignedImplementationInstance()
        {
            // Arrange
            this.m_Builder.Dependency.Lifetime = TestDependencyLifetime.Instance(true);
            this.m_Builder.Dependency.ImplementationInstance = 123;

            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.UpdateLifetime(It.IsAny<Dependency>(), It.IsAny<DependencyLifetime>()))
                    .Callback((Dependency d, DependencyLifetime l) => d.Lifetime = l);

            // Act
            _ = this.m_Builder.WithLifetime(TestDependencyLifetime.Instance(false));

            // Assert
            _ = this.m_Builder.Dependency.ImplementationInstance.Should().BeNull();
        }

        #endregion WithLifetime Tests

        #region - - - - - - WithDependency Tests - - - - - -

        [Fact]
        public void WithDependency_NoDependencySpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithDependency(null, out _))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithDependency_DependencyIsForDifferentType_ThrowsArgumentException()
        {
            // Arrange
            var _Dependency = new Dependency(typeof(object));

            // Act
            var _Expected = Record.Exception(() => this.m_Builder.WithDependency(_Dependency, out _));

            // Assert
            _ = _Expected.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void WithDependency_DependencyIsForSameType_CreatesNewDependencyAndReturnsOldDependency()
        {
            // Arrange
            var _Initial = this.m_Builder.Dependency;
            var _Expected = new Dependency(typeof(IDependency))
            {
                AllowScannedImplementationTypes = true,
                Behaviour = this.m_MockDependencyBehaviour.Object,
                ImplementationFactory = factory => string.Empty,
                ImplementationInstance = string.Empty,
                ImplementationTypes = new List<Type> { typeof(IDependency) },
                Lifetime = TestDependencyLifetime.Instance(true)
            };

            // Act
            _ = this.m_Builder.WithDependency(_Expected, out var _Actual);

            // Assert
            _ = _Actual.Should().Be(_Initial);
            _ = this.m_Builder.Dependency.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void WithDependency_IncomingDependencyHasLinkedDependency_DependencyIsNotLinked()
        {
            // Arrange
            var _Dependency = new Dependency(typeof(IDependency)) { LinkedDependency = new Dependency(typeof(IDependency)) };

            // Act
            _ = this.m_Builder.WithDependency(_Dependency, out _);

            // Assert
            _ = this.m_Builder.Dependency.LinkedDependency.Should().BeNull();
        }

        [Fact]
        public void WithDependency_ExistingDependencyHasLinkedDependency_DependencyLinkIsNotRetained()
        {
            // Arrange
            this.m_Builder.Dependency.LinkedDependency = new Dependency(typeof(IDependency));

            var _Dependency = new Dependency(typeof(IDependency));

            // Act
            _ = this.m_Builder.WithDependency(_Dependency, out _);

            // Assert
            _ = this.m_Builder.Dependency.LinkedDependency.Should().BeNull();
        }

        [Fact]
        public void WithDependency_BothDependenciesHaveLinkedDependency_IncomingLinkedDependencyOverridesExisting()
        {
            // Arrange
            this.m_Builder.Dependency.LinkedDependency = new Dependency(typeof(IDependency));

            var _Expected = new Dependency(typeof(IDependency));

            var _Dependency = new Dependency(typeof(IDependency)) { LinkedDependency = _Expected };

            // Act
            _ = this.m_Builder.WithDependency(_Dependency, out _);

            // Assert
            _ = this.m_Builder.Dependency.LinkedDependency.Should().Be(_Expected);
        }

        #endregion WithDependency Tests

        #region - - - - - - WithBehaviour Tests - - - - - -

        [Fact]
        public void WithBehaviour_NoBehaviourSpecified_ThrowsArgumentNullException()
            => Record.Exception(() => this.m_Builder.WithBehaviour(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void WithBehaviour_AnyBehaviour_UpdatesBehaviourThroughBehaviour()
        {
            // Arrange
            var _DependencyBehaviour = new Mock<IDependencyBehaviour>().Object;

            // Act
            _ = this.m_Builder.WithBehaviour(_DependencyBehaviour);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.UpdateBehaviour(this.m_Builder.Dependency, _DependencyBehaviour), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion WithBehaviour Tests

    }

}
