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
    public sealed partial class History : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        private List<CourtDetail> getinfo = new List<CourtDetail>();
        public History()
        {
            this.InitializeComponent();
            DisplayBooking();

        }

        private async void DisplayBooking()
        {
            try
            {
                List<CourtDetail> temp = new List<CourtDetail>();

                getinfo = await FirebaseHelper.BookedDetail();
                foreach (var item in getinfo)
                {
                    int result = DateTime.Compare(DateTime.Now, DateTime.Parse(item.Dayy));
                    if (result > 0)
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
        private async void CancelButton_ClickAsync(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;

            btn.IsEnabled = false;


            ContentDialog noDialog = new ContentDialog
            {
                Title = "Confirmation",

                Content = "This action will cascade delete to all the " + btn.Tag.ToString() + " information. \nDo you want to delete it ?",
                CloseButtonText = "No",
                PrimaryButtonText = "Yes"

            };

            ContentDialogResult result = await noDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var x = btn.Tag.ToString();
                await FirebaseHelper.DeleteHistory(x);

                List<CourtDetail> getinfo = new List<CourtDetail>();

                futurebooking.ItemsSource = null;

                ContentDialog returnback = new ContentDialog
                {
                    Title = "Success",

                    Content = "The selected records successfully delete. Please go to HomePage for page refreshing",

                    PrimaryButtonText = "Home Page"

                };

                ContentDialogResult homepage = await returnback.ShowAsync();

                if (homepage == ContentDialogResult.Primary)
                {
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            btn.IsEnabled = true;



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

        private void ManageBooking_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ManageBooking));
        }

        private void AvailableSlot_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AvailableSlot));
        }
    }
}

