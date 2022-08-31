using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using JassWeb.Shared;
using MauiJass.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace MauiJass.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [RelayCommand]
        async void LoginAsync()
        {
            if (string.IsNullOrEmpty(Player.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", "Le pseudo ne peut pas être vide...", "OK");
            }
            else
            {
                await signalRService.ChecKNickname(Player.Name);
            }
        }
        public IPlayer Player { get; set; }

        private readonly SignalRService signalRService;

       
        public async Task Init()
        {

            await signalRService.DisconnectAsync();
            await signalRService.StartAsync();
            signalRService.HubConnection.On("MustSwitchNickname", () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Ce pseudo est déjà en jeu. Veuillez en choisir un autre.", "OK");
                });
            });
            signalRService.HubConnection.On("OpenJassPage", () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync(
                        nameof(JassPage),
                        true,
                        new Dictionary<string, object>
                        {
                            { "player", Player }
                        });
                });
            });
        }
        public void Cleanup()
        {
            signalRService.Unsubscribe("MustSwitchNickname");
            signalRService.Unsubscribe("OpenJassPage");
        }
        public MainViewModel(SignalRService signalRService, IPlayer player)
        {
            Random random = new Random();
            string rdmName = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int rdm = random.Next(0, 26);
                rdmName += (char)('a' + rdm);
            }
            this.signalRService = signalRService;
            Player = player;
            Player.Name = rdmName;
        }
    }
}

