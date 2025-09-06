using ChildrenEvaluationSystem.Application.Entities.Base;

namespace ChildrenEvaluationSystem.Application.Entities;

public class Group : BaseEntity
{
    public required string GroupName { get; set; }
}