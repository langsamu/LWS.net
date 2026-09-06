namespace Model.TestManifest;

internal static class Vocabulary
{
    private static readonly NodeFactory factory = new();

    internal static string NS => "https://www.w3.org/ns/test-manifest#";

    internal static INode Manifest { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}Manifest"));

    internal static INode Entries { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}entries"));

    internal static INode Name { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}name"));

    internal static INode Status { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}status"));

    internal static INode Include { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}include"));

    internal static INode Label { get; } = factory.CreateUriNode(UriFactory.Create("http://www.w3.org/2000/01/rdf-schema#label"));
}
