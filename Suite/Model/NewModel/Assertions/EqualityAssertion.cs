namespace Model.NewModel.Assertions;

public partial class EqualityAssertion
{
    public override Result Execute(Context context)
    {
        var actual = context.Get(Param);
        return actual == Value
            ? new Result { Outcome = "Pass", Info = $"Parameter '{Param}' equals '{Value}'." }
            : new Result { Outcome = "Fail", Info = $"Parameter '{Param}' expected '{Value}', but got '{actual}'." };
    }
}
