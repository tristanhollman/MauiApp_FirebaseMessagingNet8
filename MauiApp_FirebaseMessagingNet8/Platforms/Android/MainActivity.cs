using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace MauiApp_FirebaseMessagingNet8;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // From Android SDK 33 we need to manually request the notification permission before creating a notification channel.
        if (OperatingSystem.IsAndroidVersionAtLeast(33) && ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
        {
            // Note, this will always trigger a prompt, even if the user previously declined the permission request.
            ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.PostNotifications }, 0);
        }
        CreateNotificationChannel();
        OnNewIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
    }

    private void CreateNotificationChannel()
    {
        // Register the notification channel with the OS
        Console.WriteLine("~~ MainActivity - CreateNotificationChannel");
        var notificationManager = GetSystemService(NotificationService) as NotificationManager;
        notificationManager!.CreateNotificationChannel(new NotificationChannel("my_test_notification_channel", "test nofitication channel", NotificationImportance.Default));
    }
}
