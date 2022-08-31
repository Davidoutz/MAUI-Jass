using JassWeb.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace MauiJass.Services
{
    public class SignalRService
    {
        public readonly HubConnection HubConnection;
        //public static readonly string HUB_URL = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5192" : "http://localhost:5192";
        public static readonly string HUB_URL = "https://signalrmauiserver.azurewebsites.net";
        public SignalRService()
        {
            HubConnection = new HubConnectionBuilder().WithUrl($"{HUB_URL}/gamehub").Build();
        }

        public async Task StartAsync()
        {
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await HubConnection.StartAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public void Unsubscribe(string method)
        {
            HubConnection.Remove(method);
        }
        public async Task ConnectAsync(IPlayer player)
        {

            try
            {
                await HubConnection.SendAsync("Connect", player);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task DisconnectAsync(IPlayer player)
        {
            await HubConnection.SendAsync("Disconnect", player);
        }
        public async Task SendMessageAsync(IPlayer player, string message)
        {
            await HubConnection.SendAsync("SendMessage", player.Name, message);
        }
        public async Task SetAtout(Guid gameId, SUIT suit)
        {
            await HubConnection.SendAsync("SetAtout", gameId, suit);
        }
        public async Task Chibre(Guid gameId, Player player)
        {
            await HubConnection.SendAsync("Chibre", gameId, player);
        }
        public async Task PlayCard(Guid gameId, Player player, Card card)
        {
            await HubConnection.SendAsync("PlayCard", gameId, player, card);
        }
        public async Task JoinGame(string playerName, Guid gameId, int teamId, int seat)
        {
            await HubConnection.SendAsync("JoinGame", playerName, gameId, teamId, seat);
        }
        public async Task CreateGame(string playerName)
        {
            await HubConnection.SendAsync("CreateGame", playerName);
        }
        public async Task LeaveGame(Guid gameId, Player player)
        {
            await HubConnection.SendAsync("LeaveGame", gameId, player);
        }
        public async Task GetGames()
        {
            await HubConnection.SendAsync("SendSelfUpdateGames");
        }
        public async Task ChecKNickname(string nickname)
        {
            await HubConnection.SendAsync("CheckNickname", nickname);
        }


        public async Task DisconnectAsync()
        {
            if (HubConnection.State != HubConnectionState.Disconnected)
            {
                try
                {
                    await HubConnection.StopAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}

