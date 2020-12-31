namespace StarFruit2.Common
{
    public interface IDescriptionProvider
    {
        string? GetDescription<T>(T source, string route=null);
    }
}
