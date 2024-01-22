using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;

namespace Slender.Dependencies.Internals
{

    internal class DependencyAssemblyScanVisitor : AssemblyScanVisitor
    {

        #region - - - - - - Properties - - - - - -

        public Action<Type> OnDependencyFound { get; set; }

        public Action<Type, IEnumerable<Type>> OnDependencyImplementationsFound { get; set; }

        public Action<Type> OnTypeFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        protected override void VisitAbstract(Type abstractType)
            => this.OnDependencyFound.Invoke(abstractType);

        protected override void VisitAbstractAndImplementations(Type abstractType, IEnumerable<Type> implementationTypes)
            => this.OnDependencyImplementationsFound.Invoke(abstractType, implementationTypes);

        protected override void VisitInterface(Type interfaceType)
            => this.OnDependencyFound.Invoke(interfaceType);

        protected override void VisitInterfaceAndImplementations(Type interfaceType, IEnumerable<Type> implementationTypes)
            => this.OnDependencyImplementationsFound.Invoke(interfaceType, implementationTypes);

        protected override void VisitType(Type type)
        {
            this.OnTypeFound.Invoke(type);
            base.VisitType(type);
        }

        #endregion Methods

    }

}
