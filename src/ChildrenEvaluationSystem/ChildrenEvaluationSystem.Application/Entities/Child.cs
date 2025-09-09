using ChildrenEvaluationSystem.Application.Entities.Base;
using Newtonsoft.Json;

namespace ChildrenEvaluationSystem.Application.Entities;

public class Child : BaseEntity
{
    [JsonProperty("groupId")]
    public required string GroupId { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}