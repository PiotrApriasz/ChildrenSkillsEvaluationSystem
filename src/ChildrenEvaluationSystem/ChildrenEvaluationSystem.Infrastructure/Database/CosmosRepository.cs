using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Infrastructure.Database;

public class CosmosRepository<T> : IRepository<T>
{
    private readonly ICosmosContainerProvider _provider;
    private readonly string _containerName;

    private readonly Dictionary<Type,string> _containerMap = new()
    {
        { typeof(Group), "groups" },
        { typeof(Children), "childrens" },
        { typeof(AssessmentTemplate), "assessment-templates" },
        { typeof(Assessment), "assessments" }
    };

    public CosmosRepository(ICosmosContainerProvider provider)
    {
        _provider = provider;
        _containerName = _containerMap.TryGetValue(typeof(T), out var n)
            ? n
            : throw new InvalidOperationException($"No container mapping for {typeof(T).Name}");
    }

    public async Task<T?> GetAsync(string id, PartitionKey pk, CancellationToken ct = default)
    {
        var container = _provider.GetContainer(_containerName);
        try
        {
            var resp = await container.ReadItemAsync<T>(id, pk, cancellationToken: ct);
            return resp.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        var container = _provider.GetContainer(_containerName);
        var pk = PartitionKeyResolver.ForEntity(entity!);
        var resp = await container.CreateItemAsync(entity, pk, cancellationToken: ct);
        return resp.Resource;
    }

    public async Task<T> UpsertAsync(T entity, CancellationToken ct = default)
    {
        var container = _provider.GetContainer(_containerName);
        var pk = PartitionKeyResolver.ForEntity(entity!);
        var resp = await container.UpsertItemAsync(entity, pk, cancellationToken: ct);
        return resp.Resource;
    }

    public async Task DeleteAsync(string id, PartitionKey pk, CancellationToken ct = default)
    {
        var container = _provider.GetContainer(_containerName);
        await container.DeleteItemAsync<T>(id, pk, cancellationToken: ct);
    }

    public async IAsyncEnumerable<T> QueryAsync(string? query = null, QueryRequestOptions? opts = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var container = _provider.GetContainer(_containerName);
        var q = string.IsNullOrWhiteSpace(query) ? "SELECT * FROM c" : query;
        using var iterator = container.GetItemQueryIterator<T>(new QueryDefinition(q), requestOptions: opts);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(ct);
            foreach (var item in page)
                yield return item;
        }
    }
    
    public async Task<IReadOnlyList<T>> GetAllByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var query = $"SELECT * FROM c WHERE c.userId = '{userId}'";
        var list = new List<T>();
        await foreach (var item in QueryAsync(query, opts: null, ct))
            list.Add(item);
        return list;
    }
    
    public async Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var query = $"SELECT c.id FROM c WHERE c.userId = '{userId}'";
        var count = 0;
        await foreach (var _ in QueryAsync(query, opts: null, ct))
            count++;
        return count;
    }
}