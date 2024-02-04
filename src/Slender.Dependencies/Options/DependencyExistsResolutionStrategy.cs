using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Represents a strategy to handle when a duplicate dependency is being added by type to a <see cref="DependencyCollection"/>.
    /// </summary>
    /// <param name="dependencyProvider">A delegate that can provide a new dependency.</param>
    /// <param name="existingDependency">The existing dependency in the dependency collection.</param>
    /// <param name="incomingDependencyType">The dependency type that is being added to the dependency collection.</param>
    /// <returns>The dependency to be stored against the dependency collection.</returns>
    /// <remarks>For pre-defined strategies, see <see cref="DependencyExistsResolutionStrategies"/>.</remarks>
    public delegate IDependency DependencyExistsResolutionStrategy(
        DependencyProvider dependencyProvider,
        IDependency existingDependency,
        Type incomingDependencyType);

    /// <summary>
    /// Provides pre-defined implementations of <see cref="DependencyExistsResolutionStrategy"/>.
    /// </summary>
    public static class DependencyExistsResolutionStrategies
    {

        #region - - - - - - Fields - - - - - -

        /// <summary>
        /// Provides a <see cref="DependencyExistsResolutionStrategy"/> that keeps the existing dependency from the <see cref="DependencyCollection"/>.
        /// </summary>
        public static readonly DependencyExistsResolutionStrategy KeepExisting
            = (dependencyProvider, existing, incomingType) => existing;

        /// <summary>
        /// Provides a <see cref="DependencyExistsResolutionStrategy"/> that replaces an existing dependency with a new dependency using the specified dependency provider.
        /// </summary>
        public static readonly DependencyExistsResolutionStrategy ReplaceExisting
            = (dependencyProvider, existing, incomingType) => dependencyProvider.Invoke(incomingType);

        #endregion Methods

    }

}
