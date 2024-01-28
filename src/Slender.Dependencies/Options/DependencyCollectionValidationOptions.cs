using System;
using System.Collections.Generic;

namespace Slender.Dependencies.Options
{

    internal class DependencyCollectionValidationOptions : IDependencyCollectionValidationOptions
    {

        #region - - - - - - Properties - - - - - -

        internal bool IgnoreInvalidImplementations { get; private set; }

        internal bool IgnoreMissingImplementations { get; private set; }

        internal bool IgnoreMissingLifetimes { get; private set; }

        internal List<Type> TransitiveDependencies { get; } = new List<Type>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreInvalidImplementations()
        {
            this.IgnoreInvalidImplementations = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreMissingImplementations()
        {
            this.IgnoreMissingImplementations = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreMissingLifetimes()
        {
            this.IgnoreMissingLifetimes = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.ResolveTransitiveDependency(Type transitiveDependencyType)
            => this.TransitiveDependencies.Remove(transitiveDependencyType)
                ? this
                : throw new InvalidOperationException($"'{transitiveDependencyType.Name}' is not a transitive dependency.");

        #endregion Methods

    }

}
