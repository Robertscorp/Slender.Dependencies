using System;
using System.Collections.Generic;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Provides options to configure dependency collection validation.
    /// </summary>
    public class DependencyCollectionValidationOptions
    {

        #region - - - - - - Properties - - - - - -

        internal bool ShouldIgnoreInvalidImplementations { get; private set; }

        internal bool ShouldIgnoreMissingImplementations { get; private set; }

        internal bool ShouldIgnoreMissingLifetimes { get; private set; }

        internal List<Type> TransitiveDependencies { get; } = new List<Type>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have implementations that are incompatible with the lifetime of the dependency.
        /// </summary>
        /// <returns>Itself.</returns>
        public DependencyCollectionValidationOptions IgnoreInvalidImplementations()
        {
            this.ShouldIgnoreInvalidImplementations = true;
            return this;
        }

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have no implementations.
        /// </summary>
        /// <returns>Itself.</returns>
        public DependencyCollectionValidationOptions IgnoreMissingImplementations()
        {
            this.ShouldIgnoreMissingImplementations = true;
            return this;
        }

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have no lifetime.
        /// </summary>
        /// <returns>Itself.</returns>
        public DependencyCollectionValidationOptions IgnoreMissingLifetimes()
        {
            this.ShouldIgnoreMissingLifetimes = true;
            return this;
        }

        /// <summary>
        /// Informs the validation process that the specified <paramref name="transitiveDependencyType"/> has been resolved with the dependency injection container directly.
        /// </summary>
        /// <returns>Itself.</returns>
        public DependencyCollectionValidationOptions ResolveTransitiveDependency(Type transitiveDependencyType)
            => this.TransitiveDependencies.Remove(transitiveDependencyType)
                ? this
                : throw new InvalidOperationException($"'{transitiveDependencyType.Name}' is not a transitive dependency.");

        #endregion Methods

    }

}
