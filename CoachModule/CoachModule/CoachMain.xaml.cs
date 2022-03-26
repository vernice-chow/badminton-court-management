using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BadmintonCourtBookingSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class CoachMain : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        private List<Coach> allCoachs = new List<Coach>();

        public CoachMain()
        {

            this.InitializeComponent();
            SelectCoach();

        }

        public async void DisplayDialog(string title, string content)
        {
            ContentDialog noDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noDialog.ShowAsync();
        }

        public async void SelectCoach()
        {
            try
            {
                allCoachs = await FirebaseHelper.GetAllCoach();
                CoachList.ItemsSource = allCoachs;
            }
            catch (Exception theException)
            {
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }

        public void BookButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BookCoachPage));
        }

        private void emailButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            emailButton.IsEnabled = false;
            Send_Email();
            emailButton.IsEnabled = true;
        }

        private async void Send_Email()
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.Subject = "Enquiry about Hire Coach";
            emailMessage.Body = "I accidentally booked a coach and make a payment, how can I cancel it?";

            var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient("hirecoach@rapidbadmintoncourt.com");
            emailMessage.To.Add(emailRecipient);

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }
    }

}

