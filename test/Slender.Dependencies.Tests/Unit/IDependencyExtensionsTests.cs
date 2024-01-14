using FluentAssertions;
using Moq;
using Slender.Dependencies.Legacy;
using Slender.Dependencies.Tests.Support;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public class IDependencyExtensionsTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IDependency> m_MockDependency = new();

        private readonly Func<DependencyFactory, object> m_ImplementationFactory = factory => new();
        private readonly object m_ImplementationInstance = new Mock<IDependency>().Object;
        private readonly DependencyLifetime m_Lifetime = TestDependencyLifetime.Instance(true);

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public IDependencyExtensionsTests()
        {
            _ = this.m_MockDependency
                    .Setup(mock => mock.GetDependencyType())
                    .Returns(typeof(IDependency));
            _ = 0;
        }

        #endregion Constructors

        #region - - - - - - HasImplementationFactory Tests - - - - - -

        [Fact]
        public void HasImplementationFactory_NullDependency_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationFactory<IDependency>(null!, this.m_ImplementationFactory));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationFactory_NullFactory_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationFactory(this.m_MockDependency.Object, null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationFactory_DependencyAndFactorySpecified_AddsFactoryAndReturnsDependency()
        {
            // Arrange

            // Act
            var _Actual = IDependencyExtensions.HasImplementationFactory(this.m_MockDependency.Object, this.m_ImplementationFactory);

            // Assert
            _ = _Actual.Should().Be(this.m_MockDependency.Object);

            this.m_MockDependency.Verify(mock => mock.AddImplementation(this.m_ImplementationFactory), Times.Once());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        #endregion HasImplementationFactory Tests

        #region - - - - - - HasImplementationInstance Tests - - - - - -

        [Fact]
        public void HasImplementationInstance_TDependency__NullDependency_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationInstance<IDependency>(null!, this.m_ImplementationInstance));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationInstance_TDependency_NullInstance_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationInstance(this.m_MockDependency.Object, null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationInstance_TDependency_InstanceIsNotDerivedFromDependencyType_ThrowsArgumentException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationInstance(this.m_MockDependency.Object, new object()));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentException>();

            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationInstance_TDependency_DependencyAndValidInstanceSpecified_AddsInstanceAndReturnsDependency()
        {
            // Arrange

            // Act
            var _Actual = IDependencyExtensions.HasImplementationInstance(this.m_MockDependency.Object, this.m_ImplementationInstance);

            // Assert
            _ = _Actual.Should().Be(this.m_MockDependency.Object);

            this.m_MockDependency.Verify(mock => mock.AddImplementation(this.m_ImplementationInstance), Times.Once());
            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        #endregion HasImplementationInstance Tests

        #region - - - - - - HasImplementationType<TDependency> Tests - - - - - -

        [Fact]
        public void HasImplementationType_TDependency_NullDependency_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationType<IDependency>(null!, typeof(Dependency)));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationType_TDependency_NullType_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationType(this.m_MockDependency.Object, null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationType_TDependency_TypeIsNotDerivedFromDependencyType_ThrowsArgumentException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationType(this.m_MockDependency.Object, typeof(string)));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentException>();

            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationType_TDependency_DependencyAndValidTypeSpecified_AddsTypeAndReturnsDependency()
        {
            // Arrange

            // Act
            var _Actual = IDependencyExtensions.HasImplementationType(this.m_MockDependency.Object, typeof(Dependency));

            // Assert
            _ = _Actual.Should().Be(this.m_MockDependency.Object);

            this.m_MockDependency.Verify(mock => mock.AddImplementation(typeof(Dependency)), Times.Once());
            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        #endregion HasImplementationType<TDependency> Tests

        #region - - - - - - HasImplementationType<TImplementation> Tests - - - - - -

        [Fact]
        public void HasImplementationType_TImplementation_NullDependency_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationType<IDependency>(null!));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationType_TImplementation_TypeIsNotDerivedFromDependencyType_ThrowsArgumentException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasImplementationType<string>(this.m_MockDependency.Object));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentException>();

            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasImplementationType_TImplementation_DependencyAndValidTypeSpecified_AddsTypeAndReturnsDependency()
        {
            // Arrange

            // Act
            var _Actual = IDependencyExtensions.HasImplementationType<Dependency>(this.m_MockDependency.Object);

            // Assert
            _ = _Actual.Should().Be(this.m_MockDependency.Object);

            this.m_MockDependency.Verify(mock => mock.AddImplementation(typeof(Dependency)), Times.Once());
            this.m_MockDependency.Verify(mock => mock.GetDependencyType());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        #endregion HasImplementationType<TImplementation> Tests

        #region - - - - - - HasLifetime Tests - - - - - -

        [Fact]
        public void HasLifetime_NullDependency_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasLifetime<IDependency>(null!, this.m_Lifetime));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasLifetime_NullLifetime_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var _Exception = Record.Exception(() => IDependencyExtensions.HasLifetime(this.m_MockDependency.Object, null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();

            this.m_MockDependency.VerifyNoOtherCalls();
        }

        [Fact]
        public void HasLifetime_DependencyAndLifetimeSpecified_SetsLifetimeAndReturnsDependency()
        {
            // Arrange

            // Act
            var _Actual = IDependencyExtensions.HasLifetime(this.m_MockDependency.Object, this.m_Lifetime);

            // Assert
            _ = _Actual.Should().Be(this.m_MockDependency.Object);

            this.m_MockDependency.Verify(mock => mock.SetLifetime(this.m_Lifetime), Times.Once());
            this.m_MockDependency.VerifyNoOtherCalls();
        }

        #endregion HasLifetime Tests

    }

}
