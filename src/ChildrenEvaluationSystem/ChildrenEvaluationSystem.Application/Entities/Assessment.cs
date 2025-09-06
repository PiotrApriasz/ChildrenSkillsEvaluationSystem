using ChildrenEvaluationSystem.Application.Entities.Base;

namespace ChildrenEvaluationSystem.Application.Entities;

public class Assessment : BaseEntity
{
    public required string GroupId { get; set; }
    public required string  ChildId { get; set; }
    public required string TemplateId { get; set; }
    public int TemplateVersion { get; set; }
    public DateOnly Date { get; set; }                
    
    public required Dictionary<string, System.Text.Json.JsonElement> Answers { get; set; }
    
    public double? ScoreOverall { get; set; }
    public List<string>? Tags { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public required string CreatedBy { get; set; }
    
}