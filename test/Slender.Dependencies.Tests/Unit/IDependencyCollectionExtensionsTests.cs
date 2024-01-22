using FluentAssertions;
using Moq;
using Slender.AssemblyScanner;
using Slender.Dependencies.Tests.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class IDependencyCollectionExtensionsTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<Action<IDependency>> m_MockDependencyAction = new();
        private readonly Mock<Action<IDependencyCollection, Type>> m_MockRegisterDependencyAction = new();
        private readonly Mock<Action<IDependency, Type>> m_MockRegisterImplementationAction = new();
        private readonly Mock<Action<IDependencyCollection, Type>> m_MockRegisterTypeAction = new();

        private readonly IAssemblyScan m_AssemblyScan;
        private readonly List<Type> m_AssemblyTypes = new();
        private readonly TestDependencyCollection m_DependencyCollection = new();
        private readonly DependencyScanningOptions m_DependencyScanningOptions;
        private readonly Func<DependencyFactory, IDependency> m_ImplementationFactory = new Mock<Func<DependencyFactory, IDependency>>().Object;
        private readonly ServiceImplementation m_ImplementationInstance = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public IDependencyCollectionExtensionsTests()
        {
            var _MockAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes.ToArray());

            this.m_AssemblyScan = _MockAssemblyScan.Object;
            this.m_DependencyScanningOptions = new()
            {
                OnUnregisteredDependencyTypeFound = this.m_MockRegisterDependencyAction.Object,
                OnUnregisteredImplementationTypeFound = this.m_MockRegisterImplementationAction.Object,
                OnUnregisteredTypeFound = this.m_MockRegisterTypeAction.Object,
            };
        }

        #endregion Constructors

        #region - - - - - - AddAssemblyScan Tests - - - - - -

        [Fact]
        public void AddAssemblyScan_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddAssemblyScan<IDependencyCollection>(default!, this.m_DependencyScanningOptions, this.m_AssemblyScan))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddAssemblyScan_NotSpecifyingOptions_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddAssemblyScan(null, this.m_AssemblyScan))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddAssemblyScan_NotSpecifyingAssemblyScan_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddAssemblyScan_AddingScanWithExistingDependency_IgnoresExistingDependencies()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(IService));
            this.m_AssemblyTypes.Add(typeof(IService));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            this.m_MockRegisterDependencyAction.VerifyNoOtherCalls();
            this.m_MockRegisterImplementationAction.VerifyNoOtherCalls();
            this.m_MockRegisterTypeAction.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingScanWithNoDependencies_DoesNotInvokeImplementationRegistrationAction()
        {
            // Arrange

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            this.m_MockRegisterDependencyAction.VerifyNoOtherCalls();
            this.m_MockRegisterImplementationAction.VerifyNoOtherCalls();
            this.m_MockRegisterTypeAction.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingScanWithGenericDependencies_OpenAndClosedGenericsAreRegisteredCorrectly()
        {
            // Arrange
            this.m_AssemblyTypes.AddRange(new[]
            {
                typeof(AbstractService),
                typeof(ClosedGenericServiceImplementation),
                typeof(ClosedGenericServiceImplementation2),
                typeof(ConcreteService),
                typeof(IGenericService<>),
                typeof(IService),
                typeof(IUnimplementedService),
                typeof(OpenGenericOtherImplementation<>),
                typeof(OpenGenericServiceImplementation<>),
                typeof(ServiceDecorator),
                typeof(ServiceImplementation),
                typeof(ServiceImplementation2),
                typeof(UninheritedAbstractService)
            });

            var _Expected = new TestDependencyCollection
            {
                Dependencies = new List<TestDependency>
                {
                    new(typeof(AbstractService))
                    {
                        Implementations = new()
                        {
                            typeof(ConcreteService)
                        }
                    },
                    new(typeof(IService))
                    {
                        Implementations = new()
                        {
                            typeof(ServiceDecorator),
                            typeof(ServiceImplementation),
                            typeof(ServiceImplementation2)
                        }
                    },
                    new(typeof(IGenericService<>))
                    {
                        Implementations = new()
                        {
                            typeof(OpenGenericServiceImplementation<>)
                        }
                    },
                    new(typeof(IGenericService<object>))
                    {
                        Implementations = new()
                        {
                            typeof(ClosedGenericServiceImplementation),
                            typeof(ClosedGenericServiceImplementation2)
                        }
                    },
                    new(typeof(IUnimplementedService)),
                    new(typeof(UninheritedAbstractService))
                }.OfType<IDependency>().ToList()
            };

            this.m_DependencyScanningOptions.OnUnregisteredDependencyTypeFound = (dependencies, type) => dependencies.AddDependency(type);
            this.m_DependencyScanningOptions.OnUnregisteredImplementationTypeFound = (dependency, type) => dependency.AddImplementation(type);

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingScanWithTransitiveDependency_IgnoresTransitiveDependenciesFromRegistrationActions()
        {
            // Arrange
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService));
            this.m_AssemblyTypes.Add(typeof(IService));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            this.m_MockRegisterDependencyAction.VerifyNoOtherCalls();
            this.m_MockRegisterImplementationAction.VerifyNoOtherCalls();
            this.m_MockRegisterTypeAction.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingScanWithOpenGenericTransitiveDependency_IgnoresOpenAndClosedGenericTransitiveDependenciesFromRegistrationActions()
        {
            // Arrange
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IGenericService<>));
            this.m_AssemblyTypes.Add(typeof(IGenericService<>));
            this.m_AssemblyTypes.Add(typeof(IGenericService<object>));
            this.m_AssemblyTypes.Add(typeof(OpenGenericServiceImplementation<>));
            this.m_AssemblyTypes.Add(typeof(OpenGenericServiceImplementation<object>));
            this.m_AssemblyTypes.Add(typeof(ClosedGenericServiceImplementation));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            this.m_MockRegisterTypeAction.Verify(mock => mock.Invoke(It.IsAny<IDependencyCollection>(), typeof(IGenericService<>)), Times.Never());
            this.m_MockRegisterTypeAction.Verify(mock => mock.Invoke(It.IsAny<IDependencyCollection>(), typeof(IGenericService<object>)), Times.Never());
            this.m_MockRegisterDependencyAction.VerifyNoOtherCalls();
            this.m_MockRegisterImplementationAction.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingScanWithClosedGenericTransitiveDependency_IgnoresOpenAndClosedGenericTransitiveDependenciesFromRegistrationActions()
        {
            // Arrange
            this.m_DependencyCollection.AddTransitiveDependency(typeof(IGenericService<string>));
            this.m_AssemblyTypes.Add(typeof(IGenericService<>));
            this.m_AssemblyTypes.Add(typeof(IGenericService<object>));
            this.m_AssemblyTypes.Add(typeof(OpenGenericServiceImplementation<>));
            this.m_AssemblyTypes.Add(typeof(OpenGenericServiceImplementation<object>));
            this.m_AssemblyTypes.Add(typeof(ClosedGenericServiceImplementation));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_DependencyScanningOptions, this.m_AssemblyScan);

            // Assert
            this.m_MockRegisterTypeAction.Verify(mock => mock.Invoke(It.IsAny<IDependencyCollection>(), typeof(IGenericService<>)), Times.Never());
            this.m_MockRegisterTypeAction.Verify(mock => mock.Invoke(It.IsAny<IDependencyCollection>(), typeof(IGenericService<object>)), Times.Never());
            this.m_MockRegisterDependencyAction.VerifyNoOtherCalls();
            this.m_MockRegisterImplementationAction.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - AddDependency Tests - - - - - -

        [Fact]
        public void AddDependency_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddDependency<IDependencyCollection>(
                    default!,
                    typeof(string),
                    this.m_MockDependencyAction.Object))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddDependency(null, this.m_MockDependencyAction.Object))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_AddingAlreadyAddedDependency_ThrowsInvalidOperationException()
            => Record
                .Exception(() =>
                {
                    _ = this.m_DependencyCollection.AddDependency(typeof(string), this.m_MockDependencyAction.Object);
                    _ = this.m_DependencyCollection.AddDependency(typeof(string), this.m_MockDependencyAction.Object);
                })
                .Should()
                .BeOfType<InvalidOperationException>();

        [Fact]
        public void AddDependency_NotSpecifyingConfigurationAction_RegistersDependency()
        {
            // Arrange
            var _Expected = new TestDependencyCollection() { Dependencies = new() { new TestDependency(typeof(string)) } };

            // Act
            _ = this.m_DependencyCollection.AddDependency(typeof(string), null);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependency_ConfigurationActionSpecified_RegistersDependencyAndInvokesConfigurationAction()
        {
            // Arrange

            // Act
            _ = this.m_DependencyCollection.AddDependency(typeof(string), this.m_MockDependencyAction.Object);
            var _Dependency = this.m_DependencyCollection.GetDependency(typeof(string))!;

            // Assert
            this.m_MockDependencyAction.Verify(mock => mock.Invoke(_Dependency), Times.Once());
            this.m_MockDependencyAction.VerifyNoOtherCalls();
        }

        #endregion AddDependency Tests

        #region - - - - - - ConfigureDependency Tests - - - - - -

        [Fact]
        public void ConfigureDependency_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.ConfigureDependency<IDependencyCollection>(
                    default!,
                    typeof(string),
                    this.m_MockDependencyAction.Object))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void ConfigureDependency_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.ConfigureDependency(null, this.m_MockDependencyAction.Object))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void ConfigureDependency_NotSpecifyingConfigurationAction_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.ConfigureDependency(typeof(string), null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void ConfigureDependency_ConfiguringUnregisteredDependency_ThrowsInvalidOperationException()
            => Record
                .Exception(() => this.m_DependencyCollection.ConfigureDependency(typeof(string), this.m_MockDependencyAction.Object))
                .Should()
                .BeOfType<InvalidOperationException>();

        [Fact]
        public void ConfigureDependency_ConfiguringRegisteredDependency_ConfiguresRegisteredDependency()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string));

            var _Dependency = this.m_DependencyCollection.GetDependency(typeof(string))!;

            // Act
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(string), this.m_MockDependencyAction.Object);

            // Assert
            this.m_MockDependencyAction.Verify(mock => mock.Invoke(_Dependency));
            this.m_MockDependencyAction.VerifyNoOtherCalls();
        }

        #endregion ConfigureDependency Tests

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_DontIgnoreMissingLifetime_ValidationFails()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasImplementationInstance(string.Empty));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public void Validate_IgnoreMissingLifetime_ValidationSucceeds()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasImplementationInstance(string.Empty));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate(opts => opts.IgnoreMissingLifetimes()));

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_DontIgnoreMissingImplementations_ValidationFails()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasLifetime(TestDependencyLifetime.Instance(true)));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public void Validate_IgnoreMissingImplementations_ValidationSucceeds()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasLifetime(TestDependencyLifetime.Instance(true)));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate(opts => opts.IgnoreMissingImplementations()));

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_DontIgnoreInvalidImplementations_ValidationFails()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasLifetime(TestDependencyLifetime.Instance(false)));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().BeOfType<Exception>();
        }

        [Fact]
        public void Validate_IgnoreInvalidImplementations_ValidationSucceeds()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasImplementationInstance(string.Empty).HasLifetime(TestDependencyLifetime.Instance(false)));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate(opts => opts.IgnoreInvalidImplementations()));

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_ValidDependencyCollection_ValidationSucceeds()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(string), d => d.HasImplementationInstance(string.Empty).HasLifetime(TestDependencyLifetime.Instance(true)));

            this.m_DependencyCollection.AddTransitiveDependency(typeof(IService));

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate(opts => opts.ResolveTransitiveDependency(typeof(IService))));

            // Assert
            _ = _Exception.Should().BeNull();
        }

        #endregion Validate Tests

    }

    public abstract class AbstractService { }

    public class ClosedGenericServiceImplementation : IGenericService<object> { }

    public class ClosedGenericServiceImplementation2 : IGenericService<object> { }

    public class ConcreteService { }

    public interface IGenericService<TGeneric> { }

    public interface IService { }

    public interface IUnimplementedService { }

    public class OpenGenericOtherImplementation<TGeneric> { }

    public class OpenGenericServiceImplementation<TGeneric> : IGenericService<TGeneric> { }

    public class ServiceDecorator : IService { }

    public class ServiceImplementation : IService { }

    public class ServiceImplementation2 : IService { }

    public abstract class UninheritedAbstractService { }

}
