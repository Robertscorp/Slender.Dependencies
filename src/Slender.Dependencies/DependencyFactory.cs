using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// A factory used to produce an instance of the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type to produce.</param>
    /// <returns>An instance of the specified type, or <see langword="null"/>.</returns>
    public delegate object DependencyFactory(Type type);

    /// <summary>
    /// Contains <see cref="DependencyFactory"/> extension methods.
    /// </summary>
    public static class DependencyFactoryExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Produces an instance of <typeparamref name="TDependency"/>.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency to produce.</typeparam>
        /// <param name="dependencyFactory">The factory used to produce the instance.</param>
        /// <returns>An instance of <typeparamref name="TDependency"/>, or <see langword="null"/>.</returns>
        public static TDependency GetInstance<TDependency>(this DependencyFactory dependencyFactory)
            => (TDependency)dependencyFactory(typeof(TDependency));

        #endregion Methods

    }

}
