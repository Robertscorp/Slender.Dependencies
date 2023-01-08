namespace Slender.ServiceRegistrations
{

    public class RegistrationLifetime
    {

        #region - - - - - - Constructors - - - - - -

        private RegistrationLifetime(string name, bool allowImplementationInstances)
        {
            this.Name = name;
            this.AllowImplementationInstances = allowImplementationInstances;
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public bool AllowImplementationInstances { get; }

        public string Name { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public static RegistrationLifetime Scoped() => new RegistrationLifetime("Scoped", false);

        public static RegistrationLifetime Singleton() => new RegistrationLifetime("Singleton", true);

        public static RegistrationLifetime Transient() => new RegistrationLifetime("Transient", false);

        #endregion Methods

    }

}
