namespace Model.EARL;

public static class Vocabulary
{
    private static readonly NodeFactory factory = new();

    public static string NS => "http://www.w3.org/ns/earl#";

    public static INode Assertion { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}Assertion"));

    public static INode AssertedBy { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}assertedBy"));

    public static INode Subject { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}subject"));

    public static INode Test { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}test"));

    public static INode Result { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}result"));

    public static INode Outcome { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}outcome"));

    public static IUriNode TestRequirement { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}TestRequirement"));

    public static IUriNode TestCase { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}TestCase"));

    public static INode Failed { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}failed"));

    public static INode CantTell { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}cantTell"));

    public static INode Info { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}info"));

    public static INode Passed { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}passed"));
    
    public static INode Title { get; } = factory.CreateUriNode(UriFactory.Create("http://purl.org/dc/terms/title"));

    public static INode Description { get; } = factory.CreateUriNode(UriFactory.Create("http://purl.org/dc/terms/description"));

    public static INode Date { get; } = factory.CreateUriNode(UriFactory.Create("http://purl.org/dc/terms/date"));

    public static INode IsPartOf { get; } = factory.CreateUriNode(UriFactory.Create("http://purl.org/dc/terms/isPartOf"));
}
