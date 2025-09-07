using Newtonsoft.Json;

namespace ChildrenEvaluationSystem.Application.Entities.Base;

public class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [JsonProperty("userId")]
    public required string UserId { get; set; }
    = Guid.NewGuid().ToString("N");
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}