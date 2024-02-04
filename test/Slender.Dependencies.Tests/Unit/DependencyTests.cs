using FluentAssertions;
using Slender.Dependencies.Tests.Support;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public class DependencyTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dependency m_Dependency = new(typeof(string), autoLockLifetime: false);

        #endregion Fields

        #region - - - - - - AddDecorator Tests - - - - - -

        [Fact]
        public void AddDecorator_NoDecoratorSpecified_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.AddDecorator(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDecorator_NewDecoratorSpecified_AddsDecorator()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string)) { Decorators = new() { typeof(string) } };

            // Act
            this.m_Dependency.AddDecorator(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDecorator_ExistingDecoratorSpecified_DoesNotAddDecorator()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string)) { Decorators = new() { typeof(string) } };
            this.m_Dependency.AddDecorator(typeof(string));

            // Act
            this.m_Dependency.AddDecorator(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDecorator_DecoratorsAreLocked_DoesNotAddDecorator()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string));
            _ = this.m_Dependency.LockDecorators();

            // Act
            this.m_Dependency.AddDecorator(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        #endregion AddDecorator Tests

        #region - - - - - - AddImplementation Tests - - - - - -

        [Fact]
        public void AddImplementation_NoImplementationSpecified_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.AddImplementation(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddImplementation_ImplementationSpecified_AddsImplementation()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string)) { Implementations = new() { typeof(string) } };

            // Act
            this.m_Dependency.AddImplementation(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddImplementation_ExistingImplementationSpecified_DoesNotAddImplementation()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string)) { Implementations = new() { typeof(string) } };
            this.m_Dependency.AddImplementation(typeof(string));

            // Act
            this.m_Dependency.AddImplementation(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddImplementation_ImplementationsAreLocked_DoesNotAddImplementation()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string));
            _ = this.m_Dependency.LockImplementations();

            // Act
            this.m_Dependency.AddImplementation(typeof(string));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        #endregion AddImplementation Tests

        #region - - - - - - AddToDependency Tests - - - - - -

        [Fact]
        public void AddToDependency_NoDependencySpecified_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.AddToDependency(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddToDependency_DependencySpecified_AddsToDependency()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(string));
            var _Expected = new TestDependency(typeof(string))
            {
                Decorators = new() { typeof(int), typeof(object), typeof(IService) },
                Implementations = new() { typeof(ServiceImplementation), typeof(ServiceImplementation2) },
                Lifetime = TestDependencyLifetime.Instance(true),
                DependencyType = typeof(string)
            };

            this.m_Dependency.AddDecorator(typeof(int));
            this.m_Dependency.AddDecorator(typeof(object));
            this.m_Dependency.AddDecorator(typeof(IService));
            this.m_Dependency.AddImplementation(typeof(ServiceImplementation));
            this.m_Dependency.AddImplementation(typeof(ServiceImplementation2));
            this.m_Dependency.SetLifetime(TestDependencyLifetime.Instance(true));

            // Act
            this.m_Dependency.AddToDependency(_Dependency);

            // Assert
            _ = _Dependency.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddToDependency Tests

        #region - - - - - - GetDependencyType Tests - - - - - -

        [Fact]
        public void GetDependencyType_GetDependencyType_GetsDependencyType()
            => this.m_Dependency.GetDependencyType().Should().Be(typeof(string));

        #endregion GetDependencyType Tests

        #region - - - - - - LockDependency Tests - - - - - -

        [Fact]
        public void LockDependency_AnyDependency_DependencyBecomesReadOnly()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string));

            // Act
            _ = this.m_Dependency.LockDependency();
            this.m_Dependency.AddDecorator(typeof(string));
            this.m_Dependency.AddImplementation(typeof(string));
            this.m_Dependency.SetLifetime(TestDependencyLifetime.Instance(true));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        #endregion LockDependency Tests

        #region - - - - - - SetLifetime Tests - - - - - -

        [Fact]
        public void SetLifetime_NoLifetimeSpecified_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.SetLifetime(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void SetLifetime_LifetimeSpecified_SetsLifetime()
        {
            // Arrange
            var _Lifetime = TestDependencyLifetime.Instance(true);
            var _Expected = new TestDependency(typeof(string)) { Lifetime = _Lifetime };

            // Act
            this.m_Dependency.SetLifetime(_Lifetime);

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void SetLifetime_LifetimeIsLocked_DoesNotSetLifetime()
        {
            // Arrange
            var _Expected = new TestDependency(typeof(string));
            _ = this.m_Dependency.LockLifetime();

            // Act
            this.m_Dependency.SetLifetime(TestDependencyLifetime.Instance(true));

            // Assert
            _ = this.m_Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void SetLifetime_LifetimeIsSetForAutoLockDependency_LocksLifetime()
        {
            // Arrange
            var _Dependency = new Dependency(typeof(string), autoLockLifetime: true);
            var _Lifetime = TestDependencyLifetime.Instance(true);
            var _Expected = new TestDependency(typeof(string)) { Lifetime = _Lifetime };

            // Act
            _Dependency.SetLifetime(_Lifetime);
            _Dependency.SetLifetime(TestDependencyLifetime.Instance(false));

            // Assert
            _ = _Dependency.Read().Should().BeEquivalentTo(_Expected);
        }

        #endregion SetLifetime Tests

    }

}
