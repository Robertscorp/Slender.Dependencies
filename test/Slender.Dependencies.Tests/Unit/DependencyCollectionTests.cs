using FluentAssertions;
using Moq;
using Slender.Dependencies.Options;
using Slender.Dependencies.Tests.Support;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class DependencyCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<DependencyExistsResolutionStrategy> m_MockDependencyExistsStrategy = new();
        private readonly Mock<DependencyMergeResolutionStrategy> m_MockDependencyMergeStrategy = new();
        private readonly Mock<DependencyProvider> m_MockDependencyProvider = new();

        private readonly DependencyCollection m_DependencyCollection;
        private readonly TestDependency m_ExistingClosedGenericDependency = new(typeof(IGenericService<object>));
        private readonly TestDependency m_ExistingDependency = new(typeof(IService));
        private readonly TestDependency m_ExistingOpenGenericDependency = new(typeof(IGenericService<>));

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public DependencyCollectionTests()
        {
            this.m_DependencyCollection = new(options =>
                options.HasDependencyProvider(this.m_MockDependencyProvider.Object)
                    .HasResolutionStrategyForExistingDependency(this.m_MockDependencyExistsStrategy.Object)
                    .HasResolutionStrategyForMergingDependencies(this.m_MockDependencyMergeStrategy.Object));

            this.m_DependencyCollection.AddDependency(this.m_ExistingClosedGenericDependency);
            this.m_DependencyCollection.AddDependency(this.m_ExistingDependency);
            this.m_DependencyCollection.AddDependency(this.m_ExistingOpenGenericDependency);

            _ = this.m_MockDependencyExistsStrategy
                    .Setup(mock => mock.Invoke(It.IsAny<DependencyProvider>(), It.IsAny<IDependency>(), It.IsAny<Type>()))
                    .Returns((DependencyProvider dp, IDependency d, Type t) => dp.Invoke(t));

            _ = this.m_MockDependencyMergeStrategy
                    .Setup(mock => mock.Invoke(It.IsAny<IDependency>(), It.IsAny<IDependency>()))
                    .Returns((IDependency existing, IDependency incoming) => incoming);

            _ = this.m_MockDependencyProvider
                    .Setup(mock => mock.Invoke(It.IsAny<Type>()))
                    .Returns((Type t) => new TestDependency(t));
        }

        #endregion Constructors

        #region - - - - - - AddDependency(IDependency) Tests - - - - - -

        [Fact]
        public void AddDependency_IDependency_NotSpecifyingDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddDependency(dependency: null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_IDependency_AddingNewDependency_AddsNewDependencyWithoutInvokingMergeStrategy()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(IService2));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _Dependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            this.m_DependencyCollection.AddDependency(_Dependency);

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyMergeStrategy.Verify(mock => mock.Invoke(It.IsAny<IDependency>(), It.IsAny<IDependency>()), Times.Never());
        }

        [Fact]
        public void AddDependency_IDependency_AddingNewClosedGenericDependency_AddsNewDependencyWithoutInvokingMergeStrategy()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(IGenericService<string>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _Dependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            this.m_DependencyCollection.AddDependency(_Dependency);

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyMergeStrategy.Verify(mock => mock.Invoke(It.IsAny<IDependency>(), It.IsAny<IDependency>()), Times.Never());
        }

        [Fact]
        public void AddDependency_IDependency_AddingAlreadyAddedDependency_ResolvesUsingMergeStrategy()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(IService));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _Dependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            this.m_DependencyCollection.AddDependency(_Dependency);

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyMergeStrategy.Verify(mock => mock.Invoke(this.m_ExistingDependency, _Dependency));
        }

        [Fact]
        public void AddDependency_IDependency_AddingAlreadyAddedOpenGenericDependency_ResolvesUsingMergeStrategy()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(IGenericService<>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _Dependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency } };

            // Act
            this.m_DependencyCollection.AddDependency(_Dependency);

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyMergeStrategy.Verify(mock => mock.Invoke(this.m_ExistingOpenGenericDependency, _Dependency));
        }

        [Fact]
        public void AddDependency_IDependency_AddingAlreadyAddedClosedGenericDependency_ResolvesUsingMergeStrategy()
        {
            // Arrange
            var _Dependency = new TestDependency(typeof(IGenericService<object>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _Dependency, this.m_ExistingDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            this.m_DependencyCollection.AddDependency(_Dependency);

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyMergeStrategy.Verify(mock => mock.Invoke(this.m_ExistingClosedGenericDependency, _Dependency));
        }

        [Fact]
        public void AddDependency_IDependency_MergeStrategyReturnsNullDependency_ThrowsException()
        {
            // Arrange
            this.m_MockDependencyMergeStrategy.Reset();

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.AddDependency(new TestDependency(typeof(IService))));

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        #endregion AddDependency(IDependency) Tests

        #region - - - - - - AddDependency(Type) Tests - - - - - -

        [Fact]
        public void AddDependency_Type_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddDependency(dependencyType: null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_Type_AddingNewDependencyType_AddsNewDependencyWithoutInvokingExistsStrategy()
        {
            // Arrange
            var _ExpectedDependency = new TestDependency(typeof(IService2));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _ExpectedDependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            var _Actual = this.m_DependencyCollection.AddDependency(_ExpectedDependency.DependencyType);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_ExpectedDependency);
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyExistsStrategy.Verify(mock => mock.Invoke(this.m_MockDependencyProvider.Object, this.m_ExistingDependency, _ExpectedDependency.DependencyType), Times.Never());
        }

        [Fact]
        public void AddDependency_Type_AddingNewClosedGenericDependencyType_AddsNewDependencyWithoutInvokingExistsStrategy()
        {
            // Arrange
            var _ExpectedDependency = new TestDependency(typeof(IGenericService<string>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _ExpectedDependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            var _Actual = this.m_DependencyCollection.AddDependency(_ExpectedDependency.DependencyType);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_ExpectedDependency);
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyExistsStrategy.Verify(mock => mock.Invoke(this.m_MockDependencyProvider.Object, this.m_ExistingDependency, _ExpectedDependency.DependencyType), Times.Never());
        }

        [Fact]
        public void AddDependency_Type_AddingAlreadyAddedDependency_ResolvesUsingExistsStrategy()
        {
            // Arrange
            var _ExpectedDependency = new TestDependency(typeof(IService));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _ExpectedDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            var _Actual = this.m_DependencyCollection.AddDependency(typeof(IService));

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_ExpectedDependency);
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyExistsStrategy.Verify(mock => mock.Invoke(this.m_MockDependencyProvider.Object, this.m_ExistingDependency, _ExpectedDependency.DependencyType));
        }

        [Fact]
        public void AddDependency_Type_AddingAlreadyAddedOpenGenericDependency_ResolvesUsingExistsStrategy()
        {
            // Arrange
            var _ExpectedDependency = new TestDependency(typeof(IGenericService<>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _ExpectedDependency, this.m_ExistingDependency, this.m_ExistingClosedGenericDependency } };

            // Act
            var _Actual = this.m_DependencyCollection.AddDependency(_ExpectedDependency.DependencyType);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_ExpectedDependency);
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyExistsStrategy.Verify(mock => mock.Invoke(this.m_MockDependencyProvider.Object, this.m_ExistingOpenGenericDependency, _ExpectedDependency.DependencyType));
        }

        [Fact]
        public void AddDependency_Type_AddingAlreadyAddedClosedGenericDependency_ResolvesUsingExistsStrategy()
        {
            // Arrange
            var _ExpectedDependency = new TestDependency(typeof(IGenericService<object>));
            var _Expected = new TestDependencyCollection { Dependencies = new() { _ExpectedDependency, this.m_ExistingDependency, this.m_ExistingOpenGenericDependency } };

            // Act
            var _Actual = this.m_DependencyCollection.AddDependency(_ExpectedDependency.DependencyType);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_ExpectedDependency);
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);

            this.m_MockDependencyExistsStrategy.Verify(mock => mock.Invoke(this.m_MockDependencyProvider.Object, this.m_ExistingClosedGenericDependency, _ExpectedDependency.DependencyType));
        }

        [Fact]
        public void AddDependency_Type_DependencyProviderReturnsNullDependency_ThrowsException()
        {
            // Arrange
            this.m_MockDependencyProvider.Reset();

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.AddDependency(typeof(string)));

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public void AddDependency_Type_DependencyExistsStrategyReturnsNullDependency_ThrowsException()
        {
            // Arrange
            this.m_MockDependencyExistsStrategy.Reset();

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.AddDependency(typeof(IService)));

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        #endregion AddDependency(Type) Tests

        #region - - - - - - AddToDependencyCollection Tests - - - - - -

        [Fact]
        public void AddToDependencyCollection_NotSpecifyingDependencyCollection_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddToDependencyCollection(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddToDependencyCollection_AnyDependencyCollection_AddsAllDependenciesAndTransitiveDependencies()
        {
            // Arrange
            var _DependencyCollection = new TestDependencyCollection();
            var _Expected = new TestDependencyCollection()
            {
                Dependencies = new() { this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency },
                TransitiveDependencies = new() { typeof(ISetup), typeof(IService2) }
            };

            this.m_DependencyCollection.AddTransitiveDependency(typeof(ISetup));
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService2));

            // Act
            this.m_DependencyCollection.AddToDependencyCollection(_DependencyCollection);

            // Assert
            _ = _DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion AddToDependencyCollection Tests

        #region - - - - - - AddTransitiveDependency Tests - - - - - -

        [Fact]
        public void AddTransitiveDependency_NotSpecifyingTransitiveDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddTransitiveDependency(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransitiveDependency_AddingTransitiveDependency_AddsTransitiveDependency()
        {
            // Arrange
            var _Expected = new TestDependencyCollection
            {
                Dependencies = new() { this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency },
                TransitiveDependencies = new() { typeof(IService) }
            };

            // Act
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService));

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddTransitiveDependency_AddingOpenGenericTransitiveDependency_AddsOpenGenericTransitiveDependency()
        {
            // Arrange
            var _Expected = new TestDependencyCollection
            {
                Dependencies = new() { this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency },
                TransitiveDependencies = new() { typeof(IGenericService<>) }
            };

            // Act
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IGenericService<>));

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddTransitiveDependency_AddingClosedGenericTransitiveDependency_AddsAsOpenGenericTransitiveDependency()
        {
            // Arrange
            var _Expected = new TestDependencyCollection
            {
                Dependencies = new() { this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency },
                TransitiveDependencies = new() { typeof(IGenericService<>) }
            };

            // Act
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IGenericService<object>));

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddTransitiveDependency_AddingAlreadyAddedTransitiveDependency_DoesNothing()
        {
            // Arrange
            var _Expected = new TestDependencyCollection
            {
                Dependencies = new() { this.m_ExistingDependency, this.m_ExistingClosedGenericDependency, this.m_ExistingOpenGenericDependency },
                TransitiveDependencies = new() { typeof(IService) }
            };

            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService));

            // Act
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService));

            // Assert
            _ = this.m_DependencyCollection.Read().Should().BeEquivalentTo(_Expected);
        }

        #endregion AddTransitiveDependency Tests

        #region - - - - - - GetDependency Tests - - - - - -

        [Fact]
        public void GetDependency_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.GetDependency(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void GetDependency_DependencyIsNotRegisteredInCollection_ReturnsNull()
            => this.m_DependencyCollection
                .GetDependency(typeof(IDisposable))
                .Should()
                .BeNull();

        [Fact]
        public void GetDependency_OpenGenericDependencyIsNotRegisteredInCollection_ReturnsNull()
            => this.m_DependencyCollection
                .GetDependency(typeof(IEquatable<>))
                .Should()
                .BeNull();

        [Fact]
        public void GetDependency_ClosedGenericDependencyIsNotRegisteredInCollection_ReturnsNull()
            => this.m_DependencyCollection
                .GetDependency(typeof(IGenericService<string>))
                .Should()
                .BeNull();

        [Fact]
        public void GetDependency_DependencyIsRegisteredInCollection_ReturnsDependency()
            => this.m_DependencyCollection
                .GetDependency(typeof(IService))
                .Should()
                .Be(this.m_ExistingDependency);

        [Fact]
        public void GetDependency_OpenGenericDependencyIsRegisteredInCollection_ReturnsDependency()
            => this.m_DependencyCollection
                .GetDependency(typeof(IGenericService<>))
                .Should()
                .Be(this.m_ExistingOpenGenericDependency);

        [Fact]
        public void GetDependency_ClosedGenericDependencyIsRegisteredInCollection_ReturnsDependency()
            => this.m_DependencyCollection
                .GetDependency(typeof(IGenericService<object>))
                .Should()
                .Be(this.m_ExistingClosedGenericDependency);

        #endregion GetDependency Tests

    }

}
