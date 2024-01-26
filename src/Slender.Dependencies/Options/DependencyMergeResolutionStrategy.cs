namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Represents a strategy to handle when a duplicate dependency is being added to a <see cref="DependencyCollection"/>.
    /// </summary>
    /// <param name="existingDependency">The dependency that exists on the dependency collection.</param>
    /// <param name="incomingDependency">the dependency that is being added to the dependency collection.</param>
    /// <returns>The dependency to be stored against the dependency collection.</returns>
    /// <remarks>For pre-defined strategies, see <see cref="DependencyMergeResolutionStrategies"/>.</remarks>
    public delegate IDependency DependencyMergeResolutionStrategy(IDependency existingDependency, IDependency incomingDependency);

    /// <summary>
    /// Provides pre-defined implementations of <see cref="DependencyMergeResolutionStrategy"/>.
    /// </summary>
    public static class DependencyMergeResolutionStrategies
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Provides a strategy that ignores the duplicate dependency.
        /// </summary>
        public static readonly DependencyMergeResolutionStrategy IgnoreIncoming
            = new DependencyMergeResolutionStrategy((existing, incoming) => existing);

        /// <summary>
        /// Provides a strategy that merges the existing dependency into the duplicate dependency and replaces the existing dependency with the duplicate dependency.
        /// </summary>
        public static readonly DependencyMergeResolutionStrategy MergeExistingIntoIncoming
            = new DependencyMergeResolutionStrategy((existing, incoming) =>
            {
                existing.AddToDependency(incoming);
                return incoming;
            });

        /// <summary>
        /// Provides a strategy that merges the duplicate dependency into the existing dependency.
        /// </summary>
        public static readonly DependencyMergeResolutionStrategy MergeIncomingIntoExisting
            = new DependencyMergeResolutionStrategy((existing, incoming) =>
            {
                incoming.AddToDependency(existing);
                return existing;
            });

        /// <summary>
        /// Provides a strategy that replaces the existing dependency with the duplicate dependency.
        /// </summary>
        public static readonly DependencyMergeResolutionStrategy ReplaceExisting
            = new DependencyMergeResolutionStrategy((existing, incoming) => incoming);

        #endregion Methods

    }

}
