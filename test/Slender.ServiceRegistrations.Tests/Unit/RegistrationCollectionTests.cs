using FluentAssertions;
using Moq;
using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IRegistrationBehaviour> m_MockRegistrationBehaviour = new();
        private readonly Mock<Action<RegistrationCollection, Type>> m_MockScanningBehaviour = new();

        private readonly IAssemblyScan m_AssemblyScan;
        private readonly IAssemblyScan m_AssemblyScan2;
        private readonly List<Type> m_AssemblyTypes = new();
        private readonly List<Type> m_AssemblyTypes2 = new();
        private readonly RegistrationCollection m_RegistrationCollection = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationCollectionTests()
        {
            var _MockAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes.ToArray());

            var _MockAssemblyScan2 = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan2
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes2.ToArray());

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Registration>(), It.IsAny<Type>()))
                    .Callback((Registration r, Type t) => r.ImplementationTypes.Add(t));

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<Registration>()))
                    .Callback((Registration registration) => registration.AllowScannedImplementationTypes = true);

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.UpdateLifetime(It.IsAny<Registration>(), It.IsAny<RegistrationLifetime>()))
                    .Callback((Registration r, RegistrationLifetime l) => r.Lifetime = l);

            this.m_AssemblyScan = _MockAssemblyScan.Object;
            this.m_AssemblyScan2 = _MockAssemblyScan2.Object;
        }

        #endregion Constructors

        #region - - - - - - AddAssemblyScan Tests - - - - - -

        [Fact]
        public void AddAssemblyScan_AddingScanWithRegistrationThatAllowsScannedImplementations_ScannedImplementationsAdded()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Registration>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithClosedGenericServiceWithoutScanning_DoesNotAddImplementationToService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<object>), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithClosedGenericServiceWithScanning_AddsImplementationToService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<object>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithOpenGenericServiceWithoutScanning_DoesNotRegisterClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<>))
                {
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithOpenGenericServiceWithScanning_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericClass_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(OtherOpenGenericImplementation<>));

            var _Expected = Array.Empty<Type>();

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationsOverMultipleScansWithOpenGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));
            this.m_AssemblyTypes2.Add(typeof(ClosedGenericImplementation2));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>
                    {
                        typeof(ClosedGenericImplementation),
                        typeof(ClosedGenericImplementation2)
                    },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan2);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericImplementationWithOpenGenericService_RegistersImplementationAgainstOpenGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingEmptyScanWithRegistrationThatAllowsScannedImplementations_DoesNothing()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Assert
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - AddService Tests - - - - - -

        [Fact]
        public void AddService_NotSpecifyingServiceType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_RegistrationCollection.AddService(null, RegistrationLifetime.Scoped(), r => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddService_NotSpecifyingLifetime_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_RegistrationCollection.AddService(typeof(IService), null, r => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddService_NotSpecifyingConfigurationAction_RegistersService()
            => this.m_RegistrationCollection
                .AddService(typeof(IService), RegistrationLifetime.Scoped(), null)
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Registration(typeof(IService)) { Lifetime = RegistrationLifetime.Scoped() }
                });

        [Fact]
        public void AddService_AddingAlreadyAddedService_ThrowsInvalidOperationException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddService(typeof(IService), RegistrationLifetime.Scoped(), r => { });

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.AddService(typeof(IService), RegistrationLifetime.Scoped(), r => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void AddService_AddingAutoRegisteredClosedGenericService_ThrowsInvalidOperationException()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddService(typeof(IGenericService<>), RegistrationLifetime.Scoped(), r => r.ScanForImplementations());
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.AddService(typeof(IGenericService<object>), RegistrationLifetime.Scoped(), r => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void AddService_AddingClosedGenericServiceWithScanning_AddsClosedGenericImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddService(typeof(IGenericService<object>), RegistrationLifetime.Scoped(), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddService_AddingOpenGenericServiceWithScanning_DiscoversAndAutoRegistersClosedGenericServices()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddService(typeof(IGenericService<>), RegistrationLifetime.Scoped(), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddService_AddingUnregisteredService_BecomesRegisteredServiceWithSpecifiedLifetime()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddService(typeof(IGenericService<object>), RegistrationLifetime.Scoped(), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddService_AddingUnregisteredServiceWithImplementationsOverMultipleAssembles_ImplementationsAddedInScanOrder()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation));
            this.m_AssemblyTypes2.Add(typeof(ServiceImplementation2));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan2);

            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>()
                    {
                        typeof(ServiceImplementation),
                        typeof(ServiceImplementation2)
                    },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddService(
                    typeof(IService),
                    RegistrationLifetime.Scoped(),
                    r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        #endregion AddService Tests

        #region - - - - - - ConfigureService Tests - - - - - -

        [Fact]
        public void ConfigureService_NotSpecifyingServiceType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_RegistrationCollection.ConfigureService(null, r => { }))
                .Should().BeOfType<ArgumentNullException>();

        [Fact]
        public void ConfigureService_NotSpecifyingConfigurationAction_ThrowsArgumentNullException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddService(typeof(IService), RegistrationLifetime.Scoped(), null);

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.ConfigureService(typeof(IService), null));

            // Assert
            _ = _Exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ConfigureService_ConfigureNeverAddedService_ThrowsInvalidOperationException()
            => Record
                .Exception(() => this.m_RegistrationCollection.ConfigureService(typeof(IService), r => { }))
                .Should().BeOfType<InvalidOperationException>();

        [Fact]
        public void ConfigureService_ConfigureUnregisteredService_ThrowsInvalidOperationException()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(IService));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.ConfigureService(typeof(IService), r => { }));

            // Assert
            _ = _Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void ConfigureService_EnablingAllowScanOnRegistrationWithPreScannedImplementations_EnablesAllowScanAndAddsImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IService), r => r.ScanForImplementations());

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<Registration>()));
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Registration>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ConfigureService_EnablingAllowScanOnRegistrationWithNoPreScannedImplementations_EnablesAllowScan()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IService), r => r.ScanForImplementations());

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(It.IsAny<Registration>()));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ConfigureService_ConfiguringOpenGenericServiceToAllowScan_DiscoversAndAutoRegistersClosedGenericServices()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)},
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IGenericService<>), r => r.ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void ConfigureService_ConfiguringAutoRegisteredClosedGenericService_NoLongerLinkedToOpenGenericBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                    Lifetime = RegistrationLifetime.Scoped()
                },
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Singleton()
                }
            };

            // Act
            _ = this.m_RegistrationCollection
                    .ConfigureService(typeof(IGenericService<object>), r
                        => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object)
                            .WithLifetime(RegistrationLifetime.Singleton()));

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void ConfigureService_ConfiguringManuallyRegisteredServiceToFindExistingManuallyAddedImplementation_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.AddImplementationType<ServiceImplementation>());
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ServiceImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection
                    .ConfigureService(typeof(IService), r
                        => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object)
                            .ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion ConfigureService Tests

        #region - - - - - - GetEnumerator Tests - - - - - -

        [Fact]
        public void GetEnumerator_EmptyNonGenericService_GetsIncludedInEnumerator()
            => this.m_RegistrationCollection
                .AddScoped(typeof(IService))
                .Should()
                .BeEquivalentTo(new[] { new Registration(typeof(IService)) { Lifetime = RegistrationLifetime.Scoped() } });

        [Fact]
        public void GetEnumerator_OpenGenericServiceWithOnlyAllowScan_DoesNotGetIncludedInEnumerator()
            => this.m_RegistrationCollection
                .AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations())
                .Should()
                .BeEquivalentTo(Array.Empty<Registration>());

        [Fact]
        public void GetEnumerator_EmptyOpenGenericService_GetsIncludedInEnumerator()
            => this.m_RegistrationCollection
                .AddScoped(typeof(IGenericService<>))
                .Should()
                .BeEquivalentTo(new[] { new Registration(typeof(IGenericService<>)) { Lifetime = RegistrationLifetime.Scoped() } });

        [Fact]
        public void GetEnumerator_OpenGenericServiceWithImplementationType_GetsIncludedInEnumerator()
            => this.m_RegistrationCollection
                .AddScoped(typeof(IGenericService<>), r => r.AddImplementationType(typeof(OpenGenericImplementation<>)))
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Registration(typeof(IGenericService<>))
                    {
                        ImplementationTypes = new List<Type>() { typeof(OpenGenericImplementation<>) },
                        Lifetime = RegistrationLifetime.Scoped()
                    }
                });

        [Fact]
        public void GetEnumerator_OpenGenericServiceWithImplementationInstance_GetsIncludedInEnumerator()
            => this.m_RegistrationCollection
                .AddSingleton(typeof(IGenericService<>), r => r.WithImplementationInstance(string.Empty))
                .Should()
                .BeEquivalentTo(new[]
                {
                    new Registration(typeof(IGenericService<>))
                    {
                        ImplementationInstance = string.Empty,
                        Lifetime = RegistrationLifetime.Singleton()
                    }
                });

        [Fact]
        public void GetEnumerator_OpenGenericServiceWithImplementationFactory_GetsIncludedInEnumerator()
        {
            // Arrange
            var _Factory = (Func<ServiceFactory, object>)(serviceFactory => string.Empty);

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.WithImplementationFactory(_Factory));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<>))
                {
                    ImplementationFactory = _Factory,
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion GetEnumerator Tests

        #region - - - - - - GetUnresolvedRequiredPackages Tests - - - - - -

        [Fact]
        public void GetUnresolvedRequiredPackages_AnyRequest_ReturnsAllUnresolvedRequiredPackages()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddRequiredPackage("PackageA");

            var _Expected = new[] { "PackageA" };

            // Act
            var _Actual = this.m_RegistrationCollection.GetUnresolvedRequiredPackages();

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_Expected);
        }

        #endregion GetUnresolvedRequiredPackages Tests

        #region - - - - - - MergeRegistrationCollection Tests - - - - - -

        [Fact]
        public void MergeRegistrationCollection_MergingWithExistingRegistration_UsesExistingRegistrationsBehaviourToMerge()
        {
            // Arrange
            var _Builder = default(RegistrationBuilder);
            var _Collection = new RegistrationCollection().AddScoped(typeof(object));
            var _Registration = _Collection.Single();

            _ = this.m_RegistrationCollection.AddScoped(typeof(object), r => _Builder = r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_Collection);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.MergeRegistration(_Builder, _Registration), Times.Once());
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void MergeRegistrationCollection_MergingWithNoExistingRegistration_AddsIncomingRegistration()
        {
            // Arrange
            var _Collection = new RegistrationCollection().AddScoped(typeof(object));
            var _Expected = new[] { new Registration(typeof(object)) { Lifetime = RegistrationLifetime.Scoped() } };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_Collection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergingWithExistingImplementations_AddsIncomingImplementationsToExistingService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation));
            this.m_AssemblyTypes2.Add(typeof(ServiceImplementation2));

            var _Collection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan2);
            var _Expected = new[]
            {
                new Registration(typeof(IService))
                {
                    AllowScannedImplementationTypes = true,
                    ImplementationTypes = new List<Type>() { typeof(ServiceImplementation), typeof(ServiceImplementation2) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_Collection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        [Fact]
        public void MergeRegistrationCollection_MergingWithNoExistingImplementations_AddsIncomingImplementations()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ServiceImplementation2));

            var _Collection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan);
            var _Expected = new Registration(typeof(IService))
            {
                AllowScannedImplementationTypes = true,
                ImplementationTypes = new List<Type>() { typeof(ServiceImplementation2) },
                Lifetime = RegistrationLifetime.Scoped()
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_Collection);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Single().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergingRequiredPackages_AddsIncomingRequiredPackages()
        {
            // Arrange
            var _RegistrationCollection = new RegistrationCollection().AddRequiredPackage("PackageA").AddRequiredPackage("PackageB");

            _ = this.m_RegistrationCollection.AddRequiredPackage("PackageA").AddRequiredPackage("PackageC");

            var _Expected = new[] { "PackageA", "PackageC", "PackageA", "PackageB" };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = this.m_RegistrationCollection.GetUnresolvedRequiredPackages().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeOpenGenericServiceIntoUnregisteredClosedGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationCollection = new RegistrationCollection().AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeUnregisteredClosedGenericServiceIntoOpenGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan);

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeAutoRegisteredClosedGenericServiceIntoManuallyRegisteredService_ClosedGenericServiceRemainsUnlinkedFromOpenGenericServiceBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationBehaviour = new Mock<IRegistrationBehaviour>().Object;

            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan);
            _ = _RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<object>), r => r.ScanForImplementations());

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IGenericService<>), r
                    => r.WithRegistrationBehaviour(_RegistrationBehaviour)
                        .WithLifetime(RegistrationLifetime.Singleton()));

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeManuallyRegisteredServiceIntoAutoRegisteredClosedGenericService_ClosedGenericServiceNoLongerLinkedToOpenGenericServiceBehaviour()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationBehaviour = new Mock<IRegistrationBehaviour>().Object;

            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan);
            _ = _RegistrationCollection.AddScoped(typeof(IGenericService<object>), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IGenericService<>), r
                    => r.WithRegistrationBehaviour(_RegistrationBehaviour)
                        .WithLifetime(RegistrationLifetime.Singleton()));

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeTwoUnregisteredServices_InsertsIncomingImplementationTypesIntoStartOfUnregisteredServicesImplementationTypes()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation2));
            this.m_AssemblyTypes2.Add(typeof(ClosedGenericImplementation));

            var _RegistrationBehaviour = new Mock<IRegistrationBehaviour>().Object;
            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(this.m_AssemblyScan2);

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation), typeof(ClosedGenericImplementation2) },
                    Lifetime = RegistrationLifetime.Scoped()
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object).ScanForImplementations());

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected, opts => opts.WithStrictOrdering());
        }

        #endregion MergeRegistrationCollection Tests

        #region - - - - - - ScanForUnregisteredServices Tests - - - - - -

        [Fact]
        public void ScanForUnregisteredServices_ManuallyRegisteredService_NotScanned()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => { });

            // Act
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredServices_AutomaticallyRegisteredService_NotScanned()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredServices_UnregisteredService_IsScanned()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.Verify(mock => mock.Invoke(this.m_RegistrationCollection, typeof(IGenericService<object>)), Times.Once());
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void ScanForUnregisteredServices_InvokedMultipleTimesWithUnregisteredService_IsScannedMultipleTimes()
        {
            // Arrange
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_AssemblyScan);

            // Act
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);
            _ = this.m_RegistrationCollection.ScanForUnregisteredServices(this.m_MockScanningBehaviour.Object);

            // Assert
            this.m_MockScanningBehaviour.Verify(mock => mock.Invoke(this.m_RegistrationCollection, typeof(IGenericService<object>)), Times.Exactly(3));
            this.m_MockScanningBehaviour.VerifyNoOtherCalls();
        }

        #endregion ScanForUnregisteredServices Tests

        #region - - - - - - Validate Tests - - - - - -

        [Fact]
        public void Validate_AllServicesAndPackagesResolved_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => _ = r.AddImplementationType<ServiceImplementation>());
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package").ResolveRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractServiceRegisteredAsImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped<IService>();

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractServiceRegisteredAsImplementation_DoesNotThrowException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped<ServiceImplementation>();

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        [Fact]
        public void Validate_AbstractServiceUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NonAbstractServiceUnableToFindScannedImplementation_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(ServiceImplementation), r => r.ScanForImplementations());

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        [Fact]
        public void Validate_NotAllServicesAndPackagesResolved_ThrowsException()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => { });
            _ = this.m_RegistrationCollection.AddRequiredPackage("Package");

            // Act
            var _Exception = Record.Exception(() => this.m_RegistrationCollection.Validate());

            // Assert
            _ = _Exception.Should().NotBeNull();
        }

        #endregion Validate Tests

    }

    public class ClosedGenericImplementation : IGenericService<object> { }

    public class ClosedGenericImplementation2 : IGenericService<object> { }

    public interface IGenericService<TGeneric> { }

    public interface IService { }

    public class OpenGenericImplementation<TGeneric> : IGenericService<TGeneric> { }

    public class OtherOpenGenericImplementation<TGeneric> { }

    public class ServiceImplementation : IService { }

    public class ServiceImplementation2 : IService { }

}
