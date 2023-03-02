using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Speakers.UI.ViewModels
{
    public class SpeakersViewModel : BaseViewModel
    {
        private Speaker selectedSpeaker;
        private bool isRefreshing;

        public ObservableCollection<SpeakerViewModel> Speakers { get; private set; } = new ObservableCollection<SpeakerViewModel>();

        public ICommand RefreshCommand => new Command(async () => await GetSpeakersAsync());

        public ICommand DeleteCommand => new Command<SpeakerViewModel>(RemoveMonkey);
        public ICommand FavoriteCommand => new Command<SpeakerViewModel>(FavoriteMonkey);


        void RemoveMonkey(SpeakerViewModel speaker)
        {
            if (Speakers.Contains(speaker))
            {
                Speakers.Remove(speaker);
            }
        }


        void FavoriteMonkey(SpeakerViewModel speaker)
        {
            
        }
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public Speaker SelectedSpeaker
        {
            get => selectedSpeaker;
            set => SetProperty(ref selectedSpeaker, value);
        }
        public IHttpClientFactory HttpClientFactory { get; }

        public SpeakersViewModel(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
            GetSpeakersAsync();
        }

        public async Task GetSpeakersAsync()
        {
            IsRefreshing = true;
            this.Speakers.Clear();
            // Create the client
            using HttpClient client = HttpClientFactory.CreateClient("localhost_android");

            try
            {
                var speakers = await client.GetFromJsonAsync<IEnumerable<Speaker>>("api/Speakers",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                foreach(var speaker in speakers)
                    this.Speakers.Add(new SpeakerViewModel(speaker));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting something fun to say: {Error}", ex);
                //_logger.LogError("Error getting something fun to say: {Error}", ex);
            }

            IsRefreshing = false;

        }
    }
}
