namespace Core.Wrappers;

public interface ICacheWrapper
{
    Task<TValue?> GetValueAsync<TValue>(string key);

    Task SetValueAsync<TValue>(string key, TValue value);
}