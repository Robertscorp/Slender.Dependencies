namespace Slender.Dependencies.Tests.Support
{

    public abstract class AbstractService { }

    public class ClosedGenericServiceImplementation : IGenericService<object> { }

    public class ClosedGenericServiceImplementation2 : IGenericService<object> { }

    public class ConcreteService { }

    public interface IGenericService<TGeneric> { }

    public interface IGenericService2<TGeneric> { }

    public interface IService { }

    public interface IService2 { }

    public interface IUnimplementedService { }

    public class OpenGenericOtherImplementation<TGeneric> { }

    public class OpenGenericServiceImplementation<TGeneric> : IGenericService<TGeneric> { }

    public class ServiceDecorator : IService { }

    public class ServiceImplementation : IService { }

    public class ServiceImplementation2 : IService { }

    public abstract class UninheritedAbstractService { }

}
