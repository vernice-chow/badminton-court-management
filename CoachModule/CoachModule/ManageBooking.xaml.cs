using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class ManageBooking : Page
    {
        FirebaseHelper AdminFirebaseHelper = new FirebaseHelper();
        private List<CourtDetail> getinfo = new List<CourtDetail>();
        public ManageBooking()
        {
            this.InitializeComponent();
            DisplayBooking();

        }

        private async void DisplayBooking()
        {
            try
            {
                List<CourtDetail> temp = new List<CourtDetail>();
                getinfo = await AdminFirebaseHelper.BookedDetail();
                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Now, DateTime.Parse(item.Dayy));
                    if (result < 0 && item.Email != "")
                        temp.Add(new CourtDetail(DateTime.Parse(item.Dayy).ToString("yyyy-MM-dd"), item.Courtss, item.Slot, item.Email, item.Time));

                }


                futurebooking.ItemsSource = temp;

            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog noDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noDialog.ShowAsync();
        }



        private async Task CancelEmail(string name)
        {

            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage
            {
                Subject = "Cancellation of Booking From RAPID Badminton",
                Body = "Your booking has being cancel successfully. \n If you didn't request on this step, kindly contact us as soon as possible.\n Regards, \nRAPID Badminton"
            };

            var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient(name);
            emailMessage.To.Add(emailRecipient);

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }


        private async void CancelButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            afterclick.Visibility = Visibility.Collapsed;
            try
            {
                Button btn = sender as Button;

                btn.IsEnabled = false;

                btn.IsEnabled = true;
                var day = "";
                var court = "";
                var time = "";
                var key = btn.Tag.ToString();
                var email = "";
                int error = 0;
                foreach (var item in getinfo)
                {

                    if (item.Slot == key)
                    {
                        day = item.Dayy;
                        court = item.Courtss;
                        time = item.Time;
                        email = item.Email;
                    }
                    else
                        error++;
                }

                if (error == getinfo.Count)
                {
                    DisplayDialog("Error", "The record coundn't be found in daatbase");
                }
                else
                {
                    ContentDialog noDialog = new ContentDialog
                    {
                        Title = "Confirmation",

                        Content = "This action will  delete to the " + email + "'s booking. \nDo you want to delete it ?",
                        CloseButtonText = "No",
                        PrimaryButtonText = "Yes"

                    };

                    ContentDialogResult result = await noDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {

                        await AdminFirebaseHelper.DeleteBooking(day, time, key, court);
                        await CancelEmail(email);
                    }
                }
            }

            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }
        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                btn.IsEnabled = true;

                if (!string.IsNullOrWhiteSpace(updatetime.Text) && !string.IsNullOrWhiteSpace(updateemail.Text))
                {
                    var emailing = updateemail.Text.ToString();
                    var timing = updatetime.Text.ToString();
                    var key = yes;
                    int error = 0;
                    var day = "";
                    var court = "";

                    foreach (var item in getinfo)
                    {

                        if (item.Slot == key)
                        {
                            day = item.Dayy;
                            court = item.Courtss;
                        }
                    }
                    foreach (var item in getinfo)
                    {
                        //info not change
                        if (item.Email == emailing && item.Time == timing && item.Slot == key)
                        {
                            DisplayDialog("Update error", "There is a record with same intance. Please check again");
                            error++;
                            break;
                        }
                        //info crash
                        if (item.Time == timing && item.Dayy == day && item.Email != emailing && item.Courtss == court && item.Slot != key)
                        {
                            DisplayDialog("Update error", "There is a crash on booking time. Please check again");
                            error++;
                            break;
                        }

                    }
                    //no error
                    if (error == 0)
                    {
                        await AdminFirebaseHelper.UpdateData(day, timing, key, court, emailing);
                        afterclick.Visibility = Visibility.Collapsed;
                        DisplayDialog("Success", "Update Successfully.");
                        futurebooking.ItemsSource = null;
                        DisplayBooking();
                    }
                }
                else
                    DisplayDialog("Error", "The Update Info cannot be empty: ");
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }
        private string yes = "";
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            btn.IsEnabled = false;
            afterclick.Visibility = Visibility.Visible;
            yes = btn.Tag.ToString();

            foreach (var item in getinfo)
            {
                if (item.Slot == yes)
                {
                    updatetime.Text = item.Time.ToString();
                    updateemail.Text = item.Email.ToString();
                }
            }
            btn.IsEnabled = false;
        }


        private void History_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(History));
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SideAdmin));
        }

        private void Available_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AvailableSlot));
        }
    }
}
