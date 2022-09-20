using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    public class ServiceRegistrations
    {

        #region - - - - - - Properties - - - - - -

        public List<(Type ServiceType, Type DecoratorType)> Decorators { get; } = new List<(Type, Type)>();

        public List<(Type ServiceType, Type ImplementationType)> ScopedServicesToRegister { get; } = new List<(Type, Type)>();

        public List<(Type ServiceType, object Implementation)> SingletonImplementationsToRegister { get; } = new List<(Type, object)>();

        public List<(Type ServiceType, Type ImplementationType)> SingletonServicesToRegister { get; } = new List<(Type, Type)>();

        public List<(Type ServiceType, Type ImplementationType)> TransientServicesToRegister { get; } = new List<(Type, Type)>();

        #endregion Properties

    }

}
