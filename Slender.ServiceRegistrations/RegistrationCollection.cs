using Slender.ServiceRegistrations.Visitors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slender.ServiceRegistrations
{

    public class RegistrationCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<Assembly> m_Assemblies = new List<Assembly>();
        private readonly ConcurrentDictionary<Type, Registration> m_RegistrationsByType = new ConcurrentDictionary<Type, Registration>();

        private ServiceScanVisitor m_ServiceScanVisitor;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public RegistrationCollection() { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public RegistrationCollection AddAssemblies(Assembly assembly, params Assembly[] additonalAssemblies)
            => assembly is null
                ? throw new ArgumentNullException(nameof(assembly))
                : this.AddAssemblies(new[] { assembly }.Union(additonalAssemblies));

        public RegistrationCollection AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            this.m_Assemblies.AddRange(assemblies.Where(a => a != null));

            return this;
        }

        public RegistrationCollection AddAssembly(Assembly assembly)
            => this.AddAssemblies(assembly);

        public RegistrationCollection AddScopedService<TService>(Action<Registration> configurationAction = null)
            => this.AddScopedService(typeof(TService), configurationAction);

        public RegistrationCollection AddScopedService(Type type, Action<Registration> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Scoped(), configurationAction);

        private RegistrationCollection AddService(Type type, RegistrationLifetime lifetime, Action<Registration> configurationAction)
        {
            var _Registration
                = this.m_RegistrationsByType.ContainsKey(type)
                    ? throw new Exception($"{type.Name} has already been registered. Use {nameof(ConfigureService)} instead.")
                    : this.m_RegistrationsByType.GetOrAdd(type, new Registration(type, lifetime));

            configurationAction?.Invoke(_Registration);

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

        public RegistrationCollection ScanForUnregisteredServices(Action<Type> serviceRegistrationAction)
        {
            this.m_ServiceScanVisitor = new ServiceScanVisitor
            {
                OnServiceFound = t => { if (!this.m_RegistrationsByType.ContainsKey(t)) serviceRegistrationAction(t); }
            };

            return this;
        }

        #endregion Methods

    }

}
