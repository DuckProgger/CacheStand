﻿namespace Core.Services;

public interface IDistributedCacheWrapper
{
    Task<TValue?> GetValueAsync<TValue>(string key);
    
    Task SetValueAsync<TValue>(string key, TValue value);
}