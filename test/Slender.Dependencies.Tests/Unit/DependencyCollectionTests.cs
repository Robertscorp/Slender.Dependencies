using FluentAssertions;
using Moq;
using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class DependencyCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IDependencyBehaviour> m_MockDependencyBehaviour = new();
        private readonly Mock<Action<DependencyCollection, Type>> m_MockScanningBehaviour = new();

        private readonly IAssemblyScan m_AssemblyScan;
        private readonly IAssemblyScan m_AssemblyScan2;
        private readonly List<Type> m_AssemblyTypes = new();
        private readonly List<Type> m_AssemblyTypes2 = new();
        private readonly DependencyCollection m_DependencyCollection = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public DependencyCollectionTests()
        {
            var _MockAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes.ToArray());

            var _MockAssemblyScan2 = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan2
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes2.ToArray());

            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Dependency>(), It.IsAny<Type>()))
                    .Callback((Dependency d, Type t) => d.ImplementationTypes.Add(t));

            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()))
                    .Callback((Dependency dependency) => dependency.AllowScannedImplementationTypes = true);

            _ = this.m_MockDependencyBehaviour
                    .Setup(mock => mock.UpdateLifetime(It.IsAny<Dependency>(), It.IsAny<DependencyLifetime>()))
                    .Callback((Dependency d, DependencyLifetime l) => d.Lifetime = l);

            this.m_AssemblyScan = _MockAssemblyScan.Object;
            this.m_AssemblyScan2 = _MockAssemblyScan2.Object;
        }

        #endregion Constructors

        #region - - - - - - AddAssemblyScan Tests - - - - - -

        [Fact]
        public void AddAssemblyScan_AddingScanWithDependencyThatAllowsScannedImplementations_ScannedImplementationsAdded()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Dependency>(), typeof(DependencyImplementation)));
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithClosedGenericDependencyWithoutScanning_DoesNotAddImplementationToDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithClosedGenericDependencyWithScanning_AddsImplementationToDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithOpenGenericDependencyWithoutScanning_DoesNotRegisterClosedGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<>))
                {
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithOpenGenericDependencyWithScanning_RegistersClosedGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericClass_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(OtherOpenGenericImplementation<>));

            var _Expected = Array.Empty<Type>();

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationsOverMultipleScansWithOpenGenericDependency_RegistersClosedGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));
            this.m_AssemblyTypes2.Add(typeof(ClosedGenericImplementation2));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>
                    {
                        typeof(ClosedGenericImplementation),
                        typeof(ClosedGenericImplementation2)
                    },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan2);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericImplementationWithOpenGenericDependency_RegistersImplementationAgainstOpenGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingEmptyScanWithDependencyThatAllowsScannedImplementations_DoesNothing()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations().WithBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - AddDependency Tests - - - - - -

        [Fact]
        public void AddDependency_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddDependency(null, DependencyLifetime.Scoped(), d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_NotSpecifyingLifetime_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddDependency(typeof(IDependency), null, d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddDependency_NotSpecifyingConfigurationAction_RegistersDependency()
            => this.m_DependencyCollection
                .AddDependency(typeof(IDependency), DependencyLifetime.Scoped(), null)
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Dependency(typeof(IDependency)) { Lifetime = DependencyLifetime.Scoped() }
                });

        [Fact]
        public void AddDependency_AddingAlreadyAddedDependency_ThrowsInvalidOperationException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(IDependency), DependencyLifetime.Scoped(), d => { });

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.AddDependency(typeof(IDependency), DependencyLifetime.Scoped(), d => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void AddDependency_AddingAutoRegisteredClosedGenericDependency_ThrowsInvalidOperationException()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<>), DependencyLifetime.Scoped(), d => d.ScanForImplementations());
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<object>), DependencyLifetime.Scoped(), d => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void AddDependency_AddingClosedGenericDependencyWithScanning_AddsClosedGenericImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<object>), DependencyLifetime.Scoped(), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependency_AddingOpenGenericDependencyWithScanning_DiscoversAndAutoRegistersClosedGenericDependencies()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<>), DependencyLifetime.Scoped(), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependency_AddingUnregisteredDependency_BecomesRegisteredDependencyWithSpecifiedLifetime()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<object>), DependencyLifetime.Scoped(), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependency_AddingUnregisteredDependencyWithImplementationsOverMultipleAssembles_ImplementationsAddedInScanOrder()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));
            this.m_AssemblyTypes2.Add(typeof(DependencyImplementation2));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan2);

            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>()
                    {
                        typeof(DependencyImplementation),
                        typeof(DependencyImplementation2)
                    },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.AddDependency(
                    typeof(IDependency),
                    DependencyLifetime.Scoped(),
                    d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        #endregion AddDependency Tests

        #region - - - - - - ConfigureDependency Tests - - - - - -

        [Fact]
        public void ConfigureDependency_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.ConfigureDependency(null, d => { }))
                .Should().BeOfType<ArgumentNullException>();

        [Fact]
        public void ConfigureDependency_NotSpecifyingConfigurationAction_ThrowsArgumentNullException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddDependency(typeof(IDependency), DependencyLifetime.Scoped(), null);

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.ConfigureDependency(typeof(IDependency), null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ConfigureDependency_ConfigureNeverAddedDependency_ThrowsInvalidOperationException()
            => Record
                .Exception(() => this.m_DependencyCollection.ConfigureDependency(typeof(IDependency), d => { }))
                .Should().BeOfType<InvalidOperationException>();

        [Fact]
        public void ConfigureDependency_ConfigureUnregisteredDependency_ThrowsInvalidOperationException()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(IDependency));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.ConfigureDependency(typeof(IDependency), d => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ConfigureDependency_EnablingAllowScanOnDependencyWithPreScannedImplementations_EnablesAllowScanAndAddsImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object));
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IDependency), d => d.ScanForImplementations());

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()));
            this.m_MockDependencyBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Dependency>(), typeof(DependencyImplementation)));
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ConfigureDependency_EnablingAllowScanOnDependencyWithNoPreScannedImplementations_EnablesAllowScan()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IDependency), d => d.ScanForImplementations());

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<Dependency>()));
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ConfigureDependency_ConfiguringOpenGenericDependencyToAllowScan_DiscoversAndAutoRegistersClosedGenericDependencies()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>));
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IGenericDependency<>), d => d.ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void ConfigureDependency_ConfiguringAutoRegisteredClosedGenericDependency_NoLongerLinkedToOpenGenericBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                    Lifetime = DependencyLifetime.Scoped()
                },
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_DependencyCollection
                    .ConfigureDependency(typeof(IGenericDependency<object>), d
                        => d.WithBehaviour(this.m_MockDependencyBehaviour.Object)
                            .WithLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void ConfigureDependency_ConfiguringManuallyRegisteredDependencyToFindExistingManuallyAddedImplementation_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.AddImplementationType<DependencyImplementation>());
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(DependencyImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection
                    .ConfigureDependency(typeof(IDependency), d
                        => d.WithBehaviour(this.m_MockDependencyBehaviour.Object)
                            .ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion ConfigureDependency Tests

        #region - - - - - - GetEnumerator Tests - - - - - -

        [Fact]
        public void GetEnumerator_EmptyNonGenericDependency_GetsIncludedInEnumerator()
            => this.m_DependencyCollection
                .AddScoped(typeof(IDependency))
                .Should()
                .BeEquivalentTo(new[] { new Dependency(typeof(IDependency)) { Lifetime = DependencyLifetime.Scoped() } });

        [Fact]
        public void GetEnumerator_OpenGenericDependencyWithOnlyAllowScan_DoesNotGetIncludedInEnumerator()
            => this.m_DependencyCollection
                .AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations())
                .Should()
                .BeEquivalentTo(Array.Empty<Dependency>());

        [Fact]
        public void GetEnumerator_EmptyOpenGenericDependency_GetsIncludedInEnumerator()
            => this.m_DependencyCollection
                .AddScoped(typeof(IGenericDependency<>))
                .Should()
                .BeEquivalentTo(new[] { new Dependency(typeof(IGenericDependency<>)) { Lifetime = DependencyLifetime.Scoped() } });

        [Fact]
        public void GetEnumerator_OpenGenericDependencyWithImplementationType_GetsIncludedInEnumerator()
            => this.m_DependencyCollection
                .AddScoped(typeof(IGenericDependency<>), d => d.AddImplementationType(typeof(OpenGenericImplementation<>)))
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Dependency(typeof(IGenericDependency<>))
                    {
                        ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                        Lifetime = DependencyLifetime.Scoped()
                    }
                });

        [Fact]
        public void GetEnumerator_OpenGenericDependencyWithImplementationInstance_GetsIncludedInEnumerator()
            => this.m_DependencyCollection
                .AddSingleton(typeof(IGenericDependency<>), d => d.WithImplementationInstance(string.Empty))
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Dependency(typeof(IGenericDependency<>))
                    {
                        ImplementationInstance = string.Empty,
                        Lifetime = DependencyLifetime.Singleton()
                    }
                });

        [Fact]
        public void GetEnumerator_OpenGenericDependencyWithImplementationFactory_GetsIncludedInEnumerator()
        {
            // Arrange
            var _Factory = (Func<DependencyFactory, object>)(factory => string.Empty);

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.WithImplementationFactory(_Factory));

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<>))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion GetEnumerator Tests

        #region - - - - - - GetUnresolvedRequiredPackages Tests - - - - - -

        [Fact]
        public void GetUnresolvedRequiredPackages_AnyRequest_ReturnsAllUnresolvedRequiredPackages()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddRequiredPackage("PackageA");

            var _Expected = new[] { "PackageA" };

            // Act
            var _Actual = this.m_DependencyCollection.GetUnresolvedRequiredPackages();

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_Expected);
        }

        #endregion GetUnresolvedRequiredPackages Tests

        #region - - - - - - MergeDependencies Tests - - - - - -

        [Fact]
        public void MergeDependencies_MergingWithExistingDependency_UsesExistingBehaviourToMerge()
        {
            // Arrange
            var _Builder = default(DependencyBuilder);
            var _Collection = new DependencyCollection().AddScoped(typeof(object));
            var _Dependency = _Collection.Single();

            _ = this.m_DependencyCollection.AddScoped(typeof(object), d => _Builder = d.WithBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_Collection);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.MergeDependencies(_Builder, _Dependency), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void MergeDependencies_MergingWithNoExistingDependency_AddsIncomingDependency()
        {
            // Arrange
            var _Collection = new DependencyCollection().AddScoped(typeof(object));
            var _Expected = new[] { new Dependency(typeof(object)) { Lifetime = DependencyLifetime.Scoped() } };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_Collection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergingWithExistingImplementations_AddsIncomingImplementationsToExistingDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));
            this.m_AssemblyTypes2.Add(typeof(DependencyImplementation2));

            var _Collection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan2);
            var _Expected = new[]
            {
                new Dependency(typeof(IDependency))
                {
                    AllowScannedImplementationTypes = true,
                    ImplementationTypes = new List<Type>() { typeof(DependencyImplementation), typeof(DependencyImplementation2) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_Collection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        [Fact]
        public void MergeDependencies_MergingWithNoExistingImplementations_AddsIncomingImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation2));

            var _Collection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan);
            var _Expected = new Dependency(typeof(IDependency))
            {
                AllowScannedImplementationTypes = true,
                ImplementationTypes = new List<Type>() { typeof(DependencyImplementation2) },
                Lifetime = DependencyLifetime.Scoped()
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_Collection);
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Single().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergingRequiredPackages_AddsIncomingRequiredPackages()
        {
            // Arrange
            var _DependencyCollection = new DependencyCollection().AddRequiredPackage("PackageA").AddRequiredPackage("PackageB");

            _ = this.m_DependencyCollection.AddRequiredPackage("PackageA").AddRequiredPackage("PackageC");

            var _Expected = new[] { "PackageA", "PackageC", "PackageA", "PackageB" };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.GetUnresolvedRequiredPackages().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergeOpenGenericDependencyIntoUnregisteredClosedGenericDependency_RegistersClosedGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _DependencyCollection = new DependencyCollection().AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergeUnregisteredClosedGenericDependencyIntoOpenGenericDependency_RegistersClosedGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _DependencyCollection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan);

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergeAutoRegisteredClosedGenericDependencyIntoManuallyRegisteredDependency_ClosedGenericDependencyRemainsUnlinkedFromOpenGenericDependencyBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _DependencyBehaviour = new Mock<IDependencyBehaviour>().Object;

            var _DependencyCollection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan);
            _ = _DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.ScanForImplementations());

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IGenericDependency<>), d
                    => d.WithBehaviour(_DependencyBehaviour)
                        .WithLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergeManuallyRegisteredDependencyIntoAutoRegisteredClosedGenericDependency_ClosedGenericDependencyNoLongerLinkedToOpenGenericDependencyBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _DependencyBehaviour = new Mock<IDependencyBehaviour>().Object;

            var _DependencyCollection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan);
            _ = _DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IGenericDependency<>), d
                    => d.WithBehaviour(_DependencyBehaviour)
                        .WithLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeDependencies_MergeTwoUnregisteredDependencies_InsertsIncomingImplementationTypesIntoStartOfImplementationTypesOfUnregisteredDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation2));
            this.m_AssemblyTypes2.Add(typeof(ClosedGenericImplementation));

            var _DependencyBehaviour = new Mock<IDependencyBehaviour>().Object;
            var _DependencyCollection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan2);

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Dependency(typeof(IGenericDependency<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockDependencyBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation), typeof(ClosedGenericImplementation2) },
                    Lifetime = DependencyLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_DependencyCollection.MergeDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.WithBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        #endregion MergeDependencies Tests

        #region - - - - - - ScanForUnregisteredDependencies Tests - - - - - -

        [Fact]
        public void ScanForUnregisteredDependencies_ManuallyRegisteredDependency_NotScanned()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => { });

            // Act
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredDependencies_AutomaticallyRegisteredDependency_NotScanned()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations());
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredDependencies_UnregisteredDependency_IsScanned()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.Verify(mock => mock.Invoke(this.m_DependencyCollection, typeof(IGenericDependency<object>)), Times.Once());
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredDependencies_InvokedMultipleTimesWithUnregisteredDependency_IsScannedMultipleTimes()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);
            _ = this.m_DependencyCollection.ScanForUnregisteredDependencies(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.Verify(mock => mock.Invoke(this.m_DependencyCollection, typeof(IGenericDependency<object>)), Times.Exactly(3));
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        #endregion ScanForUnregisteredDependencies Tests

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_AllDependenciesAndPackagesResolved_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => _ = d.AddImplementationType<DependencyImplementation>());
            _ = this.m_DependencyCollection.AddRequiredPackage("Package").ResolveRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractDependencyRegisteredAsImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped<IDependency>();

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractDependencyRegisteredAsImplementation_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped<DependencyImplementation>();

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractDependencyUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractDependencyUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(DependencyImplementation), d => d.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NotAllDependenciesAndPackagesResolved_ThrowsException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => { });
            _ = this.m_DependencyCollection.AddRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        #endregion Validate Tests

    }

    public class ClosedGenericImplementation : IGenericDependency<object> { }

    public class ClosedGenericImplementation2 : IGenericDependency<object> { }

    public class DependencyImplementation : IDependency { }

    public class DependencyImplementation2 : IDependency { }

    public interface IDependency { }

    public interface IGenericDependency<TGeneric> { }

    public class OpenGenericImplementation<TGeneric> : IGenericDependency<TGeneric> { }

    public class OtherOpenGenericImplementation<TGeneric> { }

}
