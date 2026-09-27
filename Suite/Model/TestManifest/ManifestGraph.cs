namespace Model.TestManifest;

public class ManifestGraph(ITripleStore store) : WrapperTripleStore(store)
{
    public IEnumerable<Manifest> Manifests => Default.InstancesOf(Vocabulary.Manifest, Manifest.Wrap);

    private IGraph Default => this[(IRefNode)null!];
}
