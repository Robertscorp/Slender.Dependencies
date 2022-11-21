using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slender.ServiceRegistrations
{

    public class AssemblyScanner
    {

        #region - - - - - - Methods - - - - - -

        private static List<Type> GetOrAdd(Dictionary<Type, List<Type>> dictionary, Type type)
        {
            if (!dictionary.TryGetValue(type, out var _List))
            {
                _List = new List<Type>();
                dictionary.Add(type, _List);
            }
            return _List;
        }

        private static IEnumerable<Type> GetAbstractBases(Type type)
            => type == null
                ? Enumerable.Empty<Type>()
                : type.IsAbstract
                    ? GetAbstractBases(type.BaseType).Union(new[] { type })
                    : GetAbstractBases(type.BaseType);

        public static void Scan(
            IAssemblyScannerStrategy strategy,
            Assembly assembly,
            params Assembly[] additionalAssembliesToScan)
        {
            if (strategy is null) throw new ArgumentNullException(nameof(strategy));
            if (assembly is null) throw new ArgumentNullException(nameof(assembly));

            var _AbstractImplementations = new Dictionary<Type, List<Type>>();
            var _InterfaceImplementations = new Dictionary<Type, List<Type>>();

            foreach (var _Type in new[] { assembly }.Union(additionalAssembliesToScan).SelectMany(a => a.GetTypes()))
            {
                if (_Type.IsEnum)
                    strategy.EnumerationFound(_Type);

                else if (_Type.IsInterface) // Needs to be before IsAbstract, as an Interface is Abstract.
                    strategy.InterfaceFound(_Type);

                else if (_Type.IsAbstract)
                {
                    // A Static class is an Abstract Sealed Class.
                    if (!_Type.IsSealed) strategy.AbstractFound(_Type);
                }
                else if (_Type.IsValueType)
                    strategy.ValueTypeFound(_Type);

                else if (_Type.BaseType == typeof(MulticastDelegate)) // Needs to be before IsClass, as a delegate is a Class.
                    strategy.DelegateFound(_Type);

                else if (_Type.IsClass)
                {
                    strategy.ImplementationFound(_Type);

                    foreach (var _AbstractBase in GetAbstractBases(_Type))
                        GetOrAdd(_AbstractImplementations, _AbstractBase).Add(_Type);

                    foreach (var _Interface in _Type.GetDirectInterfaces())
                        GetOrAdd(_InterfaceImplementations, _Interface).Add(_Type);
                }
            }

            foreach (var _AbstractAndImplementations in _AbstractImplementations)
                strategy.AbstractImplementationsFound(_AbstractAndImplementations.Key, _AbstractAndImplementations.Value);

            foreach (var _IntercaceAndImplementations in _InterfaceImplementations)
                strategy.InterfaceImplementationsFound(_IntercaceAndImplementations.Key, _IntercaceAndImplementations.Value);
        }

        #endregion Methods

    }

    public interface IAssemblyScannerStrategy
    {

        #region - - - - - - Methods - - - - - -

        void AbstractFound(Type abstractType);

        void AbstractImplementationsFound(Type abstractType, IEnumerable<Type> implementationTypes);

        void DelegateFound(Type delegateType);

        void EnumerationFound(Type enumerationType);

        void ImplementationFound(Type implementationType);

        void InterfaceFound(Type interfaceType);

        void InterfaceImplementationsFound(Type interfaceType, IEnumerable<Type> implementationTypes);

        void ValueTypeFound(Type valueType);

        #endregion Methods 

    }

}
