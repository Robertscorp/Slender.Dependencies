using System;
using System.Reflection;

namespace Slender.ServiceRegistrations.Tests.Support
{

    internal static class TestRegistrationLifetime
    {

        #region - - - - - - Methods - - - - - -

        public static RegistrationLifetime Instance(bool allowImplementationInstances)
            => (RegistrationLifetime)Activator.CreateInstance(
                typeof(RegistrationLifetime),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { "", allowImplementationInstances },
                culture: null)!;

        #endregion Methods

    }

}
