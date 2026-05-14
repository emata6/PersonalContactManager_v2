using Microsoft.AspNetCore.SignalR;

namespace PersonalContactManager.Api.Hubs;

public sealed class ContactsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "contacts");
        await base.OnConnectedAsync();
    }
}
