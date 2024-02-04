using System.Linq;

namespace Slender.Dependencies.Internals
{

    internal class ValidationDependency : ReadOnlyDependency
    {

        #region - - - - - - Constructors - - - - - -

        public ValidationDependency(IDependency dependency) : base(dependency) { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public bool HasInvalidImplementations()
            => this.Lifetime != null && this.Implementations.Any(i => !this.Lifetime.SupportsImplementation(i));

        public bool HasNoImplementations()
            => !this.Implementations.Any();

        public bool HasNoLifetime()
            => this.Lifetime == null;

        #endregion Methods

    }

}
