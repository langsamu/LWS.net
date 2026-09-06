using VDS.RDF.Parsing;

namespace Model;

internal static class Vocabulary
{
    private static readonly NodeFactory factory = new();

    internal static INode RdfType { get; } = factory.CreateUriNode(UriFactory.Create(RdfSpecsHelper.RdfType));
}
