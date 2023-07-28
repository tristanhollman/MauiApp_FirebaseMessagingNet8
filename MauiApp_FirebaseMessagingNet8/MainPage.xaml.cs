using Microsoft.Azure.NotificationHubs;
using System.Globalization;

namespace MauiApp_FirebaseMessagingNet8;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = this;
    }

    private async void Register_Clicked(object sender, EventArgs e)
    {
        var fcmToken = Preferences.Default.Get<string?>("fcmtoken", null);

        if (string.IsNullOrEmpty(fcmToken))
        {
            await DisplayAlert("ERROR", "No FCM Token found", "Close");
            return;
        }

        var hubClient = InitializeNotificationHub();

        var registrationId = await hubClient.CreateRegistrationIdAsync();

        var tagToRegister = string.Format(CultureInfo.InvariantCulture, "urn:www-***:user-id:{0}_{1}", "TESTCLIENT", 1);

        var notificationTemplate = GetNotificationTemplate();

        var registration = new FcmTemplateRegistrationDescription(fcmToken, notificationTemplate, new[] { tagToRegister })
        {
            RegistrationId = registrationId,
            TemplateName = "NotificationTemplate"
        };

        await hubClient.CreateOrUpdateRegistrationAsync(registration);

        await DisplayAlert("SUCCESS", "Created registration in Azure", "Close");
    }

    private NotificationHubClient InitializeNotificationHub()
    {
        var azureNotificationHubPath = "***.test.notificationhub";
        var azureNotificationHubConnectionString = "Endpoint=***";

        return new NotificationHubClient(azureNotificationHubConnectionString, azureNotificationHubPath);
    }
    private static string GetNotificationTemplate()
    {
        var localizedMessageTemplateProperty = "MessageEnglish";
        return string.Format(CultureInfo.InvariantCulture, @"{{""data"":{{""{0}"":""$({1})"", ""{2}"":""$({2})"", ""{3}"":""$({3})"", ""{4}"":""$({4})""}}}}", "msg", localizedMessageTemplateProperty, "ScheduledWeekNumber", "ScheduledYear", "Category");
    }
}

