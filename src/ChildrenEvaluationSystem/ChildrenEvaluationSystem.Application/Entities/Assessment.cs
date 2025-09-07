using ChildrenEvaluationSystem.Application.Entities.Base;
using Newtonsoft.Json;

namespace ChildrenEvaluationSystem.Application.Entities;

public class Assessment : BaseEntity
{
    [JsonProperty("groupId")]
    public required string GroupId { get; set; }
    
    [JsonProperty("childId")]
    public required string  ChildId { get; set; }
    
    public required string TemplateId { get; set; }
    public int TemplateVersion { get; set; }
    public DateOnly Date { get; set; }                
    
    public required Dictionary<string, System.Text.Json.JsonElement> Answers { get; set; }
    
    public double? ScoreOverall { get; set; }
    public List<string>? Tags { get; set; }
    
}