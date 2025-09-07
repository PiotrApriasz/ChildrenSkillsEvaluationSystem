using ChildrenEvaluationSystem.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Infrastructure.Database;

public class CosmosContainerProvider(CosmosClient client, CosmosOptions options) : ICosmosContainerProvider
{
    private Microsoft.Azure.Cosmos.Database? _db;

    public Container GetContainer(string name)
    {
        if (_db is null)
            InitDatabase();
        return _db!.GetContainer(name);
    }

    private void InitDatabase()
    {
        if (_db != null) return;
        if (_db != null) return;
        _db = client.GetDatabase(options.DatabaseId);
    }
}