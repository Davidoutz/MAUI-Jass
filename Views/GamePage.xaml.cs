using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiJass.ViewModels;
using Newtonsoft.Json;
using JassWeb.Shared;

namespace MauiJass;

public partial class GamePage : ContentPage
{
    List<Image> leftImages = new List<Image>();
    List<Image> rightImages = new List<Image>();
    List<Image> partnerImages = new List<Image>();
    List<Image> playerImages = new List<Image>();
    string animationDirection;
    public static int CARDS_ZINDEX = 1;

    private readonly Random _random = new Random();
    public static int SMALL_DENSITY_FACTOR = DeviceDisplay.MainDisplayInfo.Density == 3 ? 18 : (DeviceDisplay.MainDisplayInfo.Density == 2 ? 12 : (int)DeviceDisplay.MainDisplayInfo.Density * 7);
    public static int SMALL_CARD_WIDTH = (int)DeviceDisplay.MainDisplayInfo.Width / SMALL_DENSITY_FACTOR;
    public static int SMALL_CARD_HEIGHT = (int)DeviceDisplay.MainDisplayInfo.Height / SMALL_DENSITY_FACTOR;
    public static int MEDIUM_DENSITY_FACTOR = DeviceDisplay.MainDisplayInfo.Density == 3 ? 15 : (DeviceDisplay.MainDisplayInfo.Density == 2 ? 10 : (int)DeviceDisplay.MainDisplayInfo.Density * 6);
    public static int MEDIUM_CARD_WIDTH = (int)DeviceDisplay.MainDisplayInfo.Width / MEDIUM_DENSITY_FACTOR;
    public static int MEDIUM_CARD_HEIGHT = (int)DeviceDisplay.MainDisplayInfo.Height / MEDIUM_DENSITY_FACTOR;
    public static int BIG_DENSITY_FACTOR = DeviceDisplay.MainDisplayInfo.Density == 3 ? 9 : (DeviceDisplay.MainDisplayInfo.Density == 2 ? 6 : (int)DeviceDisplay.MainDisplayInfo.Density*4);
    public static int BIG_CARD_WIDTH = (int)DeviceDisplay.MainDisplayInfo.Width / BIG_DENSITY_FACTOR;
    public static int BIG_CARD_HEIGHT = (int)DeviceDisplay.MainDisplayInfo.Height / BIG_DENSITY_FACTOR;
    public int RandomNumber(int min, int max)
    {
        return _random.Next(min, max);
    }
    public GamePage(GameViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override async void OnDisappearing()
    {
        await (BindingContext as GameViewModel).DisconnectAsync();

        //leftImages = rightImages = partnerImages = playerImages = new List<Image>();
        //mainTable.Clear();

        MessagingCenter.Unsubscribe<string>(this, "SetDirection");
        MessagingCenter.Unsubscribe<string>(this, "CleanTable");
        MessagingCenter.Unsubscribe<string>(this, "RedrawCard");
        MessagingCenter.Unsubscribe<string>(this, "AnimateCard");
        MessagingCenter.Unsubscribe<string>(this, "RejoinGame");
        MessagingCenter.Unsubscribe<string>(this, "PrepareCards");
        MessagingCenter.Unsubscribe<string>(this, "PlayCard");
        MessagingCenter.Unsubscribe<string>(this, "ScrollToBottom");
        MessagingCenter.Unsubscribe<string>(this, "SetWinner");
        MessagingCenter.Unsubscribe<string>(this, "SetLoser");
        base.OnDisappearing();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await (BindingContext as GameViewModel).AppearingAsync();

        MessagingCenter.Subscribe<string>(this, "PlayCard", async (classId) =>
        {
            var image = playerImages.FirstOrDefault(i => i.ClassId.Equals(classId));
            var index = playerImages.IndexOf(image);
            await image.TranslateTo(0, -(image.Height / 3), 200);
            image.Margin = new Thickness(0, (int)(image.Height / 3), 0, 0);
            image.ZIndex = CARDS_ZINDEX++;
            image.ClassId = String.Empty;
            image.HeightRequest = SMALL_CARD_HEIGHT;
            image.WidthRequest = SMALL_CARD_WIDTH;
            var send = image.TranslateTo(0, -(image.Height), 250);
            //var scale = image.ScaleTo(0.5, 250);
            var rotate = image.RotateTo(180, 250);
            await Task.Delay(250);
            image.HorizontalOptions = LayoutOptions.Center;
            image.HorizontalOptions = LayoutOptions.Center;
            image.GestureRecognizers.Clear();
            //image.ZIndex = playerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).Count();
        });
        MessagingCenter.Subscribe<string>(this, "ScrollToBottom", async (index) =>
        {
            await Task.Delay(100);
            chatList.ScrollTo(int.Parse(index), -1, ScrollToPosition.End, true);
        });
        MessagingCenter.Subscribe<string>(this, "SetWinner", (arg) =>
        {
            //winner.IsVisible = true;
        });
        MessagingCenter.Subscribe<string>(this, "SetLoser", (arg) =>
        {
            //loser.IsVisible = true;
        });

        MessagingCenter.Subscribe<string>(this, "PrepareCards", async (arg) =>
        {
            leftImages = new List<Image>();
            rightImages = new List<Image>();
            partnerImages = new List<Image>();
            playerImages = new List<Image>();
            mainTable.Clear();
            var smallCardHeight = SMALL_CARD_HEIGHT;
            var smallCardWidth = SMALL_CARD_WIDTH;
            var bigCardHeight = (int)(BIG_CARD_HEIGHT * .93);
            var bigCardWidth = (int)(BIG_CARD_WIDTH * .93);

            List<CardAttributes> leftCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth), 2d/3*smallCardWidth, 0, 0),
                    Rotation = 130
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/20), 1d/2*smallCardWidth, 0, 0),
                    Rotation = 120
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/10), 1d/3*smallCardWidth, 0, 0),
                    Rotation = 110
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 1d/6*smallCardWidth, 0, 0),
                    Rotation = 100
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 0, 0, 0),
                    Rotation = 90
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 0, 0, 1d/6*smallCardWidth),
                    Rotation = 80
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/10), 0, 0, 1d/3*smallCardWidth),
                    Rotation = 70
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/20), 0, 0, 1d/2*smallCardWidth),
                    Rotation = 60
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(-(2d/3*smallCardWidth), 0, 0, 2d/3*smallCardWidth),
                    Rotation = 50
                }
            };
            List<CardAttributes> rightCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth), 2d/3*smallCardWidth),
                    Rotation = 130
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/20), 1d/2*smallCardWidth),
                    Rotation = 120
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/10), 1d/3*smallCardWidth),
                    Rotation = 110
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/5), 1d/6*smallCardWidth),
                    Rotation = 100
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/5), 0),
                    Rotation = 90
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/6*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/5), 0),
                    Rotation = 80
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/3*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/10), 0),
                    Rotation = 70
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/2*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/20), 0),
                    Rotation = 60
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 2d/3*smallCardWidth, -(2d/3*smallCardWidth), 0),
                    Rotation = 50
                }
            };
            List<CardAttributes> partnerCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight), smallCardWidth, 0),
                    Rotation = 40
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/20), 3d/4*smallCardWidth, 0),
                    Rotation = 30
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/10), 1d/2*smallCardWidth, 0),
                    Rotation = 20
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/5), 1d/4*smallCardWidth, 0),
                    Rotation = 10
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/5), 0, 0),
                    Rotation = 0
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(1d/4*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/5), 0, 0),
                    Rotation = -10
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(1d/2*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/10), 0, 0),
                    Rotation = -20
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(3d/4*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/20), 0, 0),
                    Rotation = -30
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(smallCardWidth, -(2d/3*smallCardHeight), 0, 0),
                    Rotation = -40
                }
            };
            List<CardAttributes> playerCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 190, 3),
                    Rotation = -84
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 170, 70),
                    Rotation = -65
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 120, 120),
                    Rotation = -41
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, 50, 150),
                    Rotation = -23
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(30, 0, 0, 160),
                    Rotation = -5
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(100, 0, 0, 140),
                    Rotation = 15
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(150, 0, 0, 100),
                    Rotation = 35
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(180, 0, 0, 55),
                    Rotation = 60
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(190, 0, 0, 0),
                    Rotation = 84
                }
            };

            foreach (var card in leftCardsAttributes)
            {
                Image image = new Image
                {
                    //BackgroundColor = Colors.Blue,
                    Source = "card_back.png",
                    HorizontalOptions = card.HorizontalOptions,
                    VerticalOptions = card.VerticalOptions,
                    WidthRequest = card.WidthRequest,
                    HeightRequest = card.HeightRequest,
                    Margin = card.Margin,
                    ClassId = "something"
                    //Rotation = card.Rotation
                };
                image.SetValue(Grid.ColumnSpanProperty, 2);
                image.SetValue(Grid.RowSpanProperty, 2);
                leftImages.Add(image);
                mainTable.Add(image, 0, 1);
                await image.RotateTo(card.Rotation, 20);
                //leftImage.SetBinding(Image.SourceProperty, new Binding($"Player.Cards[{i}].Png"));
            }
            foreach (var card in rightCardsAttributes)
            {
                Image image = new Image
                {
                    //BackgroundColor = Colors.Blue,
                    Source = "card_back.png",
                    HorizontalOptions = card.HorizontalOptions,
                    VerticalOptions = card.VerticalOptions,
                    WidthRequest = card.WidthRequest,
                    HeightRequest = card.HeightRequest,
                    Margin = card.Margin,
                    ClassId = "something"
                    //Rotation = card.Rotation
                };
                image.SetValue(Grid.ColumnSpanProperty, 2);
                image.SetValue(Grid.RowSpanProperty, 2);
                rightImages.Add(image);
                mainTable.Add(image, 0, 1);
                await image.RotateTo(card.Rotation, 20);
                //leftImage.SetBinding(Image.SourceProperty, new Binding($"Player.Cards[{i}].Png"));
            }
            foreach (var card in partnerCardsAttributes)
            {
                Image image = new Image
                {
                    Source = "card_back.png",
                    HorizontalOptions = card.HorizontalOptions,
                    VerticalOptions = card.VerticalOptions,
                    WidthRequest = card.WidthRequest,
                    HeightRequest = card.HeightRequest,
                    Margin = card.Margin,
                    ClassId = "something"
                };
                image.SetValue(Grid.ColumnSpanProperty, 2);
                image.SetValue(Grid.RowSpanProperty, 2);
                partnerImages.Add(image);
                mainTable.Add(image, 0, 1);
                await image.RotateTo(card.Rotation, 20);
            }
            var i = 0;
            var zindex = 300;
            foreach (var card in playerCardsAttributes)
            {
                Image image = new Image
                {
                    Source = "card_back.png",
                    HorizontalOptions = card.HorizontalOptions,
                    VerticalOptions = card.VerticalOptions,
                    WidthRequest = card.WidthRequest,
                    HeightRequest = card.HeightRequest,
                    Margin = card.Margin,
                    ZIndex = zindex++
                    //ZIndex = 9
                    //Rotation = card.Rotation
                };
                image.SetValue(Grid.ColumnSpanProperty, 2);
                image.SetValue(Grid.RowSpanProperty, 2);
                image.SetValue(Grid.ColumnProperty, 3);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "PlayCardCommand");
                tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding($"Player.Cards[{i}]"));
                image.GestureRecognizers.Add(tapGestureRecognizer);
                image.SetBinding(Image.SourceProperty, new Binding($"Player.Cards[{i}].Png"));
                image.ClassId = (BindingContext as GameViewModel).Player.Cards[i].Value.ToString() + "_" + (BindingContext as GameViewModel).Player.Cards[i].Suit.ToString();
                playerImages.Add(image);
                mainTable.Add(image, 0, 3);
                await image.RotateTo(card.Rotation, 20);
                i++;
            }

        });


        MessagingCenter.Subscribe<string>(this, "RejoinGame", (arg) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IsBusy = true;
                    Console.WriteLine("Debugging RejoinGame");
                    var playerCards = JsonConvert.DeserializeObject<ObservableCollection<Card>>(arg);
                    var count = playerCards.Count();
                    leftImages = new List<Image>();
                    rightImages = new List<Image>();
                    partnerImages = new List<Image>();
                    playerImages = new List<Image>();
                    mainTable.Clear();
                    var smallCardHeight = SMALL_CARD_HEIGHT;
                    var smallCardWidth = SMALL_CARD_WIDTH;
                    var bigCardHeight = (int)(BIG_CARD_HEIGHT * .93);
                    var bigCardWidth = (int)(BIG_CARD_WIDTH * .93);
                    Console.WriteLine("RejoinGame>Configuration");
                    List<CardAttributes> leftCardsAttributes = new List<CardAttributes>()
                {
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth), 2d/3*smallCardWidth, 0, 0),
                        Rotation = 130
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/20), 1d/2*smallCardWidth, 0, 0),
                        Rotation = 120
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/10), 1d/3*smallCardWidth, 0, 0),
                        Rotation = 110
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 1d/6*smallCardWidth, 0, 0),
                        Rotation = 100
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 0, 0, 0),
                        Rotation = 90
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/5), 0, 0, 1d/6*smallCardWidth),
                        Rotation = 80
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/10), 0, 0, 1d/3*smallCardWidth),
                        Rotation = 70
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth - smallCardWidth/20), 0, 0, 1d/2*smallCardWidth),
                        Rotation = 60
                    },
                    new CardAttributes()
                    {
                        HeightRequest = smallCardHeight,
                        WidthRequest = smallCardWidth,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(-(2d/3*smallCardWidth), 0, 0, 2d/3*smallCardWidth),
                        Rotation = 50
                    }
                };
                    List<CardAttributes> rightCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth), 2d/3*smallCardWidth),
                    Rotation = 130
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/20), 1d/2*smallCardWidth),
                    Rotation = 120
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/10), 1d/3*smallCardWidth),
                    Rotation = 110
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/5), 1d/6*smallCardWidth),
                    Rotation = 100
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0,  -(2d/3*smallCardWidth - smallCardWidth/5), 0),
                    Rotation = 90
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/6*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/5), 0),
                    Rotation = 80
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/3*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/10), 0),
                    Rotation = 70
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 1d/2*smallCardWidth, -(2d/3*smallCardWidth - smallCardWidth/20), 0),
                    Rotation = 60
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 2d/3*smallCardWidth, -(2d/3*smallCardWidth), 0),
                    Rotation = 50
                }
            };
                    List<CardAttributes> partnerCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight), smallCardWidth, 0),
                    Rotation = 40
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/20), 3d/4*smallCardWidth, 0),
                    Rotation = 30
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/10), 1d/2*smallCardWidth, 0),
                    Rotation = 20
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/5), 1d/4*smallCardWidth, 0),
                    Rotation = 10
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, -(2d/3*smallCardHeight - smallCardHeight/5), 0, 0),
                    Rotation = 0
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(1d/4*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/5), 0, 0),
                    Rotation = -10
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(1d/2*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/10), 0, 0),
                    Rotation = -20
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(3d/4*smallCardWidth, -(2d/3*smallCardHeight - smallCardHeight/20), 0, 0),
                    Rotation = -30
                },
                new CardAttributes()
                {
                    HeightRequest = smallCardHeight,
                    WidthRequest = smallCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(smallCardWidth, -(2d/3*smallCardHeight), 0, 0),
                    Rotation = -40
                }
            };
                    List<CardAttributes> playerCardsAttributes = new List<CardAttributes>()
            {
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, bigCardWidth*1.44, bigCardWidth*0.02),
                    Rotation = -82
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, bigCardWidth*1.29, bigCardWidth*.53),
                    Rotation = -65
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, bigCardWidth*0.9, bigCardWidth*.9),
                    Rotation = -41
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 0, bigCardWidth*0.38, bigCardWidth*1.14),
                    Rotation = -23
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(bigCardWidth*0.23, 0, 0, bigCardWidth*1.21),
                    Rotation = -5
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(bigCardWidth*0.76, 0, 0, bigCardWidth*1.06),
                    Rotation = 15
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(bigCardWidth*1.14, 0, 0, bigCardWidth*.75),
                    Rotation = 35
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(bigCardWidth*1.37, 0, 0, bigCardWidth*.42),
                    Rotation = 60
                },
                new CardAttributes()
                {
                    HeightRequest = bigCardHeight,
                    WidthRequest = bigCardWidth,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(bigCardWidth*1.44, 0, 0, 0),
                    Rotation = 85
                }
            };
                    var start = 9 - count - (9 - count) / 2;

                    Console.WriteLine("RejoinGame>LEFT images");
                    for (int k = start; k < start + count; k++)
                    {
                        Console.WriteLine("left K is " + k);
                        Image image = new Image
                        {
                            Source = "card_back.png",
                            HorizontalOptions = leftCardsAttributes[k].HorizontalOptions,
                            VerticalOptions = leftCardsAttributes[k].VerticalOptions,
                            WidthRequest = leftCardsAttributes[k].WidthRequest,
                            HeightRequest = leftCardsAttributes[k].HeightRequest,
                            Margin = leftCardsAttributes[k].Margin,
                            ClassId = "something"
                        };
                        image.SetValue(Grid.ColumnSpanProperty, 2);
                        image.SetValue(Grid.RowSpanProperty, 2);
                        leftImages.Add(image);
                        mainTable.Add(image, 0, 1);
                        await image.RotateTo(leftCardsAttributes[k].Rotation, 20);
                    }
                    Console.WriteLine("RejoinGame>Right images");
                    for (int k = start; k < start + count; k++)
                    {
                        Console.WriteLine("right K is " + k);
                        Image image = new Image
                        {
                            //BackgroundColor = Colors.Blue,
                            Source = "card_back.png",
                            HorizontalOptions = rightCardsAttributes[k].HorizontalOptions,
                            VerticalOptions = rightCardsAttributes[k].VerticalOptions,
                            WidthRequest = rightCardsAttributes[k].WidthRequest,
                            HeightRequest = rightCardsAttributes[k].HeightRequest,
                            Margin = rightCardsAttributes[k].Margin,
                            ClassId = "something"
                            //Rotation = card.Rotation
                        };
                        image.SetValue(Grid.ColumnSpanProperty, 2);
                        image.SetValue(Grid.RowSpanProperty, 2);
                        rightImages.Add(image);
                        mainTable.Add(image, 0, 1);
                        await image.RotateTo(rightCardsAttributes[k].Rotation, 20);
                        //leftImage.SetBinding(Image.SourceProperty, new Binding($"Player.Cards[{i}].Png"));
                    }
                    Console.WriteLine("RejoinGame>PARTNER images");
                    for (int k = start; k < start + count; k++)
                    {
                        Console.WriteLine("partner K is " + k);
                        Image image = new Image
                        {
                            Source = "card_back.png",
                            HorizontalOptions = partnerCardsAttributes[k].HorizontalOptions,
                            VerticalOptions = partnerCardsAttributes[k].VerticalOptions,
                            WidthRequest = partnerCardsAttributes[k].WidthRequest,
                            HeightRequest = partnerCardsAttributes[k].HeightRequest,
                            Margin = partnerCardsAttributes[k].Margin,
                            ClassId = "something"
                        };
                        image.SetValue(Grid.ColumnSpanProperty, 2);
                        image.SetValue(Grid.RowSpanProperty, 2);
                        partnerImages.Add(image);
                        mainTable.Add(image, 0, 1);
                        await image.RotateTo(partnerCardsAttributes[k].Rotation, 20);
                    }
                    Console.WriteLine("RejoinGame>PLAYER images");
                    var zindex = 300;
                    for (int k = start; k < start + count; k++)
                    {
                        Console.WriteLine("player K is " + k);
                        Image image = new Image
                        {
                            Source = "card_back.png",
                            HorizontalOptions = playerCardsAttributes[k].HorizontalOptions,
                            VerticalOptions = playerCardsAttributes[k].VerticalOptions,
                            WidthRequest = playerCardsAttributes[k].WidthRequest,
                            HeightRequest = playerCardsAttributes[k].HeightRequest,
                            Margin = playerCardsAttributes[k].Margin,
                            ZIndex = zindex++
                        };
                        image.SetValue(Grid.ColumnSpanProperty, 2);
                        image.SetValue(Grid.RowSpanProperty, 2);
                        image.SetValue(Grid.ColumnProperty, 3);
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, "PlayCardCommand");
                        tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding($"Player.Cards[{k - start}]"));
                        image.GestureRecognizers.Add(tapGestureRecognizer);
                        image.SetBinding(Image.SourceProperty, new Binding($"Player.Cards[{k - start}].Png"));
                        image.ClassId = (BindingContext as GameViewModel).Player.Cards[k - start].Value.ToString() + "_" + (BindingContext as GameViewModel).Player.Cards[k - start].Suit.ToString();

                        playerImages.Add(image);
                        mainTable.Add(image, 0, 3);
                        await image.RotateTo(playerCardsAttributes[k].Rotation, 20);
                    }

                    IsBusy = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        });
        base.OnNavigatedTo(args);


        MessagingCenter.Subscribe<string>(this, "SetDirection", (arg) =>
        {
            animationDirection = arg;
        });
        MessagingCenter.Subscribe<string>(this, "RedrawCard", (card) =>
        {
            Image image = new Image
            {
                //BackgroundColor = Colors.Blue,
                Source = card.ToLower() + ".png",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = SMALL_CARD_HEIGHT,
                HeightRequest = SMALL_CARD_WIDTH,
                ZIndex = CARDS_ZINDEX++
                //Rotation = 90
            };
            switch (animationDirection)
            {
                case "left":
                    image.Rotation = 90;
                    image.Margin = new Thickness(0, 0, (int)(BIG_CARD_WIDTH / 3), 0);
                    break;
                case "right":
                    image.Rotation = 270;
                    image.Margin = new Thickness((int)(BIG_CARD_WIDTH / 3), 0, 0, 0);
                    break;
                case "partner":
                    image.Rotation = 0;
                    image.Margin = new Thickness(0, 0, 0, (int)(MEDIUM_CARD_HEIGHT / 3));
                    break;
                case "player":
                    image.Rotation = 180;
                    image.Margin = new Thickness(0, (int)(MEDIUM_CARD_HEIGHT / 3), 0, 0);
                    break;
                default:
                    break;
            }
            image.SetValue(Grid.ColumnSpanProperty, 2);
            image.SetValue(Grid.RowSpanProperty, 2);
            leftImages.Add(image);
            mainTable.Add(image, 0, 1);
        });
        MessagingCenter.Subscribe<string>(this, "CleanTable", (arg) =>
        {
            leftImages.Where(i => string.IsNullOrEmpty(i.ClassId)).ToList().ForEach(i => i.IsVisible = false);
            rightImages.Where(i => string.IsNullOrEmpty(i.ClassId)).ToList().ForEach(i => i.IsVisible = false);
            partnerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).ToList().ForEach(i => i.IsVisible = false);
            playerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).ToList().ForEach(i => i.IsVisible = false);
        });
        MessagingCenter.Subscribe<string>(this, "AnimateCard", async (card) =>
        {
            //var c = JsonConvert.DeserializeObject<Card>(card);
            Image image;
            int rdm;
            switch (animationDirection)
            {
                case "left":
                    var sul = leftImages.Where(i => !string.IsNullOrEmpty(i.ClassId)).ToList();
                    rdm = _random.Next(0, sul.Count());
                    image = sul[rdm];
                    await image.TranslateTo(image.Height / 3, 0, 200);
                    image.Margin = new Thickness(0, 0, 0, 0);
                    image.ClassId = String.Empty;
                    image.HeightRequest = SMALL_CARD_HEIGHT;
                    image.WidthRequest = SMALL_CARD_WIDTH;
                    image.ZIndex = CARDS_ZINDEX++;

                    var l_t = image.TranslateTo(image.Height, 0, 250);
                    //var scale = image.ScaleTo(1.5, 250);
                    var l_r = image.RotateTo(270, 250);
                    await Task.Delay(125);
                    //image.HorizontalOptions = LayoutOptions.Center;
                    //image.HorizontalOptions = LayoutOptions.Center;
                    //image.SetValue(Grid.ColumnSpanProperty, 2);
                    //image.SetValue(Grid.RowSpanProperty, 2);//var d = c.Value + c.Suit.ToString()[0] + ".png";
                    image.Source = card.ToLower() + ".png";
                    await Task.Delay(125);
                    //image.ZIndex = playerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).Count();
                    break;
                case "right":
                    var sur = rightImages.Where(i => !string.IsNullOrEmpty(i.ClassId)).ToList();
                    rdm = _random.Next(0, sur.Count);
                    image = sur[rdm];
                    await image.TranslateTo(-(image.Height / 3), 0, 200);
                    image.Margin = new Thickness(0, 0, 0, 0);
                    image.ClassId = String.Empty;
                    image.HeightRequest = SMALL_CARD_HEIGHT;
                    image.WidthRequest = SMALL_CARD_WIDTH;
                    image.ZIndex = CARDS_ZINDEX++;
                    var r_t = image.TranslateTo(-(image.Height), 0, 250);
                    //image.ScaleTo(1.5, 250);
                    var r_r = image.RotateTo(270, 250);
                    await Task.Delay(125);
                    //image.HorizontalOptions = LayoutOptions.Center;
                    //image.HorizontalOptions = LayoutOptions.Center;
                    //image.SetValue(Grid.ColumnSpanProperty, 2);
                    //image.SetValue(Grid.RowSpanProperty, 2);
                    image.Source = card.ToLower() + ".png";
                    await Task.Delay(125);
                    //image.ZIndex = playerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).Count();
                    break;
                case "partner":
                    var sup = partnerImages.Where(i => !string.IsNullOrEmpty(i.ClassId)).ToList();
                    rdm = _random.Next(0, sup.Count);
                    image = sup[rdm];
                    await image.TranslateTo(0, image.Height / 3, 200);
                    image.Margin = new Thickness(0, 0, 0, (int)(image.Height / 3));
                    image.ClassId = String.Empty;
                    image.HeightRequest = SMALL_CARD_HEIGHT;
                    image.WidthRequest = SMALL_CARD_WIDTH;
                    image.ZIndex = CARDS_ZINDEX++;
                    var p_t = image.TranslateTo(0, image.Width, 250);
                    //image.ScaleTo(1.5, 250);
                    var p_r = image.RotateTo(180, 250);
                    await Task.Delay(125);
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.HorizontalOptions = LayoutOptions.Center;
                    //var d = c.Value + c.Suit.ToString()[0] + ".png";
                    image.Source = card.ToLower() + ".png";
                    await Task.Delay(125);
                    //image.ZIndex = playerImages.Where(i => string.IsNullOrEmpty(i.ClassId)).Count();
                    break;
                default:
                    break;
            }

        });
    }
}
public partial class CardAttributes : ObservableObject
{
    [ObservableProperty]
    Thickness margin;

    [ObservableProperty]
    int rotation;

    [ObservableProperty]
    LayoutOptions horizontalOptions;

    [ObservableProperty]
    LayoutOptions verticalOptions;

    [ObservableProperty]
    int heightRequest;

    [ObservableProperty]
    int widthRequest;
}