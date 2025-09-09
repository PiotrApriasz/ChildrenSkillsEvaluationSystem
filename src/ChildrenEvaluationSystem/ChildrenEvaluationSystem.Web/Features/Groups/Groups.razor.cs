using ChildrenEvaluationSystem.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ChildrenEvaluationSystem.Features.Groups;
using ChildrenEvaluationSystem.Web.Features.Groups.Components;

namespace ChildrenEvaluationSystem.Web.Features.Groups;

public partial class Groups : ComponentBase
{
    [Inject] public IMediator Mediator { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager Navigation { get; set; } = null!;

    private readonly List<GroupListItem> _groups = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadGroups();
    }

    private async Task LoadGroups()
    {
        _loading = true;
        var data = await Mediator.Send(new ListUserGroupsQuery());
        _groups.Clear();
        _groups.AddRange(data);
        _loading = false;
    }

    private async Task ShowAddGroupDialog()
    {
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseOnEscapeKey = true,
            BackdropClick = true,
            BackgroundClass = "glass-dialog-backdrop"
        };

        var dialog = await DialogService.ShowAsync<AddGroupDialog>(
            title: string.Empty,
            options: options
        );
        
        //TODO implement loading on card when new group is being created

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            _loading = true;
            var groupName = (string)result.Data!;
            var createGroupCommand = new CreateGroupCommand(groupName);
            var newGroupId = await Mediator.Send(createGroupCommand);

            await LoadGroups();
        }
    }

    private void NavigateToGroup(string id)
    {
        Navigation.NavigateTo($"/groups/{id}");
    }
}