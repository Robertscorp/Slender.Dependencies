using System;

namespace Slender.Dependencies.Internals
{

    internal static class TypeExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static Type GetTypeDefinition(this Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        #endregion Methods

    }

}
