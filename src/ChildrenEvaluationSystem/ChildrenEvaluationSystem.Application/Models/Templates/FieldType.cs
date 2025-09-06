namespace ChildrenEvaluationSystem.Application.Models.Templates;

public enum FieldType
{
    Text,        // string
    Number,      // double
    Date,        // yyyy-MM-dd or ISO 8601
    Select,      // one value from selector
    Checkbox     // bool
}