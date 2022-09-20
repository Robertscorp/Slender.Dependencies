using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.ServiceRegistrations
{

    internal static class TypeExtensions
    {

        #region - - - - - - Methods - - - - - -

        internal static Type GetTypeDefinition(this Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        internal static IEnumerable<Type> GetDirectInterfaces(this Type type)
            => type.GetInterfaces().Except(type.GetInterfaces().SelectMany(i => i.GetInterfaces()));

        #endregion Methods

    }

}
