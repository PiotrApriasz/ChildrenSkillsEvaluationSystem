using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Application.Interfaces;

public interface ICosmosContainerProvider
{
    Container GetContainer(string name);
}