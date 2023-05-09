using AggregatorMicroservice.Services.Abstractions;
using Newtonsoft.Json;
using System.Text;

namespace AggregatorMicroservice.Services;

public class HttpCrudClient : IHttpCrudClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpCrudClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> BasePostAsync<TIn>(string uri, TIn entity, string? authParam)
    {
        var httpContext = _httpClientFactory.CreateClient();
        var data = JsonConvert.SerializeObject(entity);
        var requestContent = new HttpRequestMessage(HttpMethod.Post, uri);
        requestContent.Content = new StringContent(data);
        requestContent.Content.Headers.Remove("Content-Type");
        requestContent.Content.Headers.Add("Content-Type", "application/json");
        if(authParam is not null)
            httpContext.DefaultRequestHeaders.Add("Authorization", "Bearer " + authParam);
        var response = await httpContext.SendAsync(requestContent);
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
        return response;
    }

    public async Task<TOut> PostAsync<TIn, TOut>(string uri, TIn entity, string? authParam = null)
    {
        var response = await BasePostAsync(uri, entity, authParam);
        var responseBody = await response.Content.ReadAsStringAsync();
        var content = JsonConvert.DeserializeObject<TOut>(responseBody);
        return content;
    }

    public async Task PostAsync<TIn>(string uri, TIn entity, string? authParam = null)
    {
        await BasePostAsync(uri, entity, authParam);
    }

    public async Task<TOut> GetAsync<TOut>(string uri, string? authParam = null)
    {
        var httpContext = _httpClientFactory.CreateClient();
        if (authParam is not null)
            httpContext.DefaultRequestHeaders.Add("Authorization", "Bearer " + authParam);
        var response = await httpContext.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
            throw new Exception();
        var responseBody = await response.Content.ReadAsStringAsync();
        var content = JsonConvert.DeserializeObject<TOut>(responseBody);
        return content;
    }
}
