using Speakers.UI.ViewModels;
using System.Diagnostics;

namespace Speakers.UI.Views;

public partial class SpeakersPage : ContentPage {

    public SpeakersPage(SpeakersViewModel speakersViewModel) {
        Debug.WriteLine("SpeakersPage ctor");
        InitializeComponent();
        BindingContext = speakersViewModel;
    }

    void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e) {
    }
}