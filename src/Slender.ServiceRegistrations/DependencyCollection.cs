using Slender.AssemblyScanner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A collection of dependencies that can be used to configure a dependency injection container.
    /// </summary>
    public partial class DependencyCollection : IEnumerable<Dependency>
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, DependencyBuilder> m_BuildersByType = new Dictionary<Type, DependencyBuilder>();
        private readonly List<string> m_RequiredPackages = new List<string>();
        private readonly Dictionary<Type, DependencyBuilder> m_UnregisteredBuildersByType = new Dictionary<Type, DependencyBuilder>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Creates a new instance of a dependency collection.
        /// </summary>
        public DependencyCollection() { }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds the assemblies and scans them for implementations of dependencies.
        /// </summary>
        /// <param name="assemblies">The assemblies to add.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection AddAssemblies(IEnumerable<Assembly> assemblies)
            => this.AddAssemblyScan(AssemblyScan.FromAssemblies(assemblies));

        /// <summary>
        /// Adds the assemblies and scans them for implementations of dependencies.
        /// </summary>
        /// <param name="assembly">An assembly to add.</param>
        /// <param name="additionalAssemblies">Additional assemblies to add.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection AddAssemblies(Assembly assembly, params Assembly[] additionalAssemblies)
            => this.AddAssemblyScan(AssemblyScan.FromAssemblies(assembly, additionalAssemblies));

        /// <summary>
        /// Adds the assembly and scans it for implementations of dependencies.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection AddAssembly(Assembly assembly)
            => this.AddAssemblies(assembly);

        /// <summary>
        /// Adds the assembly scan and visits it for implementations of dependencies.
        /// </summary>
        /// <param name="assemblyScan">The assembly scan to add.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection AddAssemblyScan(IAssemblyScan assemblyScan)
        {
            new DependencyAssemblyScanVisitor
            {
                OnDependencyFound = dependency =>
                {
                    if (!this.m_BuildersByType.ContainsKey(dependency)
                        && !this.m_UnregisteredBuildersByType.ContainsKey(dependency))
                        this.AddUnregisteredDependency(new DependencyBuilder(dependency));
                },
                OnDependencyImplementationsFound = (dependency, implementations) =>
                {
                    if (!this.m_BuildersByType.TryGetValue(dependency, out var _Builder)
                        && !this.m_UnregisteredBuildersByType.TryGetValue(dependency, out _Builder))
                    {
                        _Builder = new DependencyBuilder(dependency);
                        this.AddUnregisteredDependency(_Builder);
                    }

                    _Builder.AddScannedImplementationTypes(implementations, true);
                }
            }.VisitAssemblyScan(assemblyScan);

            return this;
        }

        private void AddBuilder(DependencyBuilder builder)
        {
            this.m_BuildersByType.Add(builder.Dependency.DependencyType, builder);

            if (builder.Dependency.DependencyType.IsGenericTypeDefinition)
                builder.OnScanForImplementations = () =>
                {
                    var _UnregisteredClosedGenericDependencies
                        = this.m_UnregisteredBuildersByType
                            .Values
                            .Where(b => b.Dependency.DependencyType.GUID == builder.Dependency.DependencyType.GUID)
                            .ToList();

                    foreach (var _UnregisteredClosedGenericDependency in _UnregisteredClosedGenericDependencies)
                    {
                        _ = this.m_UnregisteredBuildersByType.Remove(_UnregisteredClosedGenericDependency.Dependency.DependencyType);
                        this.AddUnregisteredDependency(_UnregisteredClosedGenericDependency);
                    }
                };

            // If the dependency already allows scanning, then force a scan.
            // This should only happen for merged builders.
            if (builder.Dependency.AllowScannedImplementationTypes)
                builder.OnScanForImplementations?.Invoke();
        }

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a dependency with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="type">The type of dependency.</param>
        /// <param name="lifetime">The lifetime of the dependency.</param>
        /// <param name="configurationAction">An action to configure the dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="lifetime"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the dependency has already been registered.</exception>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// If the specified <paramref name="configurationAction"/> invokes <see cref="DependencyBuilder.ScanForImplementations"/>, then any
        /// previously found implementations of the dependency will be automatically added through 
        /// <see cref="DependencyBuilder.AddImplementationType(Type)"/> after <paramref name="configurationAction"/> is invoked.<br/>
        /// <br/>
        /// For more information on previously found types, see <see cref="DependencyCollection.AddAssemblyScan(IAssemblyScan)"/>.
        /// </remarks>
        public DependencyCollection AddDependency(Type type, DependencyLifetime lifetime, Action<DependencyBuilder> configurationAction)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            if (this.m_BuildersByType.ContainsKey(type))
                throw new InvalidOperationException($"{type.Name} has already been registered. Use {nameof(ConfigureDependency)} instead.");

            if (this.m_UnregisteredBuildersByType.TryGetValue(type, out var _Builder))
                _ = this.m_UnregisteredBuildersByType.Remove(type);

            else
                _Builder = new DependencyBuilder(type);

            _Builder.Dependency.Lifetime = lifetime;

            // Add the builder before configuring it. This is because if the dependency is an open
            // generic and there are already unregistered closed generic dependencies, the unregistered
            // closed generic dependencies may be registered during the configuration action.
            this.AddBuilder(_Builder);

            configurationAction?.Invoke(_Builder);

            return this;
        }

        /// <summary>
        /// Registers an external package as a transitive dependency.
        /// </summary>
        /// <param name="externalPackageName">The name of the external package.</param>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// This method exists so that consumers can be informed that an external package is a transitive dependency.<br/>
        /// <br/>
        /// This should only be used if the external package doesn't provide a <see cref="DependencyCollection"/>, but instead has
        /// functionality to register itself directly into dependency injection containers.<br/>
        /// <br/>
        /// If a required package hasn't been resolved, it will cause the validation of the dependencies to fail.
        /// </remarks>
        public DependencyCollection AddRequiredPackage(string externalPackageName)
        {
            this.m_RequiredPackages.Add(externalPackageName);
            return this;
        }

        private void AddUnregisteredDependency(DependencyBuilder builder)
        {
            // If this is a closed generic dependency, and there is a registered open generic dependency
            // that allows scanning for implementations, then register this closed generic dependency
            // as an actual dependency, instead of as an unregistered dependency.
            if (builder.Dependency.DependencyType.IsGenericType
                && !builder.Dependency.DependencyType.IsGenericTypeDefinition
                && this.m_BuildersByType.TryGetValue(
                    builder.Dependency.DependencyType.GetGenericTypeDefinition(),
                    out var _OpenGenericBuilder)
                && _OpenGenericBuilder.Dependency.AllowScannedImplementationTypes)
            {
                // The unregistered closed generic dependency should behave like the open generic dependency.
                builder.Dependency.LinkedDependency = _OpenGenericBuilder.Dependency;

                this.m_BuildersByType.Add(builder.Dependency.DependencyType, builder.ScanForImplementations());
            }

            else
                this.m_UnregisteredBuildersByType.Add(builder.Dependency.DependencyType, builder);
        }

        /// <summary>
        /// Configures a dependency.
        /// </summary>
        /// <param name="type">The type of dependency.</param>
        /// <param name="configurationAction">An action to configure the dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> or <paramref name="configurationAction"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the dependency has not been registered.</exception>
        /// <returns>Itself.</returns>
        public DependencyCollection ConfigureDependency(Type type, Action<DependencyBuilder> configurationAction)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (configurationAction is null) throw new ArgumentNullException(nameof(configurationAction));

            if (!this.m_BuildersByType.TryGetValue(type, out var _Dependency))
                throw new InvalidOperationException($"{type.Name} has not been registered. Use {nameof(AddDependency)} instead.");

            configurationAction(_Dependency);

            return this;
        }

        /// <summary>
        /// Gets all required packages that have not been resolved.
        /// </summary>
        /// <returns>A read-only collection of all required packages that have not been resolved.</returns>
        public ReadOnlyCollection<string> GetUnresolvedRequiredPackages()
            => new ReadOnlyCollection<string>(this.m_RequiredPackages);

        IEnumerator<Dependency> IEnumerable<Dependency>.GetEnumerator()
            => this.m_BuildersByType
                .Values
                .Select(b => b.Dependency)
                .Where(r => !r.DependencyType.IsGenericTypeDefinition
                            || !r.AllowScannedImplementationTypes
                            || r.ImplementationFactory != null
                            || r.ImplementationInstance != null
                            || r.ImplementationTypes.Any())
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<DependencyBuilder>)this).GetEnumerator();

        /// <summary>
        /// Merges in all specified <paramref name="dependencies"/>.
        /// </summary>
        /// <param name="dependencies">The dependencies merge in.</param>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// If a <see cref="Dependency"/> is registered in both collections, then the incoming dependency will be merged into the existing dependency by invoking 
        /// <see cref="IDependencyBehaviour.MergeDependencies(DependencyBuilder, Dependency)"/> using the behaviour on the existing dependency.<br/>
        /// <br/>
        /// Additionally, the specified <paramref name="dependencies"/> will be completely cleared as a part of the merge process.
        /// </remarks>
        public DependencyCollection MergeDependencies(DependencyCollection dependencies)
        {
            foreach (var _BuilderByType in dependencies.m_BuildersByType)
                if (this.m_BuildersByType.TryGetValue(_BuilderByType.Key, out var _Builder))
                    _Builder.Dependency.Behaviour.MergeDependencies(_Builder, _BuilderByType.Value.Dependency);

                else
                {
                    if (this.m_UnregisteredBuildersByType.TryGetValue(_BuilderByType.Key, out _Builder))
                    {
                        _ = this.m_UnregisteredBuildersByType.Remove(_BuilderByType.Key);
                        _BuilderByType.Value.AddScannedImplementationTypes(_Builder.ScannedImplementationTypes, true);
                    }

                    this.AddBuilder(_BuilderByType.Value);
                }

            foreach (var _BuilderByType in dependencies.m_UnregisteredBuildersByType)
                if (this.m_BuildersByType.TryGetValue(_BuilderByType.Key, out var _Builder)
                    || this.m_UnregisteredBuildersByType.TryGetValue(_BuilderByType.Key, out _Builder))
                    _Builder.AddScannedImplementationTypes(_BuilderByType.Value.ScannedImplementationTypes, false);

                else
                    this.AddUnregisteredDependency(_BuilderByType.Value);

            foreach (var _RequiredPackage in dependencies.m_RequiredPackages)
                _ = this.AddRequiredPackage(_RequiredPackage);

            dependencies.m_BuildersByType.Clear();
            dependencies.m_UnregisteredBuildersByType.Clear();
            dependencies.m_RequiredPackages.Clear();

            return this;
        }

        /// <summary>
        /// Informs the DependencyCollection that the external package no longer needs to be tracked.
        /// </summary>
        /// <param name="externalPackageName">The name of the external package.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection ResolveRequiredPackage(string externalPackageName)
        {
            _ = this.m_RequiredPackages.Remove(externalPackageName);
            return this;
        }

        /// <summary>
        /// Scans already added assemblies for unregistered dependencies, and invokes the provided action with them.
        /// </summary>
        /// <param name="registrationAction">The action to register dependencies.</param>
        /// <returns>Itself.</returns>
        public DependencyCollection ScanForUnregisteredDependencies(Action<DependencyCollection, Type> registrationAction)
        {
            foreach (var _UnregisteredDependencies in this.m_UnregisteredBuildersByType.Keys.ToList())
                registrationAction(this, _UnregisteredDependencies);

            return this;
        }

        /// <summary>
        /// Verifies that all dependencies have an associated implementation.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <exception cref="Exception">Thrown if any dependencies do not have any associated implementation.</exception>
        public DependencyCollection Validate()
        {
            var _StringBuilder = new StringBuilder(64);

            var _BuildersWithoutImplementations
                = this.m_BuildersByType.Values
                    .Where(b =>
                        !b.Dependency.ImplementationTypes.Any()
                        && b.Dependency.ImplementationFactory == null
                        && b.Dependency.ImplementationInstance == null
                        && (b.Dependency.AllowScannedImplementationTypes || b.Dependency.DependencyType.IsAbstract))
                    .ToList();

            var _BuildersWithInvalidImplementationTypes
                = this.m_BuildersByType.Values.Where(b => b.Dependency.ImplementationTypes.Any(t => t.IsAbstract));

            if (_BuildersWithoutImplementations.Any())
            {
                _ = _StringBuilder.AppendLine("The following dependencies don't have any implementations:");

                foreach (var _Builder in _BuildersWithoutImplementations)
                    _ = _StringBuilder.Append(" - ").AppendLine(_Builder.Dependency.DependencyType.Name);
            }

            if (_BuildersWithInvalidImplementationTypes.Any())
            {
                _ = _StringBuilder.AppendLine("The following dependencies have invalid implementation types:");

                foreach (var _Builder in _BuildersWithInvalidImplementationTypes)
                    _ = _StringBuilder
                            .Append(" - ")
                            .Append(_Builder.Dependency.DependencyType.Name)
                            .AppendLine($" ({string.Join(", ", _Builder.Dependency.ImplementationTypes.Where(r => r.IsAbstract).Select(r => r.Name))})");
            }

            if (this.m_RequiredPackages.Any())
            {
                if (_StringBuilder.Length > 0)
                    _ = _StringBuilder.AppendLine();

                _ = _StringBuilder.AppendLine("The following packages are required and have not been resolved:");

                foreach (var _RequiredPackage in this.m_RequiredPackages)
                    _ = _StringBuilder.Append(" - ").AppendLine(_RequiredPackage);
            }

            return _StringBuilder.Length == 0 ? this : throw new Exception(_StringBuilder.ToString());
        }

        #endregion Methods

    }

}
