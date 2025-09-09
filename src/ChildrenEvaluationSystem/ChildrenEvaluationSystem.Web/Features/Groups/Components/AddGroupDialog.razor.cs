using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ChildrenEvaluationSystem.Web.Features.Groups.Components;

public partial class AddGroupDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = null!;
    private string _name = string.Empty;
    private bool _hasTried;

    private void Cancel() => Dialog.Cancel();

    private void Submit()
    {
        _hasTried = true;
        if (string.IsNullOrWhiteSpace(_name)) return;
        Dialog.Close(DialogResult.Ok(_name.Trim()));
    }
}