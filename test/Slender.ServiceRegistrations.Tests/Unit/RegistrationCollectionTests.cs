using FluentAssertions;
using Moq;
using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public partial class RegistrationCollectionTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly Mock<IAssemblyScan> m_MockAssemblyScan = new();
        private readonly Mock<IRegistrationBehaviour> m_MockRegistrationBehaviour = new();

        private readonly List<Type> m_AssemblyTypes = new() { typeof(ServiceImplementation) };
        private readonly RegistrationCollection m_RegistrationCollection = new();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationCollectionTests()
        {
            _ = this.m_MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(() => this.m_AssemblyTypes.ToArray());

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AddImplementationType(It.IsAny<Registration>(), It.IsAny<Type>()))
                    .Callback((Registration r, Type t) => r.ImplementationTypes.Add(t));

            _ = this.m_MockRegistrationBehaviour
                    .Setup(mock => mock.AllowScannedImplementationTypes(It.IsAny<Registration>()))
                    .Callback((Registration registration) => registration.AllowScannedImplementationTypes = true);
        }

        #endregion Constructors

        #region - - - - - - AddAssemblyScan Tests - - - - - -

        [Fact]
        public void AddAssemblyScan_AddingScanWithRegistrationThatAllowsScannedImplementations_ScannedImplementationsAdded()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            this.m_MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(It.IsAny<Registration>(), typeof(ServiceImplementation)));
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationWithOpenGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>{ typeof(ClosedGenericImplementation) }
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingClosedGenericImplementationsOverMultipleScansWithOpenGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _AssemblyScan = new Mock<IAssemblyScan>();
            _ = _AssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(new[] { typeof(ClosedGenericImplementation2) });

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
                    }
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(_AssemblyScan.Object);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingOpenGenericImplementationWithOpenGenericService_RegistersImplementationAgainstOpenGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(OpenGenericImplementation<>));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>{ typeof(OpenGenericImplementation<>) }
                }
            };

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void AddAssemblyScan_AddingEmptyScanWithRegistrationThatAllowsScannedImplementations_DoesNothing()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();

            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Act
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Assert
            this.m_MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion AddAssemblyScan Tests

        #region - - - - - - ConfigureService Tests - - - - - -

        [Fact]
        public void ConfigureService_EnablingAllowScanOnRegistrationWithPreScannedImplementations_EnablesAllowScanAndAddsImplementations()
        {
            // Arrange
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

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
        public void ConfigureService_EnablingAllowScanOnOpenGenericRegistrationWithClosedGenericImplementations_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));
            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation)}
                }
            };

            // Act
            _ = this.m_RegistrationCollection.ConfigureService(typeof(IGenericService<>), r => r.ScanForImplementations());

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
        public void MergeRegistrationCollection_MergingWithExistingImplementations_AddsIncomingImplementationsFirst()
        {
            // Arrange
            var _MockOtherAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockOtherAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(new[] { typeof(ServiceImplementation2) });

            var _Collection = new RegistrationCollection().AddAssemblyScan(_MockOtherAssemblyScan.Object);
            var _Expected = new Registration(typeof(IService))
            {
                AllowScannedImplementationTypes = true,
                ImplementationTypes = new List<Type>() { typeof(ServiceImplementation2), typeof(ServiceImplementation) },
                Lifetime = RegistrationLifetime.Scoped()
            };

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_Collection);
            _ = this.m_RegistrationCollection.AddScoped(typeof(IService), r => r.ScanForImplementations().WithRegistrationBehaviour(this.m_MockRegistrationBehaviour.Object));

            // Assert
            _ = this.m_RegistrationCollection.Single().Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergingWithNoExistingImplementations_AddsIncomingImplementations()
        {
            // Arrange
            var _MockOtherAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockOtherAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(new[] { typeof(ServiceImplementation2) });

            var _Collection = new RegistrationCollection().AddAssemblyScan(_MockOtherAssemblyScan.Object);
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
        public void MergeRegistrationCollection_MergingAssemblyScans_AddsIncomingAssemblyScan()
        {
            // Arrange
            var _AssemblyScanInfo = typeof(RegistrationCollection).GetField("m_AssemblyScan", BindingFlags.Instance | BindingFlags.NonPublic)!;

            var _MockAssemblyScan = new Mock<IAssemblyScan>();
            _ = _MockAssemblyScan
                    .Setup(mock => mock.Types)
                    .Returns(new[] { typeof(ServiceImplementation2) });

            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(_MockAssemblyScan.Object);

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            var _Expected = AssemblyScan.Empty().AddAssemblyScan(this.m_MockAssemblyScan.Object).AddAssemblyScan(_MockAssemblyScan.Object);

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = _AssemblyScanInfo.GetValue(this.m_RegistrationCollection).Should().BeEquivalentTo(_Expected);

        }

        [Fact]
        public void MergeRegistrationCollection_MergeOpenGenericServiceIntoAlreadyRegisteredClosedGenericImplementation_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationCollection = new RegistrationCollection().AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            _ = this.m_RegistrationCollection.AddAssemblyScan(this.m_MockAssemblyScan.Object);

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) }
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        [Fact]
        public void MergeRegistrationCollection_MergeClosedGenericImplementationIntoAlreadyRegisteredOpenGenericService_RegistersClosedGenericService()
        {
            // Arrange
            this.m_AssemblyTypes.Clear();
            this.m_AssemblyTypes.Add(typeof(ClosedGenericImplementation));

            var _RegistrationCollection = new RegistrationCollection().AddAssemblyScan(this.m_MockAssemblyScan.Object);

            _ = this.m_RegistrationCollection.AddScoped(typeof(IGenericService<>), r => r.ScanForImplementations());

            var _Expected = new[]
            {
                new Registration(typeof(IGenericService<object>))
                {
                    AllowScannedImplementationTypes = true,
                    Behaviour = this.m_MockRegistrationBehaviour.Object,
                    ImplementationTypes = new List<Type>() { typeof(ClosedGenericImplementation) }
                }
            };

            // Act
            _ = this.m_RegistrationCollection.MergeRegistrationCollection(_RegistrationCollection);

            // Assert
            _ = this.m_RegistrationCollection.Should().BeEquivalentTo(_Expected);
        }

        #endregion MergeRegistrationCollection Tests

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

    public class ServiceImplementation : IService { }

    public class ServiceImplementation2 : IService { }

}
