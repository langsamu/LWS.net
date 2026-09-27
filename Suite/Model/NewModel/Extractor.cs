namespace Model.NewModel;

public abstract partial class Extractor
{
    public async Task Execute(Context context, HttpResponseMessage response)
    {
        context.Set(ParamName, await Extract(context, response));
    }

    protected abstract Task<string> Extract(Context context, HttpResponseMessage response);
}
