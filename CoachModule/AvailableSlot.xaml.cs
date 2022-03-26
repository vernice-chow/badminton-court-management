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
    public sealed partial class AvailableSlot : Page
    {
        FirebaseHelper AdminFirebaseHelper = new FirebaseHelper();
        private List<CourtDetail> getinfo = new List<CourtDetail>();
        public AvailableSlot()
        {
            this.InitializeComponent();
            Available();

            futuredate.MinDate = DateTime.Now;
            futuredate.MaxDate = DateTime.Now.AddYears(1);


        }

        private async void Available()
        {
            try
            {
                List<CourtDetail> temp = new List<CourtDetail>();

                getinfo = await AdminFirebaseHelper.BookedDetail();
                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Now, DateTime.Parse(item.Dayy));
                    if (result < 0 && item.Email == "")
                        temp.Add(new CourtDetail(DateTime.Parse(item.Dayy).ToString("dd/MM/yyyy"), item.Courtss, item.Slot, item.Email, item.Time));

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

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SideAdmin));
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (inseertcourt.Text != "" && insertemail.Text != "" && futuretime != null)
                {

                    DateTime datee = futuredate.Date.Value.DateTime;
                    var format = datee.ToString("yyyy-MM-dd");
                    var daying = format.ToString();

                    var timing = formatTime(futuretime.Time.Hours, futuretime.Time.Minutes).ToString();
                    var courting = inseertcourt.Text.ToString();
                    var emailing = insertemail.Text.ToString();
                    int count = 0;
                    int wrongname = 0;
                    var updateonly = 0;
                    foreach (var item in getinfo)
                    {

                        if ((item.Dayy == daying) && (item.Time == timing) && (item.Courtss == courting) && (item.Email != ""))
                        {
                            DisplayDialog("Add Booking Error", "There is a crash on the existing booking.");

                            count++;
                            break;
                        }
                        if (item.Courtss != courting)
                        {
                            wrongname++;
                        }
                        if ((item.Dayy == daying) && (item.Time == timing) && (item.Courtss == courting) && (item.Email == ""))
                        {
                            await AdminFirebaseHelper.UpdateData(item.Dayy, timing, item.Slot, courting, emailing);
                            updateonly++;
                            DisplayDialog("Success", "Add sucessfully.");

                            break;
                        }
                    }
                    if (wrongname == getinfo.Count)
                    {
                        DisplayDialog("Add Booking Error", "Please check your Court name carefully.");
                        inseertcourt.Text = "";
                        inseertcourt.Focus(FocusState.Programmatic);

                    }
                    if (count == 0 && wrongname < getinfo.Count && updateonly == 0)
                    {
                        await AdminFirebaseHelper.AddSlot(daying, timing, courting, emailing);
                        DisplayDialog("Success", "Add sucessfully.");
                        inseertcourt.Text = "";
                        insertemail.Text = "";

                    }
                }
                else
                    DisplayDialog("Error", "Input cannot be empty.");
                // Custom method to format time
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }

        private string formatTime(int hours, int minutes)
        {
            // Put minutes in a string to format it
            string minutesString = minutes.ToString();

            // Determine the AM PM state
            bool isPM = hours > 11;
            string aMPMString = "AM";

            if (isPM)
            {
                aMPMString = "PM";
                if (hours != 12)
                {
                    hours -= 12;
                }
            }

            if (hours == 0)
            {
                hours = 12;
            }

            // Padding minutes with zero
            if (minutesString.Length == 1)
            {
                minutesString = minutesString.PadLeft(2, '0');
            }

            // Put hours in a string to format it
            string hoursString = hours.ToString();

            // Padding hours with zero


            string formattedTime = hoursString + "." + minutesString + aMPMString;
            return formattedTime;
        }

        private void BookingHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(History));
        }

        private void NavigateManage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ManageBooking));
        }

        private void HomePage_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

    }
}