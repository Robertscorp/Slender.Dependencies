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

    public class RegistrationCollection : IEnumerable<Registration>
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, IEnumerable<Type>> m_ImplementationsByType = new Dictionary<Type, IEnumerable<Type>>();
        private readonly Dictionary<Type, Registration> m_RegistrationsByType = new Dictionary<Type, Registration>();

        private IAssemblyScan m_AssemblyScan = AssemblyScan.Empty();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

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
        /// Adds the assembly scan and scans it for implementations of registered services.
        /// </summary>
        /// <param name="assemblyScan">The assembly scan to add.</param>
        /// <returns>Itself.</returns>
        public RegistrationCollection AddAssemblyScan(IAssemblyScan assemblyScan)
        {
            new ImplementationScanVisitor { OnServiceAndImplementationsFound = this.AddServiceAndImplementations }.VisitAssemblyScan(assemblyScan);

            this.m_AssemblyScan = AssemblyScan.Empty().AddAssemblyScan(this.m_AssemblyScan).AddAssemblyScan(assemblyScan);

            return this;
        }

        private void AddImplementations(Registration registration, IEnumerable<Type> implementations)
        {
            foreach (var _Implementation in implementations)
                _ = registration.AddImplementationType(_Implementation);
        }

        private void AddServiceAndImplementations(Type service, IEnumerable<Type> implementations)
        {
            if (this.m_RegistrationsByType.TryGetValue(service, out var _Registration))
                this.AddImplementations(_Registration, implementations);
            else if (this.m_ImplementationsByType.TryGetValue(service, out var _Implementations))
                this.m_ImplementationsByType[service] = _Implementations.Union(implementations);
            else
                this.m_ImplementationsByType.Add(service, implementations);
        }

        private void AddRegistration(Registration registration)
        {
            this.m_RegistrationsByType.Add(registration.ServiceType, registration);

            if (this.m_ImplementationsByType.TryGetValue(registration.ServiceType, out var _Implementations))
            {
                this.AddImplementations(registration, _Implementations);
                _ = this.m_ImplementationsByType.Remove(registration.ServiceType);
            }
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
                this.AddServiceAndImplementations(_ImplementationsByType.Key, _ImplementationsByType.Value);

            return this.AddAssemblyScan(registrations.m_AssemblyScan);
        }

        /// <summary>
        /// Registers TService as a scoped service.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddScopedService<TService>(Action<Registration> configurationAction = null)
            => this.AddScopedService(typeof(TService), configurationAction);

        /// <summary>
        /// Registers the specified type as a scoped service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddScopedService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Scoped(), configurationAction);

        private RegistrationCollection AddService(Type type, RegistrationLifetime lifetime, Action<Registration> configurationAction)
        {
            if (this.m_RegistrationsByType.ContainsKey(type))
                throw new Exception($"{type.Name} has already been registered. Use {nameof(ConfigureService)} instead.");

            var _Registration = new Registration(type, lifetime);

            configurationAction?.Invoke(_Registration);

            this.AddRegistration(_Registration);

            return this;
        }

        /// <summary>
        /// Registers TService as a singleton service.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddSingletonService<TService>(Action<Registration> configurationAction = null)
            => this.AddSingletonService(typeof(TService), configurationAction);

        /// <summary>
        /// Registers the specified type as a singleton service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddSingletonService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Singleton(), configurationAction);

        /// <summary>
        /// Registers TService as a transient service.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddTransientService<TService>(Action<Registration> configurationAction = null)
            => this.AddTransientService(typeof(TService), configurationAction);

        /// <summary>
        /// Registers the specified type as a transient service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>After the action is invoked, any matching scanned implementations will be added to the registered service.</remarks>
        public RegistrationCollection AddTransientService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Transient(), configurationAction);

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
            var _InvalidRegistrations = this.Where(r => !r.ImplementationTypes.Any() && r.ImplementationFactory == null && r.ImplementationInstance == null).ToArray();
            if (_InvalidRegistrations.Any())
            {
                var _StringBuilder = new StringBuilder(64);
                _ = _StringBuilder.AppendLine("The following services don't have any implementations:");

                foreach (var _Registration in _InvalidRegistrations)
                    _ = _StringBuilder.Append(" - ").AppendLine(_Registration.ServiceType.Name);

                throw new Exception(_StringBuilder.ToString());
            }

            return this;
        }

        #endregion Methods

    }

}
