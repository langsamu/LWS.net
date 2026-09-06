namespace Model.TestManifest;

public class ManifestGraph(ITripleStore store) : WrapperTripleStore(store)
{
    private IGraph Default => this[(IRefNode)null!];

    public IEnumerable<Manifest> Manifests => Default.InstancesOf(Vocabulary.Manifest, Manifest.Wrap);
}
