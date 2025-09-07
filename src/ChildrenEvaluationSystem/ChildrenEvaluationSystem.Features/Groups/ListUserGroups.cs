using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace ChildrenEvaluationSystem.Features.Groups;

public record ListUserGroupsQuery : IRequest<IReadOnlyList<Group>>;

public class ListUserGroupsHandler(IRepository<Group> repo, ICurrentUserService currentUser) : 
    IRequestHandler<ListUserGroupsQuery, IReadOnlyList<Group>>
{
    public async Task<IReadOnlyList<Group>> Handle(ListUserGroupsQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var result = await repo.GetAllByUserIdAsync(currentUser.UserId, ct);

        return result;
    }
}