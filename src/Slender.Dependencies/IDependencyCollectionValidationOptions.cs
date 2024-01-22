using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// Provides options to configure dependency collection validation.
    /// </summary>
    public interface IDependencyCollectionValidationOptions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have implementations that are incompatible with the lifetime of the dependency.
        /// </summary>
        /// <returns>Itself.</returns>
        IDependencyCollectionValidationOptions IgnoreInvalidImplementations();

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have no implementations.
        /// </summary>
        /// <returns>Itself.</returns>
        IDependencyCollectionValidationOptions IgnoreMissingImplementations();

        /// <summary>
        /// Causes validation to no longer fail when any dependencies have no lifetime.
        /// </summary>
        /// <returns>Itself.</returns>
        IDependencyCollectionValidationOptions IgnoreMissingLifetimes();

        /// <summary>
        /// Informs the validation process that the specified <paramref name="transitiveDependencyType"/> has been resolved with the dependency injection container directly.
        /// </summary>
        /// <returns>Itself.</returns>
        IDependencyCollectionValidationOptions ResolveTransitiveDependency(Type transitiveDependencyType);

        #endregion Methods

    }

}
