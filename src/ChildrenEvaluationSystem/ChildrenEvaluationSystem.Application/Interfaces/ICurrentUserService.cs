namespace ChildrenEvaluationSystem.Application.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    bool IsAuthenticated { get; }
}