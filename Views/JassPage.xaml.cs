using MauiJass.ViewModels;

namespace MauiJass;

public partial class JassPage : ContentPage
{
    
    private Timer _blinkTimer;
    public JassPage(JassViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _blinkTimer = new Timer(BlinkTimerCallback);
        _blinkTimer.Change(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 0, 500));
    }
    bool _toggle;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as JassViewModel).Connect();
        //await newGameFrame.ScaleTo(1.1, 300);

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //_blinkTimer.Dispose();
        (BindingContext as JassViewModel).Disconnect();
    }

    private void BlinkTimerCallback(object state)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (_toggle)
            {
                await newGameFrame.ScaleTo(1.1, 300);
            }
            else
            {
                await newGameFrame.ScaleTo(0.9, 300);
            }
            _toggle = !_toggle;
        });
    }
}

