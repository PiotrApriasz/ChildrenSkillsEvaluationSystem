using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using ChildrenEvaluationSystem.Infrastructure.Database;
using MediatR;

namespace ChildrenEvaluationSystem.Features.Groups;

public record GetGroupQuery(string GroudId) : IRequest<Group>;

public class GetGroup(IRepository<Group> repo, ICurrentUserService currentUser) : 
    IRequestHandler<GetGroupQuery, Group>
{
    public async Task<Group> Handle(GetGroupQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var groupPk = PartitionKeyResolver.ForGroup(currentUser.UserId);
        
        var group = await repo.GetAsync(request.GroudId, groupPk, ct);
        
        if (group == null || group.UserId != currentUser.UserId)
            throw new KeyNotFoundException("Group not found");

        return group;
    }
}