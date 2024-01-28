using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Provides configuration options for a <see cref="DependencyCollection"/>.
    /// </summary>
    public class DependencyCollectionOptions
    {

        #region - - - - - - Fields - - - - - -

        private bool m_AutoLockLifetimes = true;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="DependencyCollectionOptions"/> class.
        /// </summary>
        public DependencyCollectionOptions()
            => this.DependencyProvider = type => new Dependency(type, this.m_AutoLockLifetimes);

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal DependencyProvider DependencyProvider { get; private set; }

        internal DependencyExistsResolutionStrategy DependencyExistsResolutionStrategy { get; set; } = DependencyExistsResolutionStrategies.KeepExisting;

        internal DependencyMergeResolutionStrategy MergeResolutionStrategy { get; set; } = DependencyMergeResolutionStrategies.MergeExistingIntoIncoming;

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Sets a value indicating if new Dependencies should have their lifetimes locked once they have been set.
        /// </summary>
        /// <param name="autoLockLifetimes"><see langword="true"/> if lifetimes should lock once they have been set; otherwise, <see langword="false"/>.</param>
        /// <returns>Itself.</returns>
        public DependencyCollectionOptions HasAutoLockForLifetimes(bool autoLockLifetimes)
        {
            this.m_AutoLockLifetimes = autoLockLifetimes;
            return this;
        }

        /// <summary>
        /// Sets the dependency provider that will be used by the dependency collection to create new dependencies.
        /// </summary>
        /// <param name="dependencyProvider">The dependency provider that will be used by the dependency collection to create new dependencies. Cannot be <see langword="null"/>.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencyProvider"/> is <see langword="null"/>.</exception>
        public DependencyCollectionOptions HasDependencyProvider(DependencyProvider dependencyProvider)
        {
            this.DependencyProvider = dependencyProvider ?? throw new ArgumentNullException(nameof(dependencyProvider));
            return this;
        }

        /// <summary>
        /// Sets the strategy that will be used by the dependency collection when a duplicate dependency is being added by type.
        /// </summary>
        /// <param name="dependencyExistsResolutionStrategy">The strategy that will be used by the dependency collection when a duplicate dependency is being added by type. Cannot be <see langword="null"/>.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencyExistsResolutionStrategy"/> is <see langword="null"/>.</exception>
        public DependencyCollectionOptions HasResolutionStrategyForExistingDependency(DependencyExistsResolutionStrategy dependencyExistsResolutionStrategy)
        {
            this.DependencyExistsResolutionStrategy = dependencyExistsResolutionStrategy ?? throw new ArgumentNullException(nameof(dependencyExistsResolutionStrategy));
            return this;
        }

        /// <summary>
        /// Sets the strategy that will be used by the dependency collection when a duplicate dependency is being added.
        /// </summary>
        /// <param name="mergeResolutionStrategy">The strategy that will be used by the dependency collection when a duplicate dependency is being added. Cannot be <see langword="null"/>.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mergeResolutionStrategy"/> is <see langword="null"/>.</exception>
        public DependencyCollectionOptions HasResolutionStrategyForMergingDependencies(DependencyMergeResolutionStrategy mergeResolutionStrategy)
        {
            this.MergeResolutionStrategy = mergeResolutionStrategy ?? throw new ArgumentNullException(nameof(mergeResolutionStrategy));
            return this;
        }

        #endregion Methods

    }

}
