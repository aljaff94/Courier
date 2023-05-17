namespace Courier;

public class EndpointAttribute : Attribute
{
    private readonly HttpMethods _method;
    private readonly string _routeTemplate;

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string[]? Tags { get; set; }
    public bool UseOpenApi { get; set; }

    public EndpointAttribute(HttpMethods method, string routeTemplate)
    {
        _method = method;
        _routeTemplate = routeTemplate;
    }
}