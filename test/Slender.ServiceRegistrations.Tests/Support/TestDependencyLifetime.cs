using System;
using System.Reflection;

namespace Slender.ServiceRegistrations.Tests.Support
{

    internal static class TestDependencyLifetime
    {

        #region - - - - - - Methods - - - - - -

        public static DependencyLifetime Instance(bool allowImplementationInstances)
            => (DependencyLifetime)Activator.CreateInstance(
                typeof(DependencyLifetime),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { "", allowImplementationInstances },
                culture: null)!;

        #endregion Methods

    }

}
