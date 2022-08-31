using MauiJass.ViewModels;
using System.Text.RegularExpressions;

namespace MauiJass;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        (BindingContext as MainViewModel).Cleanup();
        base.OnNavigatedFrom(args);
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await (BindingContext as MainViewModel).Init();
        base.OnNavigatedTo(args);
    }

    void Entry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        var isValid = Regex.IsMatch(e.NewTextValue, "^[a-zA-Z][a-zA-Z0-9]*$");
        Entry entry = sender as Entry;
        if (isValid)
        {
            string val = entry.Text;
            if (e.NewTextValue.Length > 0)
            {
                var currentChar = e.NewTextValue.Last();
                if (currentChar == ' ')
                {
                    entry.Text = e.OldTextValue;
                }
                if (val.Length > 10)
                {
                    val = val.Remove(val.Length - 1);
                    entry.Text = val;
                }
            }
        }
        else
        {
            if (e.NewTextValue.Length > 0)
            {
                entry.Text = e.OldTextValue;
            }
            else
            {
                entry.Text = "";
            }
        }
    }
}

