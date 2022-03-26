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
    public sealed partial class SideAdmin : Page
    {
        FirebaseHelper AdminFirebaseHelper = new FirebaseHelper();

        private List<CourtDetail> getinfo = new List<CourtDetail>();
        public SideAdmin()
        {
            this.InitializeComponent();
            SelectPerson();

        }


        private async void SelectPerson()
        {
            try
            {

                List<CourtDetail> temp = new List<CourtDetail>();
                int count = 0;
                getinfo = await AdminFirebaseHelper.BookedDetail();
                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Parse((DateTime.Now).ToString("yyyy-MM-dd")), DateTime.Parse(item.Dayy));
                    if (result <= 0)
                        temp.Add(new CourtDetail(DateTime.Parse(item.Dayy).ToString(), item.Courtss, item.Slot, item.Email, item.Time));

                }

                displayinfo.ItemsSource = temp;

                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Parse((DateTime.Now).ToString("yyyy-MM-dd")), DateTime.Parse(item.Dayy));
                    if (result == 0)
                    {
                        if (item.Email != "")
                            count++;
                    }

                }
                counttoday.Text = count.ToString();
                var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                var yesterdaycount = 0;
                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Parse(yesterday), DateTime.Parse(item.Dayy));
                    if (result == 0)
                    {
                        if (item.Email != "")
                            yesterdaycount++;
                    }

                }
                if (count >= yesterdaycount)
                {
                    compareyesterday.Text = "+" + (count - yesterdaycount).ToString();

                }
                if (count < yesterdaycount)
                {
                    compareyesterday.Text = (count - yesterdaycount).ToString();

                }


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




        private void NavigateManage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ManageBooking));
        }

        private void BookingHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(History));
        }

        private void AvailableSlot_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AvailableSlot));
        }

        private void coachButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CoachAdmin));
        }

        private void viewHireButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CoachAdminUser));
        }

        private void sellerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EquipmentSellerPanel));
        }
    }
}
