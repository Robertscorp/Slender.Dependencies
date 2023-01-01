using Slender.AssemblyScanner;
using Slender.ServiceRegistrations.Visitors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slender.ServiceRegistrations
{

    public class RegistrationCollection : IEnumerable<Registration>
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, IEnumerable<Type>> m_ImplementationsByType = new Dictionary<Type, IEnumerable<Type>>();
        private readonly Dictionary<Type, Registration> m_RegistrationsByType = new Dictionary<Type, Registration>();

        private AssemblyScan m_AssemblyScan = AssemblyScan.Empty();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationCollection() { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public RegistrationCollection AddAssemblies(Assembly assembly, params Assembly[] additionalAssemblies)
        {
            var _AssemblyScan = AssemblyScan.FromAssemblies(assembly, additionalAssemblies);

            new ImplementationScanVisitor
            {
                OnServiceAndImplementationsFound = (service, implementations) =>
                {
                    if (this.m_RegistrationsByType.TryGetValue(service, out var _Registration))
                        this.AddImplementations(_Registration, implementations);
                    else if (this.m_ImplementationsByType.TryGetValue(service, out var _Implementations))
                        this.m_ImplementationsByType[service] = _Implementations.Union(implementations);
                    else
                        this.m_ImplementationsByType.Add(service, implementations);
                }
            }.VisitAssemblyScan(_AssemblyScan);

            _ = this.m_AssemblyScan.AddAssemblyScan(_AssemblyScan);

            return this;
        }

        public RegistrationCollection AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            _ = this.m_AssemblyScan.AddAssemblies(assemblies);
            return this;
        }

        public RegistrationCollection AddAssembly(Assembly assembly)
            => this.AddAssemblies(assembly);

        private void AddImplementations(Registration registration, IEnumerable<Type> implementations)
        {
            foreach (var _Implementation in implementations)
                _ = registration.AddImplementationType(_Implementation);
        }

        public RegistrationCollection AddScopedService<TService>(Action<Registration> configurationAction = null)
            => this.AddScopedService(typeof(TService), configurationAction);

        public RegistrationCollection AddScopedService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Scoped(), configurationAction);

        private RegistrationCollection AddService(Type type, RegistrationLifetime lifetime, Action<Registration> configurationAction)
        {
            if (this.m_RegistrationsByType.ContainsKey(type))
                throw new Exception($"{type.Name} has already been registered. Use {nameof(ConfigureService)} instead.");

            var _Registration = new Registration(type, lifetime);

            this.m_RegistrationsByType.Add(type, _Registration);

            configurationAction?.Invoke(_Registration);

            if (this.m_ImplementationsByType.TryGetValue(type, out var _Implementations))
            {
                this.AddImplementations(_Registration, _Implementations);
                _ = this.m_ImplementationsByType.Remove(_Registration.ServiceType);
            }

            return this;
        }

        public RegistrationCollection AddSingletonService<TService>(Action<Registration> configurationAction = null)
            => this.AddSingletonService(typeof(TService), configurationAction);

        public RegistrationCollection AddSingletonService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Singleton(), configurationAction);

        public RegistrationCollection AddTransientService<TService>(Action<Registration> configurationAction = null)
            => this.AddTransientService(typeof(TService), configurationAction);

        public RegistrationCollection AddTransientService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Transient(), configurationAction);

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

        public RegistrationCollection ScanForUnregisteredServices(Action<Type> serviceRegistrationAction)
        {
            new ServiceScanVisitor
            {
                OnServiceFound = t => { if (!this.m_RegistrationsByType.ContainsKey(t)) serviceRegistrationAction(t); }
            }.VisitAssemblyScan(this.m_AssemblyScan);

            this.m_AssemblyScan = AssemblyScan.Empty();

            return this;
        }

        #endregion Methods

    }

}
