using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiJass.Converters;
using MauiJass.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using JassWeb.Shared;

namespace MauiJass.ViewModels
{
    [QueryProperty(nameof(Player), "player")]
    public partial class JassViewModel : ObservableObject
    {
        public static int BIG_DENSITY_FACTOR = DeviceDisplay.MainDisplayInfo.Density == 3 ? 9 : (DeviceDisplay.MainDisplayInfo.Density == 2 ? 6 : (int)DeviceDisplay.MainDisplayInfo.Density * 4);
        public static int BIG_WIDTH = (int)DeviceDisplay.MainDisplayInfo.Width / BIG_DENSITY_FACTOR;
        public static int BIG_HEIGHT = (int)DeviceDisplay.MainDisplayInfo.Height / BIG_DENSITY_FACTOR;
        public JassViewModel(SignalRService signalRService)
        {
            this.signalRService = signalRService;
            TableHeight = BIG_HEIGHT;
        }
        [ObservableProperty]
        int tableHeight;

        [ObservableProperty]
        Player player;

        [ObservableProperty]
        int allPlayersCount;

        [ObservableProperty]
        ObservableCollection<Game> rooms;

        
        ObservableCollection<Game> allRooms;

        private readonly SignalRService signalRService;


        [RelayCommand]
        async void PickGame(SelectedRoomArguments args)
        {
            var team = args.Seat % 2 == 0 ? 1 : 2;
            Player.Team = team;
            Player.Number = args.Seat;
            await GoToPageAsync(Player, args.Game, team, args.Seat);
        }

        [RelayCommand]
        async void CreateGameAsync()
        {
            Player.Team = 1;
            Player.Number = 0;
            await GoToPageAsync(Player, null, 1, 0);
        }

        async Task GoToPageAsync(Player p, Game g, int teamId, int seat)
        {
            await Shell.Current.GoToAsync(nameof(GamePage), true, new Dictionary<string, object>
            {
                { "player", p },
                { "game", g },
                { "teamId", teamId },
                { "seat", seat }
            });
        }

        public async Task Connect()
        {
            signalRService.HubConnection.On<int>("UpdatePlayersCount", (number) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AllPlayersCount = number;
                    Debug.WriteLine($"Players count: {number}");
                    //await AppShell.Current.GoToAsync(nameof(SecondPage)); // not possible
                });
            });
            signalRService.HubConnection.On<string>("UpdateGames", (allGames) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var theRooms = JsonConvert.DeserializeObject<Dictionary<Guid, Game>>(allGames);
                    allRooms = new ObservableCollection<Game>(theRooms.Select(r => r.Value));
                    var filtered = allRooms.Where(r => !r.IsFinished && r.SeatOpen);
                    Rooms = new ObservableCollection<Game>(filtered);
                    Debug.WriteLine($"Games count: {Rooms.Count}");
                    //await AppShell.Current.GoToAsync(nameof(SecondPage)); // not possible
                });
            });
            signalRService.HubConnection.On<string>("AddGame", (game) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var room = JsonConvert.DeserializeObject<Game>(game);
                    allRooms.Insert(0, room);
                    Rooms.Insert(0, room);
                    Debug.WriteLine($"Game added - count: {Rooms.Count}");
                });
            });
            signalRService.HubConnection.On<string>("Problem", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", message, "OK");
                });
            });
            signalRService.HubConnection.On<string>("UpdateGame", (game) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        var room = JsonConvert.DeserializeObject<Game>(game);
                        var allLocalRoom = allRooms.FirstOrDefault(r => r.Id.Equals(room.Id));
                        if (allLocalRoom != null)
                        {
                            allLocalRoom.Players = room.Players;
                            allLocalRoom.SeatOpen = room.SeatOpen;
                            allLocalRoom.HasStarted = room.HasStarted;
                            allLocalRoom.IsFinished = room.IsFinished;
                            var localRoom = Rooms.FirstOrDefault(r => r.Id.Equals(room.Id));


                            if (room.Players.Count() >= 4 || room.IsFinished)
                            {
                                allLocalRoom.SeatOpen = false;
                                if (localRoom != null)
                                {
                                    localRoom.SeatOpen = false;
                                    Rooms.Remove(localRoom);
                                }
                            }
                            else
                            {
                                var r = Rooms.FirstOrDefault(r => r.Id.Equals(room.Id));
                                if (r != null)
                                {
                                    localRoom.Players = room.Players;
                                    localRoom.SeatOpen = room.SeatOpen;
                                    localRoom.HasStarted = room.HasStarted;
                                    localRoom.IsFinished = room.IsFinished;
                                }
                                else
                                {
                                    var roomRef = allRooms.FirstOrDefault(r => r.Id.Equals(room.Id));
                                    Rooms.Insert(0, roomRef);
                                }
                            }
                        }
                        Debug.WriteLine($"Game updated");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                });
            });
            signalRService.HubConnection.On<Guid>("RemoveGame", (gameId) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        var ar = allRooms.First(r => r.Id.Equals(gameId));
                        allRooms.Remove(ar);
                        var r = Rooms.FirstOrDefault(r => r.Id.Equals(gameId));
                        Rooms.Remove(r);
                        Debug.WriteLine($"Games removed - count: {Rooms.Count}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            });
            signalRService.HubConnection.On<string>("RetrieveId", (id) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Player.ConnectionId = id;
                });
            });
            signalRService.HubConnection.On("RemoveId", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // SignalR = string.Empty;
                    //SecureStorage.Remove("signalR");
                });
            });
            await signalRService.ConnectAsync(Player);
            await signalRService.GetGames();
        }

        public void Disconnect()
        {
            signalRService.Unsubscribe("UpdatePlayersCount");
            signalRService.Unsubscribe("UpdateGames");
            signalRService.Unsubscribe("AddGame");
            signalRService.Unsubscribe("UpdateGame");
            signalRService.Unsubscribe("RemoveGame");
            signalRService.Unsubscribe("RetrieveId");
            signalRService.Unsubscribe("RemoveId");
            signalRService.Unsubscribe("Problem");
            //await signalRService.HubConnection.StopAsync();
        }

        
    }
}

