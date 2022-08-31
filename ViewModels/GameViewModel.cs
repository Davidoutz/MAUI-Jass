using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JassWeb.Shared;
using MauiJass.Helpers;
using MauiJass.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MauiJass.ViewModels
{
    [QueryProperty(nameof(Player), "player")]
    [QueryProperty(nameof(Game), "game")]
    [QueryProperty(nameof(TeamId), "teamId")]
    [QueryProperty(nameof(Seat), "seat")]
    public partial class GameViewModel : ObservableObject
    {
        private readonly SignalRService signalRService;
        private List<Card> playableCards = new List<Card>();
        [ObservableProperty]
        Game game;
        [ObservableProperty]
        Player player;
        [ObservableProperty]
        int teamId;
        [ObservableProperty]
        int seat;

        [ObservableProperty]
        Player rightPlayer = new Player() { Name = " " };
        [ObservableProperty]
        Player leftPlayer = new Player() { Name = " " };
        [ObservableProperty]
        Player partnerPlayer = new Player() { Name = " " };

        int leftSeatIndex;
        int rightSeatIndex;
        int partnerSeatIndex;
        int playerSeatIndex;

        [ObservableProperty]
        bool playerShouldPlay;
        [ObservableProperty]
        bool partnerShouldPlay;
        [ObservableProperty]
        bool leftShouldPlay;
        [ObservableProperty]
        bool rightShouldPlay;


        [ObservableProperty]
        SUIT currentSuit;
        [ObservableProperty]
        string atoutGlyph;
        [ObservableProperty]
        Color atoutGlyphColor;

        [ObservableProperty]
        ObservableCollection<ChatMessage> chatMessages;

        [ObservableProperty]
        bool canPickAtout = false;
        [ObservableProperty]
        bool waitForPartner = false;
        [ObservableProperty]
        bool canChibre = false;
        [ObservableProperty]
        bool shouldChoseSuit;
        [ObservableProperty]
        bool notReady;
        [ObservableProperty]
        bool canPlay = false;

        [ObservableProperty]
        bool didWin = false;
        [ObservableProperty]
        bool didLose = false;
        [ObservableProperty]
        int teamOneScore;
        [ObservableProperty]
        int teamTwoScore;
        [ObservableProperty]
        string endGameText = "";
        [ObservableProperty]
        Color endGameColor = Colors.Transparent;
        [ObservableProperty]
        Color endGameBGColor = Colors.Transparent;

        [ObservableProperty]
        string teamOneScoreHundred = "";
        [ObservableProperty]
        string teamTwoScoreHundred = "";
        [ObservableProperty]
        string teamOneScoreFifty = "";
        [ObservableProperty]
        string teamTwoScoreFifty = "";
        [ObservableProperty]
        string teamOneScoreTwenty = "";
        [ObservableProperty]
        string teamTwoScoreTwenty = "";
        [ObservableProperty]
        string teamOneScoreUnits = "";
        [ObservableProperty]
        string teamTwoScoreUnits = "";

        public GameViewModel(SignalRService signalRService)
        {
            this.signalRService = signalRService;
        }

        [RelayCommand]
        async void PickAtout(string suit)
        {
            switch (suit)
            {
                case "heart":
                    await signalRService.SetAtout(Game.Id, SUIT.HEART);
                    CanChibre = CanPickAtout = ShouldChoseSuit = false;
                    break;
                case "spade":
                    await signalRService.SetAtout(Game.Id, SUIT.SPADE);
                    CanChibre = CanPickAtout = ShouldChoseSuit = false;
                    break;
                case "diamond":
                    await signalRService.SetAtout(Game.Id, SUIT.DIAMOND);
                    CanChibre = CanPickAtout = ShouldChoseSuit = false;
                    break;
                case "club":
                    await signalRService.SetAtout(Game.Id, SUIT.CLUB);
                    CanChibre = CanPickAtout = ShouldChoseSuit = false;
                    break;
                case "chibre":
                    WaitForPartner = true;
                    CanChibre = CanPickAtout = false;
                    await signalRService.Chibre(Game.Id, Player);
                    break;
                default:
                    throw new Exception("Bad parameter for PickAtoutCommand");
            }
        }
        bool hasProblem;
        [RelayCommand]
        async void Chibre()
        {
            WaitForPartner = true;
            CanChibre = CanPickAtout = false;
            await signalRService.Chibre(Game.Id, Player);
        }


        private (int, int, int, int) GetScoresHandwritten(int score)
        {
            var h = 0;
            var f = 0;
            var t = 0;
            var u = 0;

            while (score > 0)
            {
                switch (score)
                {
                    case >= 100:
                        h++;
                        score -= 100;
                        break;
                    case >= 50:
                        score -= 50;
                        f++;
                        break;
                    case >= 20:
                        score -= 20;
                        t++;
                        break;
                    default:
                        u = score;
                        score = 0;
                        break;
                }
            }
            return (h, f, t, u);
        }

        [RelayCommand]
        public async void PlayCard(Card card)
        {
            Console.WriteLine($"Card played : {card.Value.ToString()} {card.Suit.ToString()}");
            if (!NotReady && playableCards?.Where(c => c.Value.Equals(card.Value) && c.Suit.Equals(card.Suit)).FirstOrDefault() != null)
            {
                await signalRService.PlayCard(Game.Id, Player, card);
                MessagingCenter.Send(card.Value.ToString() + "_" + card.Suit.ToString(), "PlayCard");
                playableCards = null;
                CanPlay = false;
            }
        }

        public async Task AppearingAsync()
        {
            signalRService.HubConnection.On<Guid>("ReceiveGameId", (id) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Game.Id = id;
                });
            });
            signalRService.HubConnection.On<string>("Problem", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", message, "OK");
                    hasProblem = true;
                    await Shell.Current.GoToAsync("..");
                });
            });
            signalRService.HubConnection.On<string>("AddChatMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ChatMessages.Add(new() { Text = message });
                    MessagingCenter.Send((ChatMessages.Count - 1).ToString(), "ScrollToBottom");
                });
            });
            signalRService.HubConnection.On<string, string>("AddAnnounceMessage", (message, color) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //ChatMessages.Add(new() { Text = message, Color = Color.FromHex(color) });
                    ChatMessages.Add(new() { Text = message, Color = Color.FromArgb(color) });
                    MessagingCenter.Send((ChatMessages.Count - 1).ToString(), "ScrollToBottom");
                });
            });
            signalRService.HubConnection.On<string>("AtoutPicked", (atout) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var suit = JsonConvert.DeserializeObject<SUIT>(atout);
                    Color c = Colors.Black;
                    CurrentSuit = suit;
                    var s = string.Empty;
                    switch (suit)
                    {
                        case SUIT.HEART:
                            AtoutGlyph = MDIconsHelper.CardsPlayingHeart;
                            AtoutGlyphColor = Colors.Red;
                            c = Colors.Red;
                            s = "♥";
                            break;
                        case SUIT.SPADE:
                            AtoutGlyph = MDIconsHelper.CardsPlayingSpade;
                            AtoutGlyphColor = Colors.Black;
                            s = "♠";
                            break;
                        case SUIT.DIAMOND:
                            AtoutGlyph = MDIconsHelper.CardsPlayingDiamond;
                            AtoutGlyphColor = Colors.Red;
                            c = Colors.Red;
                            s = "♦";
                            break;
                        case SUIT.CLUB:
                            AtoutGlyph = MDIconsHelper.CardsPlayingClub;
                            AtoutGlyphColor = Colors.Black;
                            s = "♣";
                            break;
                        default:
                            AtoutGlyph = string.Empty;
                            AtoutGlyphColor = Colors.Transparent;
                            break;
                    }

                    ChatMessages.Add(new() { Text = $"Atout choisi:" });
                    ChatMessages.Add(new() { Text = $"{s}", Color = c });
                    ShouldChoseSuit = CanPickAtout = CanChibre = WaitForPartner = false;
                });
            });
            signalRService.HubConnection.On<string, int>("CardPlayed", (card, seat) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var c = JsonConvert.DeserializeObject<Card>(card);
                    var s = seat;
                    if (s != playerSeatIndex)
                    {
                        if (s == leftSeatIndex)
                            MessagingCenter.Send($"left", "SetDirection");
                        if (s == rightSeatIndex)
                            MessagingCenter.Send($"right", "SetDirection");
                        if (s == partnerSeatIndex)
                            MessagingCenter.Send($"partner", "SetDirection");
                        MessagingCenter.Send($"{c.Value.ToString().ToLower()}{c.Suit.ToString().ToLower()[0]}", "AnimateCard");
                    }
                });
            });
            signalRService.HubConnection.On("Chibre", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShouldChoseSuit = true;
                    CanPickAtout = true;
                });
            });
            signalRService.HubConnection.On("CleanTable", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessagingCenter.Send("hi", "CleanTable");
                });
            });
            signalRService.HubConnection.On<int>("HighlightPlayer", (seat) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PlayerShouldPlay = LeftShouldPlay = RightShouldPlay = PartnerShouldPlay = false;
                    if (playerSeatIndex == seat)
                        PlayerShouldPlay = true;
                    else if (leftSeatIndex == seat)
                        LeftShouldPlay = true;
                    else if (rightSeatIndex == seat)
                        RightShouldPlay = true;
                    else if (partnerSeatIndex == seat)
                        PartnerShouldPlay = true;
                });
            });
            signalRService.HubConnection.On<string, string>("JoinExistingGame", (cards, announces) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var playerCards = JsonConvert.DeserializeObject<ObservableCollection<Card>>(cards);
                    var playerAnnounces = JsonConvert.DeserializeObject<List<Announce>>(announces);
                    try
                    {
                        Player.Cards = new ObservableCollection<Card>(playerCards);
                        Player.Announces = new List<Announce>(playerAnnounces);
                    }
                    catch { }
                    MessagingCenter.Send(cards, "RejoinGame");
                });
            });
            signalRService.HubConnection.On<string>("NextTurn", (cards) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    playableCards = JsonConvert.DeserializeObject<List<Card>>(cards);
                    CanPlay = true;
                });
            });
            signalRService.HubConnection.On<string>("RedrawHand", (hand) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var h = JsonConvert.DeserializeObject<Hand>(hand);
                    var seat = h.NextSeat;
                    var cardsPlayed = 0;
                    if (h.Card_1 is not null)
                        cardsPlayed++;
                    if (h.Card_2 is not null)
                        cardsPlayed++;
                    if (h.Card_3 is not null)
                        cardsPlayed++;
                    if (seat == leftSeatIndex)
                    {
                        switch (cardsPlayed)
                        {
                            case 1:
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 2:
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 3:
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_3.Value.ToString()}{h.Card_3.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            default:
                                break;
                        }
                    }
                    else if (seat == rightSeatIndex)
                    {
                        switch (cardsPlayed)
                        {
                            case 1:
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 2:
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 3:
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_3.Value.ToString()}{h.Card_3.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            default:
                                break;
                        }
                    }
                    else if (seat == partnerSeatIndex)
                    {
                        switch (cardsPlayed)
                        {
                            case 1:
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 2:
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 3:
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"player", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_3.Value.ToString()}{h.Card_3.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            default:
                                break;
                        }
                    }
                    else if (seat == playerSeatIndex)
                    {
                        switch (cardsPlayed)
                        {
                            case 1:
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 2:
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            case 3:
                                MessagingCenter.Send($"right", "SetDirection");
                                MessagingCenter.Send($"{h.Card_1.Value.ToString()}{h.Card_1.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"partner", "SetDirection");
                                MessagingCenter.Send($"{h.Card_2.Value.ToString()}{h.Card_2.Suit.ToString()[0]}", "RedrawCard");
                                MessagingCenter.Send($"left", "SetDirection");
                                MessagingCenter.Send($"{h.Card_3.Value.ToString()}{h.Card_3.Suit.ToString()[0]}", "RedrawCard");
                                break;
                            default:
                                break;
                        }
                    }
                });
            });
            signalRService.HubConnection.On<string>("AddPlayer", (players) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var p = JsonConvert.DeserializeObject<Dictionary<int, Player>>(players);
                    Game.Players = p;
                    foreach (var item in p.Values)
                    {
                        switch (playerSeatIndex)
                        {
                            case 0:
                                switch (item.Number)
                                {
                                    case 1:
                                        RightPlayer = item;
                                        break;
                                    case 2:
                                        PartnerPlayer = item;
                                        break;
                                    case 3:
                                        LeftPlayer = item;
                                        break;
                                }
                                break;
                            case 1:
                                switch (item.Number)
                                {
                                    case 0:
                                        LeftPlayer = item;
                                        break;
                                    case 2:
                                        RightPlayer = item;
                                        break;
                                    case 3:
                                        PartnerPlayer = item;
                                        break;
                                }
                                break;
                            case 2:
                                switch (item.Number)
                                {
                                    case 0:
                                        PartnerPlayer = item;
                                        break;
                                    case 1:
                                        LeftPlayer = item;
                                        break;
                                    case 3:
                                        RightPlayer = item;
                                        break;
                                }
                                break;
                            case 3:
                                switch (item.Number)
                                {
                                    case 0:
                                        RightPlayer = item;
                                        break;
                                    case 1:
                                        PartnerPlayer = item;
                                        break;
                                    case 2:
                                        LeftPlayer = item;
                                        break;
                                }
                                break;
                        }
                    }
                });
            });

            signalRService.HubConnection.On<string>("RemovePlayerByName", (name) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NotReady = true;
                    var seat = Game.Players.FirstOrDefault(p => p.Value.Name.Equals(name)).Key;
                    if (Game.Players.Keys.Contains(seat))
                        Game.Players.Remove(seat);
                    switch (playerSeatIndex)
                    {
                        case 0:
                            switch (seat)
                            {
                                case 1:
                                    RightPlayer = null;
                                    break;
                                case 2:
                                    PartnerPlayer = null;
                                    break;
                                case 3:
                                    LeftPlayer = null;
                                    break;
                            }
                            break;
                        case 1:
                            switch (seat)
                            {
                                case 0:
                                    LeftPlayer = null;
                                    break;
                                case 2:
                                    RightPlayer = null;
                                    break;
                                case 3:
                                    PartnerPlayer = null;
                                    break;
                            }
                            break;
                        case 2:
                            switch (seat)
                            {
                                case 0:
                                    PartnerPlayer = null;
                                    break;
                                case 1:
                                    LeftPlayer = null;
                                    break;
                                case 3:
                                    RightPlayer = null;
                                    break;
                            }
                            break;
                        case 3:
                            switch (seat)
                            {
                                case 0:
                                    RightPlayer = null;
                                    break;
                                case 1:
                                    PartnerPlayer = null;
                                    break;
                                case 2:
                                    LeftPlayer = null;
                                    break;
                            }
                            break;
                    }
                });
            });
            signalRService.HubConnection.On<int>("RemovePlayer", (seat) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NotReady = true;
                    //if (playerSeatIndex == seat)
                    //    return;
                    if (Game.Players.Keys.Contains(seat))
                        Game.Players.Remove(seat);
                    switch (playerSeatIndex)
                    {
                        case 0:
                            switch (seat)
                            {
                                case 1:
                                    RightPlayer = null;
                                    break;
                                case 2:
                                    PartnerPlayer = null;
                                    break;
                                case 3:
                                    LeftPlayer = null;
                                    break;
                            }
                            break;
                        case 1:
                            switch (seat)
                            {
                                case 0:
                                    LeftPlayer = null;
                                    break;
                                case 2:
                                    RightPlayer = null;
                                    break;
                                case 3:
                                    PartnerPlayer = null;
                                    break;
                            }
                            break;
                        case 2:
                            switch (seat)
                            {
                                case 0:
                                    PartnerPlayer = null;
                                    break;
                                case 1:
                                    LeftPlayer = null;
                                    break;
                                case 3:
                                    RightPlayer = null;
                                    break;
                            }
                            break;
                        case 3:
                            switch (seat)
                            {
                                case 0:
                                    RightPlayer = null;
                                    break;
                                case 1:
                                    PartnerPlayer = null;
                                    break;
                                case 2:
                                    LeftPlayer = null;
                                    break;
                            }
                            break;
                    }
                });
            });
            signalRService.HubConnection.On<string, string>("PrepareGame", (cards, announces) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NotReady = false;
                    //ChatMessages.Add("Appareil infos:");
                    //ChatMessages.Add($"Height: {DeviceDisplay.MainDisplayInfo.Height}");
                    //ChatMessages.Add($"Width: {DeviceDisplay.MainDisplayInfo.Width}");
                    //ChatMessages.Add($"Density: {DeviceDisplay.MainDisplayInfo.Density}");
                    //ChatMessages.Add("La partie commence");
                    var playerCards = JsonConvert.DeserializeObject<ObservableCollection<Card>>(cards);
                    var playerAnnounces = JsonConvert.DeserializeObject<List<Announce>>(announces);
                    Player.Cards = playerCards;
                    Player.Announces = playerAnnounces;
                    if (Player.Announces.Count > 0)
                    {

                    }
                    MessagingCenter.Send("hi", "PrepareCards");
                });
            });
            signalRService.HubConnection.On("ResumeGame", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NotReady = false;
                });
            });
            signalRService.HubConnection.On("Start", () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShouldChoseSuit = true;
                    playableCards = Player.Cards.ToList();
                    CanChibre = true;
                    CanPickAtout = true;
                    CanPlay = true;
                    WaitForPartner = false;
                });
            });
            signalRService.HubConnection.On<string>("UpdateScores", (scores) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var s = JsonConvert.DeserializeObject<Dictionary<int, int>>(scores);
                    if (s != null && s.Count == 2)
                    {
                        TeamOneScore = s[1];
                        (var a, var b, var c, var d) = GetScoresHandwritten(s[1]);
                        TeamOneScoreHundred = new String('|', a);
                        if (b % 2 == 0)
                        {
                            TeamOneScoreFifty = new String('X', b / 2);
                        }
                        else
                        {
                            var tmp = new String('X', ((b - 1) / 2));
                            TeamOneScoreFifty = tmp + "/";
                        }
                        TeamOneScoreTwenty = new String('|', c);
                        TeamOneScoreUnits = d.ToString();
                        TeamTwoScore = s[2];
                        (a, b, c, d) = GetScoresHandwritten(s[2]);
                        TeamTwoScoreHundred = new String('|', a);
                        if (b % 2 == 0)
                        {
                            TeamTwoScoreFifty = new String('X', b / 2);
                        }
                        else
                        {
                            var tmp = new String('X', ((b - 1) / 2));
                            TeamTwoScoreFifty = tmp + "/";
                        }
                        TeamTwoScoreTwenty = new String('|', c);
                        TeamTwoScoreUnits = d.ToString();
                        TeamTwoScore = s[2];
                    }
                });
            });
            signalRService.HubConnection.On<int>("Winner", (winner) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    playableCards = null;
                    if (Player.Team == winner)
                    {
                        await Task.Delay(100);
                        DidWin = true;
                        await Task.Delay(100);
                        EndGameText = "WINNER";
                        EndGameColor = Colors.Green;
                        EndGameBGColor = Colors.Black.WithAlpha(0.9f);
                        ChatMessages.Add(new() { Text = $"Vous avez gagné :) !" });
                        MessagingCenter.Send((ChatMessages.Count - 1).ToString(), "ScrollToBottom");
                        MessagingCenter.Send("hi", "SetWinner");
                    }
                    else
                    {
                        await Task.Delay(100);
                        DidLose = true;
                        await Task.Delay(100);
                        EndGameText = "LOSER";
                        EndGameColor = Colors.Red;
                        EndGameBGColor = Colors.Black.WithAlpha(0.9f);
                        ChatMessages.Add(new() { Text = $"Vous avez perdu :( !" });
                        MessagingCenter.Send((ChatMessages.Count - 1).ToString(), "ScrollToBottom");
                        MessagingCenter.Send("hi", "SetLoser");
                    }
                });
            });



            NotReady = true;
            ChatMessages = new ObservableCollection<ChatMessage>() { new() { Text = $"Bienvenue {Player.Name} !" } };

            if (Game is null)
            {
                Game = new Game();
                playerSeatIndex = 0;
                rightSeatIndex = 1;
                partnerSeatIndex = 2;
                leftSeatIndex = 3;
                await signalRService.CreateGame(Player.Name);
            }
            else
            {
                switch (Seat)
                {
                    case 0:
                        playerSeatIndex = 0;
                        rightSeatIndex = 1;
                        partnerSeatIndex = 2;
                        leftSeatIndex = 3;
                        break;
                    case 1:
                        playerSeatIndex = 1;
                        rightSeatIndex = 2;
                        partnerSeatIndex = 3;
                        leftSeatIndex = 0;
                        break;
                    case 2:
                        playerSeatIndex = 2;
                        rightSeatIndex = 3;
                        partnerSeatIndex = 0;
                        leftSeatIndex = 1;
                        break;
                    case 3:
                        playerSeatIndex = 3;
                        rightSeatIndex = 0;
                        partnerSeatIndex = 1;
                        leftSeatIndex = 2;
                        break;
                    default:
                        break;
                }
                await signalRService.JoinGame(Player.Name, Game.Id, TeamId, Seat);
            }
        }

        [RelayCommand]
        async void Quit()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task DisconnectAsync()
        {
            if (!hasProblem)
                await signalRService.LeaveGame(Game.Id, Player);
            signalRService.Unsubscribe("AddChatMessage");
            signalRService.Unsubscribe("AddAnnounceMessage");
            signalRService.Unsubscribe("AddPlayer");
            signalRService.Unsubscribe("AtoutPicked");
            signalRService.Unsubscribe("Chibre");
            signalRService.Unsubscribe("CardPlayed");
            signalRService.Unsubscribe("CleanTable");
            signalRService.Unsubscribe("HighlightPlayer");
            signalRService.Unsubscribe("JoinExistingGame");
            signalRService.Unsubscribe("NextTurn");
            signalRService.Unsubscribe("PrepareGame");
            signalRService.Unsubscribe("ReceiveGameId");
            signalRService.Unsubscribe("RemovePlayer");
            signalRService.Unsubscribe("RemovePlayerByName");
            signalRService.Unsubscribe("ResumeGame");
            signalRService.Unsubscribe("RedrawHand");
            signalRService.Unsubscribe("Start");
            signalRService.Unsubscribe("UpdateScores");
            signalRService.Unsubscribe("Winner");
            signalRService.Unsubscribe("Problem");
            signalRService.Unsubscribe("ShowAnnounces");
        }
    }


    public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty]
        string text;

        [ObservableProperty]
        Color color = Colors.Black;
    }

}

