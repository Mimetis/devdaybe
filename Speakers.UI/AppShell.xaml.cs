using AppActions.Icons.Maui;
using Speakers.UI.Views;
using System.Windows.Input;

namespace Speakers.UI
{
    public partial class AppShell : Shell
    {
        public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();
        public ICommand HelpCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            BindingContext = this;
            
        }


        private void RegisterRoutes()
        {
            Routes.Add("Speakers", typeof(SpeakersPage));
            Routes.Add("SpeakerEdit", typeof(SpeakerEdit));

            foreach (var item in Routes)
                Routing.RegisterRoute(item.Key, item.Value);
        }
    }
}