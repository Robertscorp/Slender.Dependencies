using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations.Visitors
{

    internal class ImplementationScanVisitor : AssemblyScanVisitor
    {

        #region - - - - - - Properties - - - - - -

        public Action<Type, IEnumerable<Type>> OnServiceAndImplementationsFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        protected override void VisitAbstractAndImplementations(Type abstractType, IEnumerable<Type> implementationTypes)
            => this.OnServiceAndImplementationsFound?.Invoke(abstractType, implementationTypes);

        protected override void VisitInterfaceAndImplementations(Type interfaceType, IEnumerable<Type> implementationTypes)
            => this.OnServiceAndImplementationsFound?.Invoke(interfaceType, implementationTypes);

        #endregion Methods

    }

}
