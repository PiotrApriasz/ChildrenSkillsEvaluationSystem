using ChildrenEvaluationSystem.Features.Groups;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace ChildrenEvaluationSystem.Web.Features.Home;

public partial class Home
{
    [Inject] public IMediator Mediator { get; set; } = null!;

    private int _groupsCount;
    
    private void NavigateToGroups() => Navigation.NavigateTo("/groups");
    private void NavigateToChildren() => Navigation.NavigateTo("/children");
    private void NavigateToDiagnoses() => Navigation.NavigateTo("/diagnoses");
    private void NavigateToQuickActionChildAdd() => Navigation.NavigateTo("/children/add");
    private void NavigateToQuickActionNewDiagnosis() => Navigation.NavigateTo("/diagnoses/new");

    protected override async Task OnInitializedAsync()
    {
        _groupsCount = await Mediator.Send(new CountUserGroupsQuery());
    }
}