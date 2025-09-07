using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using MediatR;

namespace ChildrenEvaluationSystem.Features.Groups;

public record CountUserGroupsQuery : IRequest<int>;

public class CountUserGroupsHandler(IRepository<Group> repo, ICurrentUserService currentUser) : 
    IRequestHandler<CountUserGroupsQuery, int>
{
    public async Task<int> Handle(CountUserGroupsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();
        
        var result = await repo.CountByUserIdAsync(currentUser.UserId, cancellationToken);
        
        return result;
    }
}