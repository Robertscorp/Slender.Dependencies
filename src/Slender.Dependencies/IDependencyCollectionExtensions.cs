using Slender.AssemblyScanner;
using Slender.Dependencies.Internals;
using Slender.Dependencies.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Slender.Dependencies
{

    /// <summary>
    /// Contains <see cref="IDependencyCollection"/> extension methods.
    /// </summary>
    public static partial class IDependencyCollectionExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Scans the specified assemblies for possible dependencies and implementation types.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="options">The options to determine scanning behaviour. Cannot be null.</param>
        /// <param name="assemblies">The assemblies to add. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/>, <paramref name="options"/>, or <paramref name="assemblies"/> is null.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        public static TDependencyCollection AddAssemblies<TDependencyCollection>(
            this TDependencyCollection dependencies,
            DependencyScanningOptions options,
            IEnumerable<Assembly> assemblies) where TDependencyCollection : IDependencyCollection
            => AddAssemblyScan(
                dependencies,
                options,
                AssemblyScan.FromAssemblies(assemblies ?? throw new ArgumentNullException(nameof(assemblies))));

        /// <summary>
        /// Scans the specified assemblies for possible dependencies and implementation types.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="options">The options to determine scanning behaviour. Cannot be null.</param>
        /// <param name="assembly">An assembly to add. Cannot be null.</param>
        /// <param name="additionalAssemblies">Additional assemblies to add. Cannot be null</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/>, <paramref name="options"/>, <paramref name="assembly"/>, or <paramref name="additionalAssemblies"/> is null.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        public static TDependencyCollection AddAssemblies<TDependencyCollection>(
            this TDependencyCollection dependencies,
            DependencyScanningOptions options,
            Assembly assembly,
            params Assembly[] additionalAssemblies) where TDependencyCollection : IDependencyCollection
            => AddAssemblyScan(
                dependencies,
                options,
                AssemblyScan.FromAssemblies(
                    assembly ?? throw new ArgumentNullException(nameof(assembly)),
                    additionalAssemblies ?? throw new ArgumentNullException(nameof(additionalAssemblies))));

        /// <summary>
        /// Scans the specified assemblies for possible dependencies and implementation types.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="options">The options to determine scanning behaviour. Cannot be null.</param>
        /// <param name="assembly">The assembly to add. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/>, <paramref name="options"/>, or <paramref name="assembly"/> is null.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        public static TDependencyCollection AddAssembly<TDependencyCollection>(
            this TDependencyCollection dependencies,
            DependencyScanningOptions options,
            Assembly assembly) where TDependencyCollection : IDependencyCollection
            => AddAssemblies(
                dependencies,
                options,
                assembly ?? throw new ArgumentNullException(nameof(assembly)));

        /// <summary>
        /// Visits the specified <see cref="IAssemblyScan"/> for possible dependencies and implementation types.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="options">The options to determine scanning behaviour. Cannot be null.</param>
        /// <param name="assemblyScan">The assembly scan to add. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/>, <paramref name="options"/>, or <paramref name="assemblyScan"/> is null.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        public static TDependencyCollection AddAssemblyScan<TDependencyCollection>(
            this TDependencyCollection dependencies,
            DependencyScanningOptions options,
            IAssemblyScan assemblyScan) where TDependencyCollection : IDependencyCollection
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (assemblyScan == null) throw new ArgumentNullException(nameof(assemblyScan));

            var _Dependencies = new DependencyCollectionScanningDecorator(dependencies);
            var _Visitor = new DependencyAssemblyScanVisitor();

            if (options.OnUnregisteredDependencyTypeFound != null)
                _Visitor.OnDependencyFound = dependencyType =>
                {
                    if (_Dependencies.DependenciesByType.ContainsKey(dependencyType))
                        return;

                    if (_Dependencies.RegisteredTransitiveDependencies.Contains(dependencyType.GetTypeDefinition()))
                        return;

                    options.OnUnregisteredDependencyTypeFound(_Dependencies, dependencyType);
                };

            if (options.OnUnregisteredImplementationTypeFound != null)
                _Visitor.OnDependencyImplementationsFound = (dependencyType, implementations) =>
                {
                    if (_Dependencies.RegisteredTransitiveDependencies.Contains(dependencyType.GetTypeDefinition()))
                        return;

                    if (!_Dependencies.DependenciesByType.ContainsKey(dependencyType))
                        options.OnUnregisteredDependencyTypeFound(_Dependencies, dependencyType);

                    if (_Dependencies.DependenciesByType.TryGetValue(dependencyType, out var _Dependency))
                        foreach (var _Implementation in implementations)
                            options.OnUnregisteredImplementationTypeFound(_Dependency, _Implementation);
                };

            if (options.OnUnregisteredTypeFound != null)
                _Visitor.OnTypeFound = type =>
                {
                    if (_Dependencies.DependenciesByType.ContainsKey(type))
                        return;

                    if (_Dependencies.RegisteredTransitiveDependencies.Contains(type.GetTypeDefinition()))
                        return;

                    options.OnUnregisteredTypeFound(_Dependencies, type);
                };

            _Visitor.VisitAssemblyScan(assemblyScan);

            return dependencies;
        }

        /// <summary>
        /// Adds the specified <paramref name="dependencyType"/> as a dependency.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="dependencyType">The type of dependency. Cannot be null.</param>
        /// <param name="configurationAction">An action to configure the dependency. Can be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/> or <paramref name="dependencyType"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the dependency has already been registered.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>If the dependency has already been registered, use <see cref="ConfigureDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/> instead.</remarks>
        public static TDependencyCollection AddDependency<TDependencyCollection>(
            this TDependencyCollection dependencies,
            Type dependencyType,
            Action<IDependency> configurationAction) where TDependencyCollection : IDependencyCollection
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (dependencyType == null) throw new ArgumentNullException(nameof(dependencyType));
            if (dependencies.GetDependency(dependencyType) != null) throw new InvalidOperationException($"'{dependencyType.Name}' has already been registered as a dependency. Use {nameof(ConfigureDependency)} instead.");

            var _Dependency = dependencies.AddDependency(dependencyType);

            configurationAction?.Invoke(_Dependency);

            return dependencies;
        }

        /// <summary>
        /// Configures a dependency.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="dependencyType">The type of dependency. Cannot be null.</param>
        /// <param name="configurationAction">An action to configure the dependency. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/>, <paramref name="dependencyType"/>, or <paramref name="configurationAction"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the dependency has not been registered.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>If the dependency has not been registered, use <see cref="AddDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/> instead.</remarks>
        public static TDependencyCollection ConfigureDependency<TDependencyCollection>(
            this TDependencyCollection dependencies,
            Type dependencyType,
            Action<IDependency> configurationAction) where TDependencyCollection : IDependencyCollection
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            if (dependencyType == null) throw new ArgumentNullException(nameof(dependencyType));
            if (configurationAction == null) throw new ArgumentNullException(nameof(configurationAction));

            var _Dependency = dependencies.GetDependency(dependencyType) ?? throw new InvalidOperationException($"'{dependencyType.Name}' has not been registered as a dependency. Use {nameof(AddDependency)} instead.");
            configurationAction.Invoke(_Dependency);

            return dependencies;
        }

        /// <summary>
        /// Verifies that the dependency collection is valid.
        /// </summary>
        /// <typeparam name="TDependencyCollection">The type of dependency collection.</typeparam>
        /// <param name="dependencies">The dependency collection to update. Cannot be null.</param>
        /// <param name="configurationAction">An action to configure validation options. Can be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencies"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the dependency collection fails validation.</exception>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        public static TDependencyCollection Validate<TDependencyCollection>(
            this TDependencyCollection dependencies,
            Action<IDependencyCollectionValidationOptions> configurationAction = null) where TDependencyCollection : IDependencyCollection
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));

            var _ValidationCollection = new ValidationCollection(dependencies);

            configurationAction?.Invoke(_ValidationCollection.Options);

            _ValidationCollection.ThrowOnValidationFailure();

            return dependencies;
        }

        internal static ReadOnlyDependencyCollection Read(this IDependencyCollection dependencyCollection)
            => new ReadOnlyDependencyCollection(dependencyCollection ?? throw new ArgumentNullException(nameof(dependencyCollection)));

        #endregion Methods

    }

}
