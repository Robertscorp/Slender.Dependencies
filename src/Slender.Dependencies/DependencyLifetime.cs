using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// An enumeration of possible dependency lifetimes.
    /// </summary>
    public class DependencyLifetime
    {

        #region - - - - - - Fields - - - - - -

        private readonly Func<object, bool> m_SupportsImplementationFunction;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        private DependencyLifetime(string name, Func<object, bool> supportsImplementationFunction)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.m_SupportsImplementationFunction = supportsImplementationFunction ?? throw new ArgumentNullException(nameof(supportsImplementationFunction));
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

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
        public static DependencyLifetime Scoped() => new DependencyLifetime("Scoped", implementation => implementation is Type || implementation is Func<DependencyFactory, object>);

        /// <summary>
        /// The singleton lifetime. Dependencies with this lifetime will use the same instance each time the dependency is resolved.
        /// </summary>
        /// <returns>The singleton lifetime.</returns>
        public static DependencyLifetime Singleton() => new DependencyLifetime("Singleton", implementation => implementation != null);

        /// <summary>
        /// The transient lifetime. Dependencies with this lifetime will use a new instance each time the dependency is resolved.
        /// </summary>
        /// <returns>The transient lifetime.</returns>
        public static DependencyLifetime Transient() => new DependencyLifetime("Transient", implementation => implementation is Type || implementation is Func<DependencyFactory, object>);

        /// <summary>
        /// Determines if the specified <paramref name="implementation"/> is supported by this lifetime.
        /// </summary>
        /// <param name="implementation">An implementation of a dependency.</param>
        /// <returns><see langword="true"/> if the implementation is supported; otherwise, <see langword="false"/>.</returns>
        public bool SupportsImplementation(object implementation)
            => this.m_SupportsImplementationFunction.Invoke(implementation);

        #endregion Methods

    }

}
