using ChildrenEvaluationSystem.Application.Interfaces;
using ChildrenEvaluationSystem.Application.Entities;
using MediatR;

namespace ChildrenEvaluationSystem.Features.Children;

public record CountUserChildrenQuery : IRequest<int>;

public class CountUserChildren(IRepository<Child> repo, ICurrentUserService currentUser) : 
    IRequestHandler<CountUserChildrenQuery, int>
{
    public async Task<int> Handle(CountUserChildrenQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();
        
        var result = await repo.CountByUserIdAsync(currentUser.UserId, cancellationToken);
        
        return result;
    }
}