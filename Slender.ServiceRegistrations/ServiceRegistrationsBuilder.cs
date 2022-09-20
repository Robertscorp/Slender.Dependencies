using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slender.ServiceRegistrations
{

    public class ServiceRegistrationsBuilder
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<Assembly> m_AssembliesToScan = new List<Assembly>();
        private readonly List<(Type, Type)> m_Decorators = new List<(Type, Type)>();
        private readonly HashSet<Type> m_ScopedImplementationsToFind = new HashSet<Type>();
        private readonly HashSet<Type> m_SingletonImplementationsToFind = new HashSet<Type>();
        private readonly HashSet<Type> m_TransientImplementationsToFind = new HashSet<Type>();

        #endregion Fields

        #region - - - - - - Methods - - - - - -

        public ServiceRegistrationsBuilder AddAssembly(Assembly assembly)
            => this.AddAssemblies(assembly);

        public ServiceRegistrationsBuilder AddAssemblies(Assembly assembly, params Assembly[] additonalAssemblies)
        {
            this.m_AssembliesToScan.Add(assembly);
            this.m_AssembliesToScan.AddRange(additonalAssemblies);

            return this;
        }

        public ServiceRegistrationsBuilder AddDecorator<TService, TDecorator>()
            => this.AddDecorator(typeof(TService), typeof(TDecorator));

        public ServiceRegistrationsBuilder AddDecorator(Type serviceType, Type decoratorType)
        {
            this.m_Decorators.Add((serviceType, decoratorType));
            return this;
        }

        public ServiceRegistrationsBuilder AddScopedService<TService>()
            => this.AddScopedService(typeof(TService));

        public ServiceRegistrationsBuilder AddScopedService(Type serviceType)
        {
            _ = this.m_ScopedImplementationsToFind.Add(serviceType);
            return this;
        }

        public ServiceRegistrationsBuilder AddSingletonService<TService>()
            => this.AddSingletonService(typeof(TService));

        public ServiceRegistrationsBuilder AddSingletonService(Type serviceType)
        {
            _ = this.m_SingletonImplementationsToFind.Add(serviceType);
            return this;
        }

        public ServiceRegistrationsBuilder AddTransientService<TService>()
            => this.AddTransientService(typeof(TService));

        public ServiceRegistrationsBuilder AddTransientService(Type serviceType)
        {
            _ = this.m_TransientImplementationsToFind.Add(serviceType);
            return this;
        }

        public ServiceRegistrations ToServiceRegistrations()
        {
            var _ServiceRegistrations = new ServiceRegistrations();
            _ServiceRegistrations.Decorators.AddRange(this.m_Decorators);

            foreach (var _Type in this.m_AssembliesToScan.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract))
                foreach (var _InterfaceType in _Type.GetDirectInterfaces())
                {
                    this.TryRegisterImplementation(_InterfaceType, _Type, this.m_ScopedImplementationsToFind, _ServiceRegistrations.ScopedServicesToRegister);
                    this.TryRegisterImplementation(_InterfaceType, _Type, this.m_SingletonImplementationsToFind, _ServiceRegistrations.SingletonServicesToRegister);
                    this.TryRegisterImplementation(_InterfaceType, _Type, this.m_TransientImplementationsToFind, _ServiceRegistrations.TransientServicesToRegister);
                }

            return _ServiceRegistrations;
        }

        private void TryRegisterImplementation(
            Type serviceType,
            Type implementationType,
            ICollection<Type> serviceTypes,
            List<(Type, Type)> registrations)
        {
            if (serviceTypes.Contains(serviceType.GetTypeDefinition()))
                if (!implementationType.GetGenericArguments().Any())
                    registrations.Add((serviceType, implementationType));

                else if (implementationType.GetGenericArguments().Count() == serviceType.GenericTypeArguments.Count())
                    registrations.Add((serviceType.GetTypeDefinition(), implementationType.GetTypeDefinition()));
        }

        #endregion Methods

    }

}
