
public class Tuple<T1, T2>
{
    public T1 typeAction { get; private set; }
    public T2 typeParameter { get; private set; }
    internal Tuple(T1 typeAction, T2 typeParameter)
    {
        this.typeAction = typeAction;
        this.typeParameter = typeParameter;
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 typeAction, T2 typeParameter)
    {
        var tuple = new Tuple<T1, T2>(typeAction, typeParameter);
        return tuple;
    }
}