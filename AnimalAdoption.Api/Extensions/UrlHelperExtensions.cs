using Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtensions
{
    public static LinkDto BuildLink(this IUrlHelper urlHelper, string routeName, object routeValues, string rel, string method)
    {
        return new LinkDto
        {
            Href = urlHelper.Link(routeName, routeValues) ?? string.Empty,
            Rel = rel,
            Method = method
        };
    }
}