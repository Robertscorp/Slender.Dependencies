using Slender.AssemblyScanner;
using Slender.ServiceRegistrations.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A collection of registered services that can be used to configure a dependency injection container.
    /// </summary>
    public partial class RegistrationCollection : IEnumerable<Registration>
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, RegistrationBuilder> m_BuildersByType = new Dictionary<Type, RegistrationBuilder>();
        private readonly Dictionary<Type, IEnumerable<Type>> m_ImplementationsByType = new Dictionary<Type, IEnumerable<Type>>();
        private readonly List<string> m_RequiredPackages = new List<string>();

        private IAssemblyScan m_AssemblyScan = AssemblyScan.Empty();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Creates a new instance of a registered service collection.
        /// </summary>
        public RegistrationCollection() { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds the assemblies and scans them for implementations of registered services.
        /// </summary>
        /// <param name="assemblies">The assemblies to add.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection AddAssemblies(IEnumerable<Assembly> assemblies)
            => this.AddAssemblyScan(AssemblyScan.FromAssemblies(assemblies));

        /// <summary>
        /// Adds the assemblies and scans them for implementations of registered services.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        /// <param name="additionalAssemblies">Additional assemblies to add.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection AddAssemblies(Assembly assembly, params Assembly[] additionalAssemblies)
            => this.AddAssemblyScan(AssemblyScan.FromAssemblies(assembly, additionalAssemblies));

        /// <summary>
        /// Adds the assembly and scans it for implementations of registered services.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection AddAssembly(Assembly assembly)
            => this.AddAssemblies(assembly);

        /// <summary>
        /// Adds the assembly scan and visits it for implementations of registered services.
        /// </summary>
        /// <param name="assemblyScan">The assembly scan to add.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection AddAssemblyScan(IAssemblyScan assemblyScan)
        {
            new ImplementationScanVisitor { OnServiceAndImplementationsFound = (s, i) => this.AddImplementations(s, i, true) }.VisitAssemblyScan(assemblyScan);

            this.m_AssemblyScan = AssemblyScan.Empty().AddAssemblyScan(this.m_AssemblyScan).AddAssemblyScan(assemblyScan);

            return this;
        }

        private void AddBuilder(RegistrationBuilder builder)
        {
            builder.OnScanForImplementations = () => this.AddImplementations(builder);

            this.m_BuildersByType.Add(builder.Registration.ServiceType, builder);
            this.AddImplementations(builder);
        }

        private void AddImplementations(RegistrationBuilder builder)
        {
            if (this.m_ImplementationsByType.TryGetValue(builder.Registration.ServiceType, out var _Implementations) && builder.Registration.AllowScannedImplementationTypes)
            {
                this.AddImplementations(builder, _Implementations);
                _ = this.m_ImplementationsByType.Remove(builder.Registration.ServiceType);
            }
        }

        private void AddImplementations(RegistrationBuilder builder, IEnumerable<Type> implementations)
        {
            foreach (var _Implementation in implementations)
                _ = builder.AddImplementationType(_Implementation);
        }

        private void AddImplementations(Type service, IEnumerable<Type> implementations, bool existingImplementationsFirst)
        {
            if (this.m_BuildersByType.TryGetValue(service, out var _Builder) && _Builder.Registration.AllowScannedImplementationTypes)
                this.AddImplementations(_Builder, implementations);
            else if (this.m_ImplementationsByType.TryGetValue(service, out var _Implementations))
                this.m_ImplementationsByType[service]
                    = existingImplementationsFirst
                        ? _Implementations.Union(implementations)
                        : implementations.Union(_Implementations);

            else
                this.m_ImplementationsByType.Add(service, implementations);
        }

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a service with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="lifetime"/> is null.</exception>
        /// <exception cref="Exception">Thrown when there is already a registered service of the specified <paramref name="type"/>.</exception>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// If the specified <paramref name="configurationAction"/> invokes <see cref="RegistrationBuilder.ScanForImplementations"/>, then any
        /// previously found types which implement the specified service <paramref name="type"/> will be automatically added through 
        /// <see cref="RegistrationBuilder.AddImplementationType(Type)"/> after <paramref name="configurationAction"/> is invoked.<br/>
        /// <br/>
        /// For more information on previously found types, see <see cref="RegistrationCollection.AddAssemblyScan(IAssemblyScan)"/>.
        /// </remarks>
        public RegistrationCollection AddService(Type type, RegistrationLifetime lifetime, Action<RegistrationBuilder> configurationAction)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            if (this.m_BuildersByType.ContainsKey(type))
                throw new Exception($"{type.Name} has already been registered. Use {nameof(ConfigureService)} instead.");

            var _Builder = new RegistrationBuilder(type, lifetime);

            configurationAction?.Invoke(_Builder);

            this.AddBuilder(_Builder);

            return this;
        }

        /// <summary>
        /// Registers an external package as being required for implementations in the registration collection.
        /// </summary>
        /// <param name="externalPackageName">The name of the external package.</param>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// This method exists so that consumers of the registration collection can be informed that an external package
        /// has been used by implementations in the registration collection.
        /// 
        /// This should only be used if the external package doesn't provide a RegistrationCollection, but instead has
        /// functionality to register itself directly into dependency injection containers.
        /// 
        /// If a required package hasn't been resolved, it will cause the validation of the registration collection to fail.
        /// </remarks>
        public RegistrationCollection AddRequiredPackage(string externalPackageName)
        {
            this.m_RequiredPackages.Add(externalPackageName);
            return this;
        }

        /// <summary>
        /// Configures a registered service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection ConfigureService(Type type, Action<RegistrationBuilder> configurationAction)
        {
            _ = this.m_BuildersByType.TryGetValue(type, out var _Registration);

            configurationAction(_Registration);

            return this;
        }

        /// <summary>
        /// Gets all required packages that have not been resolved.
        /// </summary>
        /// <returns>A read-only collection of all required packages that have not been resolved.</returns>
        public ReadOnlyCollection<string> GetUnresolvedRequiredPackages()
            => new ReadOnlyCollection<string>(this.m_RequiredPackages);

        IEnumerator<Registration> IEnumerable<Registration>.GetEnumerator()
            => this.m_BuildersByType.Values.Select(b => b.Registration).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<RegistrationBuilder>)this).GetEnumerator();

        /// <summary>
        /// Merges all registered services from the specified <paramref name="registrationCollection"/> into this registration collection.
        /// </summary>
        /// <param name="registrationCollection">The registration collection to merge in.</param>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// If a service has been registered in both collections, then the incoming service will be merged into the existing service by invoking 
        /// <see cref="IRegistrationBehaviour.MergeRegistration(RegistrationBuilder, Registration)"/> on the existing service's behaviour.<br/>
        /// <br/>
        /// Additionally, the specified <paramref name="registrationCollection"/> will be completely cleared as a part of the merge process.
        /// </remarks>
        public RegistrationCollection MergeRegistrationCollection(RegistrationCollection registrationCollection)
        {
            foreach (var _BuilderByType in registrationCollection.m_BuildersByType)
                if (this.m_BuildersByType.TryGetValue(_BuilderByType.Key, out var _Builder))
                    _Builder.Registration.Behaviour.MergeRegistration(_Builder, _BuilderByType.Value.Registration);

                else
                    this.AddBuilder(_BuilderByType.Value);

            // Take on the incoming registrationCollection's implementations first, so that
            // they will be the first to be registered when scanning is enabled.
            // This has been done because it's assumed that this collection is expanding on the
            // services and implementations of the collection being merged in.

            foreach (var _ImplementationsByType in registrationCollection.m_ImplementationsByType)
                this.AddImplementations(_ImplementationsByType.Key, _ImplementationsByType.Value, existingImplementationsFirst: false);

            foreach (var _RequiredPackage in registrationCollection.m_RequiredPackages)
                _ = this.AddRequiredPackage(_RequiredPackage);

            _ = this.AddAssemblyScan(registrationCollection.m_AssemblyScan);

            registrationCollection.m_AssemblyScan = AssemblyScan.Empty();
            registrationCollection.m_BuildersByType.Clear();
            registrationCollection.m_ImplementationsByType.Clear();
            registrationCollection.m_RequiredPackages.Clear();

            return this;
        }

        /// <summary>
        /// Informs the RegistrationCollection that the external package no longer needs to be tracked.
        /// </summary>
        /// <param name="externalPackageName">The name of the external package.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection ResolveRequiredPackage(string externalPackageName)
        {
            _ = this.m_RequiredPackages.Remove(externalPackageName);
            return this;
        }

        /// <summary>
        /// Scans already added assemblies for services that haven't been registered, and invokes the provided action with them.
        /// </summary>
        /// <param name="serviceRegistrationAction">The action to register services.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection ScanForUnregisteredServices(Action<RegistrationCollection, Type> serviceRegistrationAction)
        {
            new ServiceScanVisitor
            {
                OnServiceFound = t => { if (!this.m_BuildersByType.ContainsKey(t)) serviceRegistrationAction(this, t); }
            }.VisitAssemblyScan(this.m_AssemblyScan);

            this.m_AssemblyScan = AssemblyScan.Empty();

            return this;
        }

        /// <summary>
        /// Verifies that all registered services have an associated implementation.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <exception cref="Exception">Thrown if any services do not have any associated implementation.</exception>
        public RegistrationCollection Validate()
        {
            var _StringBuilder = new StringBuilder(64);

            var _BuildersWithoutImplementations
                = this.m_BuildersByType.Values
                    .Where(b =>
                        !b.Registration.ImplementationTypes.Any()
                        && b.Registration.ImplementationFactory == null
                        && b.Registration.ImplementationInstance == null
                        && (b.Registration.AllowScannedImplementationTypes || b.Registration.ServiceType.IsAbstract))
                    .ToList();

            var _BuildersWithInvalidImplementationTypes
                = this.m_BuildersByType.Values.Where(b => b.Registration.ImplementationTypes.Any(t => t.IsAbstract));

            if (_BuildersWithoutImplementations.Any())
            {
                _ = _StringBuilder.AppendLine("The following services don't have any implementations:");

                foreach (var _Builder in _BuildersWithoutImplementations)
                    _ = _StringBuilder.Append(" - ").AppendLine(_Builder.Registration.ServiceType.Name);
            }

            if (_BuildersWithInvalidImplementationTypes.Any())
            {
                _ = _StringBuilder.AppendLine("The following services have invalid implementation types:");

                foreach (var _Builder in _BuildersWithInvalidImplementationTypes)
                    _ = _StringBuilder
                            .Append(" - ")
                            .Append(_Builder.Registration.ServiceType.Name)
                            .AppendLine($" ({string.Join(", ", _Builder.Registration.ImplementationTypes.Where(r => r.IsAbstract).Select(r => r.Name))})");
            }

            if (this.m_RequiredPackages.Any())
            {
                if (_StringBuilder.Length > 0)
                    _ = _StringBuilder.AppendLine();

                _ = _StringBuilder.AppendLine("The following packages are required and have not been resolved:");

                foreach (var _RequiredPackage in this.m_RequiredPackages)
                    _ = _StringBuilder.Append(" - ").AppendLine(_RequiredPackage);
            }

            return _StringBuilder.Length == 0 ? this : throw new Exception(_StringBuilder.ToString());
        }

        #endregion Methods

    }

}
