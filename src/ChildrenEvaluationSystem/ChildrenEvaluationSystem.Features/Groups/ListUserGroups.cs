using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using ChildrenEvaluationSystem.Application.Models;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Features.Groups;

public record ListUserGroupsQuery : IRequest<IReadOnlyList<GroupListItem>>;

public class ListUserGroupsHandler(IRepository<Group> groupRepo, IRepository<Child> childRepo, ICurrentUserService currentUser) : 
    IRequestHandler<ListUserGroupsQuery, IReadOnlyList<GroupListItem>>
{
    public async Task<IReadOnlyList<GroupListItem>> Handle(ListUserGroupsQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var groups = await groupRepo.GetAllByUserIdAsync(currentUser.UserId, ct);
        
        var result = new List<GroupListItem>();

        foreach (var group in groups)
        {
            var childrenCount = 0;
            var query =
                $"SELECT c.id FROM c WHERE c.groupId = '{group.Id}' AND c.userId = '{currentUser.UserId}'";

            await foreach (var _ in childRepo.QueryAsync(query, opts: null, ct))
                childrenCount++;

            result.Add(new GroupListItem(group.Id, group.GroupName, childrenCount));
        }

        return result;
    }
}