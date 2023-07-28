using Android.App;
using Android.Content;
using Android.Media;
using AndroidX.Core.App;
using Firebase.Messaging;

namespace MauiApp_FirebaseMessagingNet8.Platforms.Android;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class MessagingService : FirebaseMessagingService
{
    // identifier of the pending intent.
    private const int PendingNotificationIntentRequestCode = 42;

    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        Console.WriteLine($"~~ MessagingService - OnNewToken {token}");
        Preferences.Default.Set("fcmtoken", token);
    }

    /// <summary>
    /// Called when a push notification is received.
    /// </summary>
    /// <param name="message">Message that has been received.</param>
    public override void OnMessageReceived(RemoteMessage message)
    {
        if (message?.Data != null)
        {
            Console.WriteLine($"~~ MessagingService - OnMessageReceived {message.Data.ToString()}");
            SendNotification(message.Data);
        }
    }

    private void SendNotification(IDictionary<string, string> data)
    {
        // Build intent that will be executed when the user presses the notification
        using var uiIntent = new Intent(this, typeof(MainActivity));
        uiIntent.AddFlags(ActivityFlags.ClearTop);
        uiIntent.AddFlags(ActivityFlags.SingleTop);

        // Passthrough all received Intent.Extras to the intent that will be started when the user presses the notification.
        foreach (string key in data.Keys.Where(k => k != "msg"))
        {
            uiIntent.PutExtra(key, data[key]);
        }

        using var pendingIntent = PendingIntent.GetActivity(this, PendingNotificationIntentRequestCode, uiIntent, PendingIntentFlags.Immutable);

        var messageTitle = "job app";
        var messageText = data["msg"];

        // Create big text style to show long lines.
        var bigTextStyle = new NotificationCompat.BigTextStyle();
        bigTextStyle.BigText(messageText);

        // Build the notification
        using var builder = new NotificationCompat.Builder(this, "my_test_notification_channel");
        builder.SetAutoCancel(true)
            .SetSmallIcon(Resource.Drawable.ic_notification)
            .SetColor(Resource.Color.colorPrimary)
            .SetColorized(true)
            .SetContentTitle(messageTitle)
            .SetContentText(messageText)
            .SetPriority(NotificationCompat.PriorityDefault)
            .SetStyle(bigTextStyle)
            .SetContentIntent(pendingIntent)
            .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
            .SetVibrate(new long[] { 200, 400, 100, 400 });

        var notificationManager = NotificationManagerCompat.From(this);

        // Using the GetHashCode method to give every notification a unique id based on the message text. 
        // This will automatically replace existing notifications when the message text is the same.
        notificationManager.Notify(messageText.GetHashCode(), builder.Build());
    }
}
