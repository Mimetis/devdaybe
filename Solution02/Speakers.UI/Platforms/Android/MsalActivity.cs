using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Speakers.UI.Platforms.Android
{
    [Activity(Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal7c66175f-d9f2-47f2-90bf-bc7ce36ef91d")]
    public class MsalActivity : BrowserTabActivity {
    }
}
