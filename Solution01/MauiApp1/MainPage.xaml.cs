namespace MauiApp1 {
    public partial class MainPage : ContentPage {
        int count = 0;

        public MainPage() {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e) {

            var client = new HttpClient();

            var res = client.GetAsync("http://localhost:5054/");


            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}