using ChildrenEvaluationSystem.Application.Entities.Base;
using ChildrenEvaluationSystem.Application.Models.Templates;

namespace ChildrenEvaluationSystem.Application.Entities;

public class AssessmentTemplate : BaseEntity
{
    public required string Name { get; set; }
    public int Version { get; set; } = 1;
    public required List<TemplateField> Fields { get; set; }
    public TemplateState State { get; set; }
}