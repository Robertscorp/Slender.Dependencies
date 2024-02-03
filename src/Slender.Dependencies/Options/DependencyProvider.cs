using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Provides a new instance of the dependency.
    /// </summary>
    /// <param name="dependencyType">The <see cref="Type"/> of dependency.</param>
    /// <returns>A new instance of the dependency.</returns>
    public delegate IDependency DependencyProvider(Type dependencyType);

}
