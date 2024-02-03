using Slender.Dependencies.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slender.Dependencies.Internals
{

    internal class ValidationCollection : IDependencyCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<ValidationDependency> m_Dependencies = new List<ValidationDependency>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ValidationCollection(IDependencyCollection dependencies)
            => dependencies.AddToDependencyCollection(this);

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal DependencyCollectionValidationOptions Options { get; } = new DependencyCollectionValidationOptions();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddDependency(IDependency dependency)
            => this.m_Dependencies.Add(new ValidationDependency(dependency));

        public void AddTransitiveDependency(Type transitiveDependencyType)
        {
            if (!this.Options.TransitiveDependencies.Contains(transitiveDependencyType))
                this.Options.TransitiveDependencies.Add(transitiveDependencyType);
        }

        IDependency IDependencyCollection.AddDependency(Type dependencyType)
            => throw new NotImplementedException();

        void IDependencyCollection.AddToDependencyCollection(IDependencyCollection dependencies)
            => throw new NotImplementedException();

        IDependency IDependencyCollection.GetDependency(Type dependencyType)
            => throw new NotImplementedException();

        public void ThrowOnValidationFailure()
        {
            var _StringBuilder = new StringBuilder(64);

            if (!this.Options.ShouldIgnoreMissingLifetimes)
            {
                var _DependenciesWithoutLifetime = this.m_Dependencies.Where(d => d.HasNoLifetime()).ToList();
                if (_DependenciesWithoutLifetime.Any())
                {
                    _ = _StringBuilder.AppendLine("The following dependencies don't have a lifetime:");

                    foreach (var _Dependency in _DependenciesWithoutLifetime)
                        _ = _StringBuilder.Append(" - ").AppendLine(_Dependency.GetDependencyType().Name);
                }
            }

            if (!this.Options.ShouldIgnoreMissingImplementations)
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

            if (!this.Options.ShouldIgnoreInvalidImplementations)
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

            if (this.Options.TransitiveDependencies.Any())
            {
                if (_StringBuilder.Length > 0)
                    _ = _StringBuilder.AppendLine();

                _ = _StringBuilder.AppendLine("The following transitive dependencies have not been resolved:");

                foreach (var _TransitiveDependency in this.Options.TransitiveDependencies)
                    _ = _StringBuilder.Append(" - ").AppendLine(_TransitiveDependency.Name);
            }

            if (_StringBuilder.Length > 0)
                throw new Exception(_StringBuilder.ToString());
        }

        #endregion Methods

    }

}
