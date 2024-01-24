using System;
using System.Reflection;

namespace Slender.Dependencies.Tests.Support
{

    internal static class TestDependencyLifetime
    {

        #region - - - - - - Methods - - - - - -

        public static DependencyLifetime Instance(bool allowImplementationInstances)
            => (DependencyLifetime)Activator.CreateInstance(
                typeof(DependencyLifetime),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { "", (object implementation) => allowImplementationInstances || implementation is Type || implementation is Func<DependencyFactory, object> },
                culture: null)!;

        #endregion Methods

    }

}
