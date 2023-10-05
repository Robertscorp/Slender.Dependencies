namespace Slender.Dependencies
{

    /// <summary>
    /// An enumeration of possible dependency lifetimes.
    /// </summary>
    public class DependencyLifetime
    {

        #region - - - - - - Constructors - - - - - -

        private DependencyLifetime(string name, bool allowImplementationInstances)
        {
            this.Name = name;
            this.AllowImplementationInstances = allowImplementationInstances;
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Used to determine if dependencies with this lifetime can have implementation instances.
        /// </summary>
        public bool AllowImplementationInstances { get; }

        /// <summary>
        /// The name of the lifetime.
        /// </summary>
        public string Name { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// The scoped lifetime. Dependencies with this lifetime will use the same instance each time the dependency is resolved within its scope.
        /// </summary>
        /// <returns>The scoped lifetime.</returns>
        public static DependencyLifetime Scoped() => new DependencyLifetime("Scoped", false);

        /// <summary>
        /// The singleton lifetime. Dependencies with this lifetime will use the same instance each time the dependency is resolved.
        /// </summary>
        /// <returns>The singleton lifetime.</returns>
        public static DependencyLifetime Singleton() => new DependencyLifetime("Singleton", true);

        /// <summary>
        /// The transient lifetime. Dependencies with this lifetime will use a new instance each time the dependency is resolved.
        /// </summary>
        /// <returns>The transient lifetime.</returns>
        public static DependencyLifetime Transient() => new DependencyLifetime("Transient", false);

        #endregion Methods

    }

}
