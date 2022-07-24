
public interface IModifier<T>
{
    public T Apply(Stat<T> stat);
}