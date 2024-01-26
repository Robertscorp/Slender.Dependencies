using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Represents a strategy to handle when a duplicate dependency is being added to a <see cref="DependencyCollection"/>.
    /// </summary>
    public class DependencyMergeStrategy
    {

        #region - - - - - - Fields - - - - - -

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        private DependencyMergeStrategy(Func<IDependency, IDependency, IDependency> mergeFunction)
            => this.MergeFunction = mergeFunction ?? throw new ArgumentNullException(nameof(mergeFunction));

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Func<IDependency, IDependency, IDependency> MergeFunction { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Provides a custom strategy for merging dependencies in a <see cref="DependencyCollection"/>.
        /// </summary>
        /// <param name="customStrategy">
        /// The behaviour for merging the existing and incoming dependencies into a <see cref="DependencyCollection"/>.<br/>
        /// <br/>
        /// The first parameter of the function is the dependency that exists on the dependency collection.<br/>
        /// The second parameter of the function is the dependency that is being added to the dependency collection.<br/>
        /// The return of the function is the dependency to be stored against the dependency collection.
        /// </param>
        /// <returns>A <see cref="DependencyMergeStrategy"/> containing the custom behaviour.</returns>
        public static DependencyMergeStrategy Custom(Func<IDependency, IDependency, IDependency> customStrategy)
            => new DependencyMergeStrategy(customStrategy);

        /// <summary>
        /// Provides a strategy that ignores any duplicate dependencies being added to a <see cref="DependencyCollection"/>.
        /// </summary>
        /// <returns>A <see cref="DependencyMergeStrategy"/> containing the described behaviour.</returns>
        public static DependencyMergeStrategy IgnoreIncoming()
            => new DependencyMergeStrategy((existing, incoming) => existing);

        /// <summary>
        /// Provides a strategy that merges an existing dependency into a duplicate dependency, and replaces the existing dependency with the duplicate dependency, when the duplicate is being added to a <see cref="DependencyCollection"/>.
        /// </summary>
        /// <returns>A <see cref="DependencyMergeStrategy"/> containing the described behaviour.</returns>
        public static DependencyMergeStrategy MergeExistingIntoIncoming()
            => new DependencyMergeStrategy((existing, incoming) =>
            {
                existing.AddToDependency(incoming);
                return incoming;
            });

        /// <summary>
        /// Provides a strategy that merges a duplicate dependency into an existing dependency when the duplicate is being added to a <see cref="DependencyCollection"/>.
        /// </summary>
        /// <returns>A <see cref="DependencyMergeStrategy"/> containing the described behaviour.</returns>
        public static DependencyMergeStrategy MergeIncomingIntoExisting()
            => new DependencyMergeStrategy((existing, incoming) =>
            {
                incoming.AddToDependency(existing);
                return existing;
            });

        /// <summary>
        /// Provides a strategy that replaces an existing dependency with a duplicate dependency when the duplicate is being added to a <see cref="DependencyCollection"/>.
        /// </summary>
        /// <returns>A <see cref="DependencyMergeStrategy"/> containing the described behaviour.</returns>
        public static DependencyMergeStrategy ReplaceExisting()
            => new DependencyMergeStrategy((existing, incoming) => incoming);

        #endregion Methods

    }

}
