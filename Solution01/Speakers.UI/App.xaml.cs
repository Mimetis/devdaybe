namespace Speakers.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SQLitePCL.Batteries_V2.Init();

            MainPage = new AppShell();
        }
    }
}