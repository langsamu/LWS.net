namespace Model.LWST;

internal static class Vocabulary
{
    static readonly NodeFactory factory = new();

    internal static string NS => "https://www.w3.org/ns/lws-tests/v1#";

    internal static INode Authentication { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}authentication"));

    internal static INode Hierarchy { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}hierarchy"));

    internal static INode Prereqs { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}prereqs"));

    internal static INode Request { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}request"));

    internal static INode Response { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}response"));

    internal static INode Traits { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}traits"));

    internal static INode StatusCode { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}statusCode"));

    internal static INode ContentType { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}contentType"));

    internal static INode LinkHeaders { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}linkHeaders"));

    internal static INode OtherHeaders { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}otherHeaders"));

    internal static INode Rel { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}rel"));

    internal static INode HrefTemplate { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}hrefTemplate"));

    internal static INode BodyURL { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}bodyURL"));

    internal static INode Url { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}url"));

    internal static INode Method { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}method"));

    internal static INode AuthenticationChallenge { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}authenticationChallenge"));

    internal static INode HeaderName { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}headerName"));

    internal static INode HeaderValue { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}headerValue"));

    internal static INode Body { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}body"));

    internal static INode Slug { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}slug"));
}
