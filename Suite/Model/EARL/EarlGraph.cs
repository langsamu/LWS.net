namespace Model.EARL;

public class EarlGraph : WrapperGraph
{
    public EarlGraph(IGraph original) : base(original)
    {
        NamespaceMap.AddNamespace("dc", new Uri("http://purl.org/dc/terms/"));
        NamespaceMap.AddNamespace("", new Uri(Vocabulary.NS));
    }

    public IEnumerable<Assertion> Assertions => this.SubjectsOf(Vocabulary.AssertedBy, Assertion.Wrap);
}
