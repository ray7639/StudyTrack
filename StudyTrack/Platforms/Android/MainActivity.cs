#if ANDROID || __ANDROID__
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace StudyTrack
{
    [global::Android.App.Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = global::Android.Content.PM.LaunchMode.SingleTop,
        ConfigurationChanges = global::Android.Content.PM.ConfigChanges.ScreenSize | global::Android.Content.PM.ConfigChanges.Orientation | global::Android.Content.PM.ConfigChanges.UiMode |
                               global::Android.Content.PM.ConfigChanges.ScreenLayout | global::Android.Content.PM.ConfigChanges.SmallestScreenSize | global::Android.Content.PM.ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
#endif
