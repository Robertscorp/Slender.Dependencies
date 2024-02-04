using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Provides options when scanning assemblies for dependency and implementation types.
    /// </summary>
    public class DependencyScanningOptions
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Gets an action that will be invoked for every discovered abstract class or interface that has not been registered as a dependency in the dependency collection.
        /// </summary>
        public Action<IDependencyCollection, Type> OnUnregisteredDependencyTypeFound { get; set; }

        /// <summary>
        /// Gets an action that will be invoked for every discovered class that implements or inherits from a dependency, that has not been registered as an implementation of the dependency.
        /// </summary>
        public Action<IDependency, Type> OnUnregisteredImplementationTypeFound { get; set; }

        /// <summary>
        /// Gets an action that will be invoked for every discovered type that has not been registered as a dependency in the dependency collection.
        /// </summary>
        public Action<IDependencyCollection, Type> OnUnregisteredTypeFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        internal void Validate()
        {
            if (this.OnUnregisteredDependencyTypeFound != null) return;
            if (this.OnUnregisteredImplementationTypeFound != null) return;
            if (this.OnUnregisteredTypeFound != null) return;

            throw new Exception("DependencyScanningOptions are not valid because they are empty. At least one action must be provided.");
        }

        #endregion Methods

    }

}
