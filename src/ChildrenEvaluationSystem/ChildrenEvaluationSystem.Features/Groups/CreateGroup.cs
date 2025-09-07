using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Interfaces;
using ChildrenEvaluationSystem.Infrastructure.Database;
using MediatR;

namespace ChildrenEvaluationSystem.Features.Groups;

public record CreateGroupCommand(string Name) : IRequest<string>;

public class CreateGroupHandler(IRepository<Group> repo, ICurrentUserService currentUser)
    : IRequestHandler<CreateGroupCommand, string>
{
    public async Task<string> Handle(CreateGroupCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var group = new Group
        {
            UserId = currentUser.UserId,
            GroupName = request.Name
        };

        await repo.AddAsync(group, ct);
        return group.Id;
    }
}