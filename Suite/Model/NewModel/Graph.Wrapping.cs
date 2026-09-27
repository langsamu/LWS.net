namespace Model.NewModel;

public class Graph(IGraph original) : WrapperGraph(original)
{
    public Manifest Manifest => this.SubjectsOf(Vocabulary.Tests, Manifest.Wrap).Single();
}
