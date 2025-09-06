namespace ChildrenEvaluationSystem.Application.Models.Templates;

public class TemplateField
{
    public required string Key { get; set; }
    public required string Label { get; set; }
    public FieldType Type { get; set; }
    
    public bool Required { get; set; } = false;
    
    public double? Min { get; set; }
    public double? Max { get; set; }
    
    public int? MaxLength { get; set; }
    
    public List<string>? Options { get; set; }
}