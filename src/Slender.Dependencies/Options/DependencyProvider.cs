using System;

namespace Slender.Dependencies.Options
{

    /// <summary>
    /// Provides a new dependency instance.
    /// </summary>
    /// <param name="dependencyType">The type of dependency.</param>
    /// <returns>A new dependency instance.</returns>
    public delegate IDependency DependencyProvider(Type dependencyType);

}
