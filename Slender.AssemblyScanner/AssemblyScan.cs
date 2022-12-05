using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slender.AssemblyScanner
{

    public class AssemblyScan
    {

        #region - - - - - - Constructors - - - - - -

        private AssemblyScan() { }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public Type[] Types { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public static AssemblyScan FromAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies is null
                ? throw new ArgumentNullException(nameof(assemblies))
                : new AssemblyScan { Types = assemblies.Where(a => a != null).SelectMany(a => a.GetTypes()).ToArray() };

        public static AssemblyScan FromAssemblies(Assembly assembly, params Assembly[] additionalAssembliesToScan)
            => assembly is null
                ? throw new ArgumentNullException(nameof(assembly))
                : FromAssemblies(additionalAssembliesToScan.Where(a => a != null).Union(new[] { assembly }));

        public static AssemblyScan FromAssembly(Assembly assembly)
            => FromAssemblies(assembly);

        #endregion Methods

    }

}
