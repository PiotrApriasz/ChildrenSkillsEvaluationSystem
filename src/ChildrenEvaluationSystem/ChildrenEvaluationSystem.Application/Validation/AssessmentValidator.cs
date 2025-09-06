using System.Text.Json;
using ChildrenEvaluationSystem.Application.Entities;
using ChildrenEvaluationSystem.Application.Models.Templates;

namespace ChildrenEvaluationSystem.Application.Validation;

public class AssessmentValidator
{
    public static List<ValidationError> Validate(AssessmentTemplate template, Dictionary<string, JsonElement> answers)
    {
        var errors = new List<ValidationError>();
        var fieldsByKey = template.Fields.ToDictionary(f => f.Key, StringComparer.Ordinal);
        
        foreach (var f in template.Fields)
        {
            if (!answers.TryGetValue(f.Key, out var value))
            {
                if (f.Required)
                    errors.Add(new ValidationError { Field = f.Key, Message = "Field is required." });
                continue;
            }

            switch (f.Type)
            {
                case FieldType.Text:
                    if (value.ValueKind != JsonValueKind.String)
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value should be text." });
                    else if (f.MaxLength.HasValue && value.GetString()!.Length > f.MaxLength.Value)
                        errors.Add(new ValidationError { Field = f.Key, Message = $"Max length: {f.MaxLength}." });
                    break;

                case FieldType.Number:
                    if (!TryGetDouble(value, out var num))
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value should be number." });
                    else
                    {
                        if (f.Min.HasValue && num < f.Min.Value)
                            errors.Add(new ValidationError { Field = f.Key, Message = $"Value < {f.Min}." });
                        if (f.Max.HasValue && num > f.Max.Value)
                            errors.Add(new ValidationError { Field = f.Key, Message = $"Value > {f.Max}." });
                    }

                    break;

                case FieldType.Date:
                    if (value.ValueKind != JsonValueKind.String || !DateOnly.TryParse(value.GetString(), out _))
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value should be date (yyyy-MM-dd)." });
                    break;

                case FieldType.Select:
                    if (value.ValueKind != JsonValueKind.String)
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value should be text." });
                    else if (f.Options is { Count: > 0 } &&
                             !f.Options.Contains(value.GetString()!, StringComparer.Ordinal))
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value is outside of the parameters" });
                    break;

                case FieldType.Checkbox:
                    if (value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False)
                        errors.Add(new ValidationError { Field = f.Key, Message = "Value should be logic value (true/false)." });
                    break;
            }
        }

        errors.AddRange(from key 
            in answers.Keys 
            where !fieldsByKey.ContainsKey(key) 
            select new ValidationError { Field = key, Message = "Field is unknown" });

        return errors;
    }
    
    private static bool TryGetDouble(JsonElement el, out double val)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Number:
                return el.TryGetDouble(out val);
            case JsonValueKind.String:
                return double.TryParse(el.GetString(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out val);
            default:
                val = 0;
                return false;
        }
    }
}