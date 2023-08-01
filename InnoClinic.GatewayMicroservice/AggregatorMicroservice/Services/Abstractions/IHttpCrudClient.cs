namespace AggregatorMicroservice.Services.Abstractions;

public interface IHttpCrudClient
{
    Task<TOut> PostAsync<TIn, TOut>(string uri, TIn entity, string? authParam = null);
    Task PostAsync<TIn>(string uri, TIn entity, string? authParam = null);
    Task<TOut> GetAsync<TOut>(string uri, string? authParam = null);
    Task PutAsync<TIn>(string uri, TIn entity, string? authParam = null);
}
