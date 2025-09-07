using ChildrenEvaluationSystem.Application.Entities;
using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Infrastructure.Database;

public static class PartitionKeyResolver
{
    public static PartitionKey ForEntity(object entity) => entity switch
    {
        Assessment a => new PartitionKeyBuilder()
            .Add(a.ChildId.ToString())
            .Add(a.GroupId.ToString())
            .Add(a.UserId.ToString())
            .Build(),
        Children c => new PartitionKeyBuilder()
            .Add(c.GroupId.ToString())
            .Add(c.UserId.ToString())
            .Build(),
        AssessmentTemplate t => new PartitionKeyBuilder()
            .Add(t.UserId.ToString())
            .Build(),
        Group g => new PartitionKeyBuilder()
            .Add(g.UserId.ToString())
            .Build(),
        _ => throw new InvalidOperationException("No partition key definition for this entity type.")
    };

    public static PartitionKey ForAssessment(string childId, string groupId, string userId) =>
        new PartitionKeyBuilder().Add(childId).Add(groupId).Add(userId).Build();

    public static PartitionKey ForChildren(string groupId, string userId) =>
        new PartitionKeyBuilder().Add(groupId).Add(userId).Build();

    public static PartitionKey ForTemplate(string userId) =>
        new PartitionKeyBuilder().Add(userId).Build();

    public static PartitionKey ForGroup(string userId) =>
        new PartitionKeyBuilder().Add(userId).Build();
}