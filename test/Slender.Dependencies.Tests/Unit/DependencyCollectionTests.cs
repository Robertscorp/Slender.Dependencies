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

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));

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

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object));

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

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));

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

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object));

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

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));

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
            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan2);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericImplementationWithOpenGenericDependency_RegistersImplementationAgainstOpenGenericDependency()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));

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
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations().HasBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - AddDependencies Tests - - - - - -

        [Fact]
        public void AddDependencies_AddingWithExistingDependency_UsesExistingBehaviourToMerge()
        {
            // Arrange
            var _Builder = default(DependencyBuilder);
            var _Collection = new DependencyCollection().AddScoped(typeof(object));
            var _Dependency = _Collection.Single();

            _ = this.m_DependencyCollection.AddScoped(typeof(object), d => _Builder = d.HasBehaviour(this.m_MockDependencyBehaviour.Object));

            // Act
            _ = this.m_DependencyCollection.AddDependencies(_Collection);

            // Assert
            this.m_MockDependencyBehaviour.Verify(mock => mock.MergeDependencies(_Builder, _Dependency), Times.Once());
            this.m_MockDependencyBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddDependencies_AddingWithNoExistingDependency_AddsIncomingDependency()
        {
            // Arrange
            var _Collection = new DependencyCollection().AddScoped(typeof(object));
            var _Expected = new[] { new Dependency(typeof(object)) { Lifetime = DependencyLifetime.Scoped() } };

            // Act
            _ = this.m_DependencyCollection.AddDependencies(_Collection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddingWithExistingImplementations_AddsIncomingImplementationsToExistingDependency()
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
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Act
            _ = this.m_DependencyCollection.AddDependencies(_Collection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        [Fact]
        public void AddDependencies_AddingWithNoExistingImplementations_AddsIncomingImplementations()
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
            _ = this.m_DependencyCollection.AddDependencies(_Collection);
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Single().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_BothCollectionsHaveTransitiveDependencies_AddsUniqueTransitiveDependencies()
        {
            // Arrange
            var _DependencyCollection = new DependencyCollection().AddTransitiveDependency("DependencyA").AddTransitiveDependency("DependencyB");

            _ = this.m_DependencyCollection.AddTransitiveDependency("DependencyA").AddTransitiveDependency("DependencyC");

            var _Expected = new[] { "DependencyA", "DependencyC", "DependencyB" };

            // Act
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.GetUnresolvedTransitiveDependencies().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddOpenGenericDependencyIntoUnregisteredClosedGenericDependency_RegistersClosedGenericDependency()
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
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddUnregisteredClosedGenericDependencyIntoOpenGenericDependency_RegistersClosedGenericDependency()
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
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddAutoRegisteredClosedGenericDependencyIntoManuallyRegisteredDependency_ClosedGenericDependencyRemainsUnlinkedFromOpenGenericDependencyBehaviour()
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
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IGenericDependency<>), d
                    => d.HasBehaviour(_DependencyBehaviour)
                        .HasLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddManuallyRegisteredDependencyIntoAutoRegisteredClosedGenericDependency_ClosedGenericDependencyNoLongerLinkedToOpenGenericDependencyBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _DependencyBehaviour = new Mock<IDependencyBehaviour>().Object;

            var _DependencyCollection = new DependencyCollection().AddAssemblyScan(this.m_AssemblyScan);
            _ = _DependencyCollection.AddScoped(typeof(IGenericDependency<object>), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

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
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.ConfigureDependency(typeof(IGenericDependency<>), d
                    => d.HasBehaviour(_DependencyBehaviour)
                        .HasLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddDependencies_AddIncomingUnregisteredDependencyIntoExistingUnregisteredDependency_InsertsIncomingImplementationTypesIntoStartOfImplementationTypesOfExistingUnregisteredDependency()
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
            _ = this.m_DependencyCollection.AddDependencies(_DependencyCollection);
            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        #endregion AddDependencies Tests

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
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<object>), DependencyLifetime.Scoped(), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

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
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<>), DependencyLifetime.Scoped(), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

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
            _ = this.m_DependencyCollection.AddDependency(typeof(IGenericDependency<object>), DependencyLifetime.Scoped(), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

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
                    d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object).ScanForImplementations());

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

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object));
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
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.HasBehaviour(this.m_MockDependencyBehaviour.Object));

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
                        => d.HasBehaviour(this.m_MockDependencyBehaviour.Object)
                            .HasLifetime(DependencyLifetime.Singleton()));

            // Assert
            _ = this.m_DependencyCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void ConfigureDependency_ConfiguringManuallyRegisteredDependencyToFindExistingManuallyAddedImplementation_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(DependencyImplementation));

            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => d.HasImplementationType<DependencyImplementation>());
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
                        => d.HasBehaviour(this.m_MockDependencyBehaviour.Object)
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
                .AddScoped(typeof(IGenericDependency<>), d => d.HasImplementationType(typeof(OpenGenericImplementation<>)))
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
                .AddSingleton(typeof(IGenericDependency<>), d => d.HasImplementationInstance(string.Empty))
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

            _ = this.m_DependencyCollection.AddScoped(typeof(IGenericDependency<>), d => d.HasImplementationFactory(_Factory));

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

        #region - - - - - - GetUnresolvedTransitiveDependencies Tests - - - - - -

        [Fact]
        public void GetUnresolvedTransitiveDependencies_AnyRequest_ReturnsAllUnresolvedTransitiveDependencies()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddTransitiveDependency("DependencyA");

            var _Expected = new[] { "DependencyA" };

            // Act
            var _Actual = this.m_DependencyCollection.GetUnresolvedTransitiveDependencies();

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_Expected);
        }

        #endregion GetUnresolvedTransitiveDependencies Tests

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
        public void Validate_AllDependenciesResolved_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => _ = d.HasImplementationType<DependencyImplementation>());
            _ = this.m_DependencyCollection.AddTransitiveDependency("Dependency").ResolveTransitiveDependency("Dependency");

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
        public void Validate_NotAllDependenciesResolved_ThrowsException()
        {
            // Arrange
            _ = this.m_DependencyCollection.AddScoped(typeof(IDependency), d => { });
            _ = this.m_DependencyCollection.AddTransitiveDependency("Dependency");

            // Act
            var _Exception = Record.Exception(() => this.m_DependencyCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        #endregion Validate Tests

    }

    public class ClosedGenericImplementation : IGenericDependency<object> { }

    public class ClosedGenericImplementation2 : IGenericDependency<object> { }

    public class Decorator : IDependency { }

    public class DependencyImplementation : IDependency { }

    public class DependencyImplementation2 : IDependency { }

    public interface IDependency { }

    public interface IGenericDependency<TGeneric> { }

    public class OpenGenericImplementation<TGeneric> : IGenericDependency<TGeneric> { }

    public class OtherOpenGenericImplementation<TGeneric> { }

}
