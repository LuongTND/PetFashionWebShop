using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PetFashionWebShop.Services.Hubs
{
	public class PaymentHubServer : Hub
	{
		public async Task JoinRoom()
		{
			if (!Context.User.Identity.IsAuthenticated) return;
			await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.FindFirstValue("Id"));
		}
	}
}
