using Slender.AssemblyScanner;
using Slender.ServiceRegistrations.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A collection of service registrations that can be used to configure a dependency injection container.
    /// </summary>
    public partial class RegistrationCollection : IEnumerable<Registration>
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, IEnumerable<Type>> m_ImplementationsByType = new Dictionary<Type, IEnumerable<Type>>();
        private readonly Dictionary<Type, Registration> m_RegistrationsByType = new Dictionary<Type, Registration>();
        private readonly List<string> m_RequiredPackages = new List<string>();

        private IAssemblyScan m_AssemblyScan = AssemblyScan.Empty();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Creates a new instance of a service registration collection.
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
            new ImplementationScanVisitor { OnServiceAndImplementationsFound = this.AddImplementations }.VisitAssemblyScan(assemblyScan);

            this.m_AssemblyScan = AssemblyScan.Empty().AddAssemblyScan(this.m_AssemblyScan).AddAssemblyScan(assemblyScan);

            return this;
        }

        private void AddImplementations(Registration registration)
        {
            if (this.m_ImplementationsByType.TryGetValue(registration.ServiceType, out var _Implementations) && registration.AllowScannedImplementationTypes)
            {
                this.AddImplementations(registration, _Implementations);
                _ = this.m_ImplementationsByType.Remove(registration.ServiceType);
            }
        }

        private void AddImplementations(Registration registration, IEnumerable<Type> implementations)
        {
            foreach (var _Implementation in implementations)
                _ = registration.AddImplementationType(_Implementation);
        }

        private void AddImplementations(Type service, IEnumerable<Type> implementations)
        {
            if (this.m_RegistrationsByType.TryGetValue(service, out var _Registration) && _Registration.AllowScannedImplementationTypes)
                this.AddImplementations(_Registration, implementations);
            else if (this.m_ImplementationsByType.TryGetValue(service, out var _Implementations))
                this.m_ImplementationsByType[service] = _Implementations.Union(implementations);
            else
                this.m_ImplementationsByType.Add(service, implementations);
        }

        private void AddRegistration(Registration registration)
        {
            registration.OnScanForImplementations = () => this.AddImplementations(registration);

            this.m_RegistrationsByType.Add(registration.ServiceType, registration);
            this.AddImplementations(registration);
        }

        /// <summary>
        /// Adds all registered services from the provided registration collection to this registration collection.
        /// </summary>
        /// <param name="registrations">The registration collection to add.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="Exception">Thrown when a service in the provided registration collection already exists in this registration collection.</exception>
        public RegistrationCollection AddRegistrationCollection(RegistrationCollection registrations)
        {
            var _Registrations = registrations.ToArray();

            var _ExistingRegistration = _Registrations.Where(r => this.m_RegistrationsByType.ContainsKey(r.ServiceType)).ToArray();
            if (_ExistingRegistration.Any())
            {
                var _StringBuilder = new StringBuilder(64);
                _ = _StringBuilder.AppendLine("Could not add Registration Collection, as the following services are already registered:");

                foreach (var _Registration in _ExistingRegistration)
                    _ = _StringBuilder.Append(" - ").AppendLine(_Registration.ServiceType.Name);

                _ = _StringBuilder.AppendLine().AppendLine("This may be caused by scanning for registrations, or the order of your registrations.");

                throw new Exception(_StringBuilder.ToString());
            }

            foreach (var _Registration in _Registrations)
                this.AddRegistration(_Registration);

            foreach (var _ImplementationsByType in registrations.m_ImplementationsByType)
                this.AddImplementations(_ImplementationsByType.Key, _ImplementationsByType.Value);

            foreach (var _RequiredPackage in registrations.m_RequiredPackages)
                _ = this.AddRequiredPackage(_RequiredPackage);

            return this.AddAssemblyScan(registrations.m_AssemblyScan);
        }

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a service <see cref="Registration"/> with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="lifetime"/> is null.</exception>
        /// <exception cref="Exception">Thrown when a <see cref="Registration"/> already exists for the specified <paramref name="type"/>.</exception>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// If the specified <paramref name="configurationAction"/> invokes <see cref="Registration.ScanForImplementations"/>, then any
        /// previously found types which implement the specified service <paramref name="type"/> will be automatically added through 
        /// <see cref="Registration.AddImplementationType(Type)"/> after <paramref name="configurationAction"/> is invoked.<br/>
        /// <br/>
        /// For more information on previously found types, see <see cref="RegistrationCollection.AddAssemblyScan(IAssemblyScan)"/>.
        /// </remarks>
        public RegistrationCollection AddService(Type type, RegistrationLifetime lifetime, Action<Registration> configurationAction)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            if (this.m_RegistrationsByType.ContainsKey(type))
                throw new Exception($"{type.Name} has already been registered. Use {nameof(ConfigureService)} instead.");

            var _Registration = new Registration(type, lifetime);

            configurationAction?.Invoke(_Registration);

            this.AddRegistration(_Registration);

            return this;
        }

        /// <summary>
        /// Registers <typeparamref name="TService"/> as a singleton service.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddSingletonService<TService>(Action<Registration> configurationAction = null)
            => this.AddSingletonService(typeof(TService), configurationAction);

        /// <summary>
        /// Registers the specified <see cref="Type"/> as a singleton service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddSingletonService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Singleton(), configurationAction);

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
        public RegistrationCollection ConfigureService(Type type, Action<Registration> configurationAction)
        {
            _ = this.m_RegistrationsByType.TryGetValue(type, out var _Registration);

            configurationAction(_Registration);

            return this;
        }

        IEnumerator<Registration> IEnumerable<Registration>.GetEnumerator()
            => this.m_RegistrationsByType.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Registration>)this).GetEnumerator();

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
                OnServiceFound = t => { if (!this.m_RegistrationsByType.ContainsKey(t)) serviceRegistrationAction(this, t); }
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

            var _RegistrationsWithoutImplementations
                = this.Where(r =>
                    !r.ImplementationTypes.Any()
                    && r.ImplementationFactory == null
                    && r.ImplementationInstance == null
                    && (r.AllowScannedImplementationTypes || r.ServiceType.IsAbstract)).ToList();

            var _RegistrationsWithInvalidImplementationTypes
                = this.Where(r => r.ImplementationTypes.Any(t => t.IsAbstract));

            if (_RegistrationsWithoutImplementations.Any())
            {
                _ = _StringBuilder.AppendLine("The following services don't have any implementations:");

                foreach (var _Registration in _RegistrationsWithoutImplementations)
                    _ = _StringBuilder.Append(" - ").AppendLine(_Registration.ServiceType.Name);
            }

            if (_RegistrationsWithInvalidImplementationTypes.Any())
            {
                _ = _StringBuilder.AppendLine("The following services have invalid implementation types:");

                foreach (var _Registration in _RegistrationsWithInvalidImplementationTypes)
                    _ = _StringBuilder
                            .Append(" - ")
                            .Append(_Registration.ServiceType.Name)
                            .AppendLine($" ({string.Join(", ", _Registration.ImplementationTypes.Where(r => r.IsAbstract).Select(r => r.Name))})");
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
