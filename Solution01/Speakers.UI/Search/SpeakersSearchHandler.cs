using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Search
{
    public class SpeakersSearchHandler : SearchHandler
    {
        public IList<Speaker> Speakers { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }

        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);
            ItemsSource = null;

            //if (string.IsNullOrWhiteSpace(newValue))
            //{
            //}
            //else
            //{
            //    ItemsSource = Animals
            //        .Where(animal => animal.Name.ToLower().Contains(newValue.ToLower()))
            //        .ToList<Animal>();
            //}
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            
            // Let the animation complete
            await Task.Delay(1000);

            ShellNavigationState state = (App.Current.MainPage as Shell).CurrentState;
            // The following route works because route names are unique in this app.
            await Shell.Current.GoToAsync($"{GetNavigationTarget()}?name={((Speaker)item).SpeakerId}");
        }

        string GetNavigationTarget()
        {
            return (Shell.Current as AppShell).Routes.FirstOrDefault(route => route.Value.Equals(SelectedItemNavigationTarget)).Key;
        }
    }
}
