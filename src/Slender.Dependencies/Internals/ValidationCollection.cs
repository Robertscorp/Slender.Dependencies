using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slender.Dependencies.Internals
{

    internal class ValidationCollection : IDependencyCollection, IDependencyCollectionValidationOptions
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<ValidationDependency> m_Dependencies = new List<ValidationDependency>();
        private readonly List<Type> m_TransitiveDependencies = new List<Type>();

        private bool m_IgnoreInvalidImplementations;
        private bool m_IgnoreMissingImplementations;
        private bool m_IgnoreMissingLifetimes;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ValidationCollection(IDependencyCollection dependencies)
            => dependencies.AddToDependencyCollection(this);

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public void AddDependency(IDependency dependency)
            => this.m_Dependencies.Add(new ValidationDependency(dependency));

        public void AddTransitiveDependency(Type transitiveDependencyType)
        {
            if (!this.m_TransitiveDependencies.Contains(transitiveDependencyType))
                this.m_TransitiveDependencies.Add(transitiveDependencyType);
        }

        IDependency IDependencyCollection.AddDependency(Type dependencyType)
            => throw new NotImplementedException();

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreInvalidImplementations()
        {
            this.m_IgnoreInvalidImplementations = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreMissingImplementations()
        {
            this.m_IgnoreMissingImplementations = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.IgnoreMissingLifetimes()
        {
            this.m_IgnoreMissingLifetimes = true;
            return this;
        }

        IDependencyCollectionValidationOptions IDependencyCollectionValidationOptions.ResolveTransitiveDependency(Type transitiveDependencyType)
            => this.m_TransitiveDependencies.Remove(transitiveDependencyType)
                ? this
                : throw new InvalidOperationException($"'{transitiveDependencyType.Name}' is not a transitive dependency.");

        void IDependencyCollection.AddToDependencyCollection(IDependencyCollection dependencies)
            => throw new NotImplementedException();

        IDependency IDependencyCollection.GetDependency(Type dependencyType)
            => throw new NotImplementedException();

        public void ThrowOnValidationFailure()
        {
            var _StringBuilder = new StringBuilder(64);

            if (!this.m_IgnoreMissingLifetimes)
            {
                var _DependenciesWithoutLifetime = this.m_Dependencies.Where(d => d.HasNoLifetime()).ToList();
                if (_DependenciesWithoutLifetime.Any())
                {
                    _ = _StringBuilder.AppendLine("The following dependencies don't have a lifetime:");

                    foreach (var _Dependency in _DependenciesWithoutLifetime)
                        _ = _StringBuilder.Append(" - ").AppendLine(_Dependency.GetDependencyType().Name);
                }
            }

            if (!this.m_IgnoreMissingImplementations)
            {
                var _DependenciesWithoutImplementations = this.m_Dependencies.Where(d => d.HasNoImplementations()).ToList();
                if (_DependenciesWithoutImplementations.Any())
                {
                    if (_StringBuilder.Length > 0)
                        _ = _StringBuilder.AppendLine();

                    _ = _StringBuilder.AppendLine("The following dependencies don't have any implementations:");

                    foreach (var _Dependency in _DependenciesWithoutImplementations)
                        _ = _StringBuilder.Append(" - ").AppendLine(_Dependency.GetDependencyType().Name);
                }
            }

            if (!this.m_IgnoreInvalidImplementations)
            {
                var _DependenciesWithInvalidImplementations = this.m_Dependencies.Where(d => d.HasInvalidImplementations()).ToList();
                if (_DependenciesWithInvalidImplementations.Any())
                {
                    if (_StringBuilder.Length > 0)
                        _ = _StringBuilder.AppendLine();

                    _ = _StringBuilder.AppendLine("The following dependencies have invalid implementations:");

                    foreach (var _Dependency in _DependenciesWithInvalidImplementations)
                        _ = _StringBuilder.Append(" - ").AppendLine(_Dependency.GetDependencyType().Name);
                }
            }

            if (this.m_TransitiveDependencies.Any())
            {
                if (_StringBuilder.Length > 0)
                    _ = _StringBuilder.AppendLine();

                _ = _StringBuilder.AppendLine("The following transitive dependencies have not been resolved:");

                foreach (var _TransitiveDependency in this.m_TransitiveDependencies)
                    _ = _StringBuilder.Append(" - ").AppendLine(_TransitiveDependency.Name);
            }

            if (_StringBuilder.Length > 0)
                throw new Exception(_StringBuilder.ToString());
        }

        #endregion Methods

    }

}
