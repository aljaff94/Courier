namespace Courier;

public class EndpointAttribute : Attribute
{
    private readonly HttpMethods _method;
    private readonly string _routeTemplate;

    public EndpointAttribute(HttpMethods method, string routeTemplate)
    {
        _method = method;
        _routeTemplate = routeTemplate;
    }
}