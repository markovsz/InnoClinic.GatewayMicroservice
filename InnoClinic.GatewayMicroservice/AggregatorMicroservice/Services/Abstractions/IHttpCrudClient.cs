namespace AggregatorMicroservice.Services.Abstractions;

public interface IHttpCrudClient
{
    Task<TOut> PostAsync<TIn, TOut>(string uri, TIn entity, string authParam);
    Task<TOut> GetAsync<TOut>(string uri, string authParam);
}
