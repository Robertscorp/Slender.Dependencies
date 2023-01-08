using Slender.AssemblyScanner;
using System;

namespace Slender.ServiceRegistrations.Visitors
{

    internal class ServiceScanVisitor : AssemblyScanVisitor
    {

        #region - - - - - - Properties - - - - - -

        public Action<Type> OnServiceFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        protected override void VisitAbstract(Type abstractType)
            => this.OnServiceFound?.Invoke(abstractType);

        public override void VisitAssemblyScan(IAssemblyScan scan)
        {
            foreach (var _Type in scan.Types)
                this.VisitType(_Type);
        }

        protected override void VisitInterface(Type interfaceType)
            => this.OnServiceFound?.Invoke(interfaceType);

        #endregion Methods

    }

}
