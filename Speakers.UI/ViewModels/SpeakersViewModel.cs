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

namespace Speakers.UI.ViewModels {
    public class SpeakersViewModel : BaseViewModel {
        private Speaker selectedSpeaker;
        private bool isRefreshing;

        public ObservableCollection<SpeakerViewModel> Speakers { get; private set; } = new ObservableCollection<SpeakerViewModel>();

        public ICommand RefreshCommand => new Command(async () => await GetSpeakersAsync());

        public ICommand DeleteCommand => new Command<SpeakerViewModel>(RemoveSpeaker);
        public ICommand FavoriteCommand => new Command<SpeakerViewModel>(FavoriteMonkey);


        void RemoveSpeaker(SpeakerViewModel speaker) {
            if (Speakers.Contains(speaker)) {
                Speakers.Remove(speaker);
            }
        }


        void FavoriteMonkey(SpeakerViewModel speaker) {

        }
        public bool IsRefreshing {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        public Speaker SelectedSpeaker {
            get => selectedSpeaker;
            set => SetProperty(ref selectedSpeaker, value);
        }
        public IHttpClientFactory HttpClientFactory { get; }

        public SpeakersViewModel(IHttpClientFactory httpClientFactory) {
            HttpClientFactory = httpClientFactory;

        }


        public ICommand AppearingCommand => new Command(() => {
            if (this.IsRefreshing)
                return;

            this.IsRefreshing = true;
        });
        public ICommand DisappearingCommand => new Command(() => {
            this.IsRefreshing = false;
        });


        public async Task GetSpeakersAsync() {
            this.Speakers.Clear(); // for demo purpose

            using HttpClient client = HttpClientFactory.CreateClient("api");

            try {
                var speakers = await client.GetFromJsonAsync<IEnumerable<Speaker>>("api/Speakers",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                foreach (var speaker in speakers)
                    this.Speakers.Add(new SpeakerViewModel(speaker));
            }
            catch (Exception ex) {
                Debug.WriteLine("Error getting something fun to say: {Error}", ex);
                //_logger.LogError("Error getting something fun to say: {Error}", ex);
            }

            this.IsRefreshing = false;
        }
    }
}
