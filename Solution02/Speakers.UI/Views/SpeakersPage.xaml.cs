using Speakers.UI.ViewModels;
using System.Diagnostics;

namespace Speakers.UI.Views;

public partial class SpeakersPage : ContentPage {

    public SpeakersPage(SpeakersViewModel speakersViewModel) {
        Debug.WriteLine("SpeakersPage ctor");
        InitializeComponent();
        BindingContext = speakersViewModel;
    }

    async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e) {

        Debug.WriteLine("Calling SpeakerEdit");
        var speaker = e.CurrentSelection[0] as SpeakerViewModel;

        var navigationParameters = new Dictionary<string, object> { { "Speaker", speaker } };
        await Shell.Current.GoToAsync($"SpeakerEdit", navigationParameters);
    }
}