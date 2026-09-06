using Model.TestManifest;
using System.Reflection;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;

namespace Model;

public class Resources
{
    public static readonly Uri Base = new("http://lws.example.com/");

    private static Type ThisClass => typeof(Resources);

    private static Assembly ThisAssembly => ThisClass.Assembly;

    private static string ThisType => ThisClass.FullName!;

    public static Stream Stream(string name) =>
        ThisAssembly.GetManifestResourceStream($"{ThisType}.{name.Replace("/", ".").Replace("-", "_")}")
        ?? throw new Exception($"Resource not found:  [{ThisType}.{name}]"); // TODO: Specific exception

    public static TextReader Reader(string name) => new StreamReader(Stream(name));

    public static string String(string name) => Reader(name).ReadToEnd();

    public static RemoteDocument Loader(Uri uri, JsonLdLoaderOptions _)
    {
        if (!Base.IsBaseOf(uri))
        {
            throw new Exception("Unknown context document URI");
        }

        return new RemoteDocument { Document = String(Base.MakeRelativeUri(uri).ToString()) };
    }

    public static ManifestGraph ManifestGraph
    {
        get
        {
            var parser = new JsonLdParser(new()
            {
                Base = Base,
                DocumentLoader = Loader
            });

            var store = new TripleStore();
            parser.Load(store, Reader("manifest.jsonld"));

            var manifestG = new ManifestGraph(store);

            foreach (var manifest in manifestG.Manifests.ToArray())
            {
                foreach (var include in manifest.Include)
                {
                    var c = Base.MakeRelativeUri(include);
                    var d = c.ToString();

                    parser.Load(store, Reader(d));
                }
            }

            return manifestG;
        }
    }
}
