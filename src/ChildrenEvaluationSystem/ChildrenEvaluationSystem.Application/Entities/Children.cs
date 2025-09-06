using ChildrenEvaluationSystem.Application.Entities.Base;

namespace ChildrenEvaluationSystem.Application.Entities;

public class Children : BaseEntity
{
    public required string GroupId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}