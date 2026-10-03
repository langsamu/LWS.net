using VDS.RDF.Parsing;

namespace Model.NewModel;

internal static class Vocabulary
{
    private static readonly NodeFactory factory = new();

    internal static string NS => "http://example.com/";

    internal static INode Tests { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}tests"));

    internal static INode Name { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}name"));

    internal static INode Steps { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}steps"));

    internal static INode Request { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}request"));

    internal static INode Headers { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}headers"));

    internal static INode RdfType { get; } = factory.CreateUriNode(UriFactory.Create(RdfSpecsHelper.RdfType));

    internal static INode Body { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}body"));

    internal static INode BodyExtractor { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}BodyExtractor"));

    internal static INode Method { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}method"));

    internal static INode Uri { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}uri"));

    internal static INode Base { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}base"));

    internal static INode Relative { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}relative"));

    internal static INode Value { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}value"));

    internal static INode Extractors { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}extractors"));

    internal static INode Param { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}param"));

    internal static INode Path { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}path"));

    internal static INode Header { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}header"));

    internal static INode Assertion { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}assertion"));

    internal static INode Type { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}type"));

    internal static INode StatusCodeExtractor { get; } = factory.CreateUriNode(UriFactory.Create($"{NS}StatusCodeExtractor"));
}
