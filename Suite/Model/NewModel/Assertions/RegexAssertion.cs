namespace Model.NewModel.Assertions;

public partial class RegexAssertion
{
    public override Result Execute(Context context)
    {
        var actual = context.Get(Param);
        return Pattern.IsMatch(actual)
            ? new Result { Outcome = "Pass", Info = $"Parameter '{Param}' matches '{Pattern}'." }
            : new Result { Outcome = "Fail", Info = $"Parameter '{Param}' expected to match '{Pattern}', but got '{actual}'." };
    }
}
