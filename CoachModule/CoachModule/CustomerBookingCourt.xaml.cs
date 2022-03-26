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
    public sealed partial class CustomerBookingCourt : Page
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        private List<Court> allCourt = new List<Court>();

        //List<Court> getCourtNumber = new List<Court>();

        private List<KLBooking> bookedCourt = new List<KLBooking>();
        //private List<KLConfirmBooking> klConfirmBooking = new List<KLConfirmBooking>();


        public CustomerBookingCourt()
        {
            this.InitializeComponent();
            SelectCourt();
        }


        private async void SelectCourt()
        {
            try
            {
                allCourt = await firebaseHelper.GetAllCourts();
                CourtList.ItemsSource = allCourt;

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
            //ContentDialog dialouge = new ContentDialog();
            ContentDialogResult result = await noDialog.ShowAsync();
        }


        private async void addButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var code = button.Tag.ToString();
                int count = 0;

                List<KLBooking> temp = new List<KLBooking>();
                bookedCourt = await firebaseHelper.GetBookedCourt();
                foreach (var item in bookedCourt)
                {

                    if (item.BookedSlot == code)
                    {
                        DisplayDialog("Duplicate/Not Available", "There is already one existing record in your booking list.");
                        //break;
                        bookedCourt = await firebaseHelper.GetBookedCourt();
                        count++;
                    }
                }
                if (count == 0)
                {
                    await firebaseHelper.AddSlot(code);
                    DisplayDialog("Success", "This court has been added to your booking list.");
                    bookedCourt = await firebaseHelper.GetBookedCourt();
                }

                bookedCourt = await firebaseHelper.GetBookedCourt();
                AddedList.ItemsSource = bookedCourt;
                await firebaseHelper.UpdateCondition(code);
            }

            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }


        private async void deleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                var keykey = btn.Tag.ToString();

                //new try
                List<KLBooking> temp = new List<KLBooking>();
                bookedCourt = await firebaseHelper.GetBookedCourt();

                await firebaseHelper.DeleteSlot(keykey);

                DisplayDialog("Deletion Sucess", "The court has been successfully removed from your booking.");
                bookedCourt = await firebaseHelper.GetBookedCourt();

                bookedCourt = await firebaseHelper.GetBookedCourt();
                AddedList.ItemsSource = bookedCourt;
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }


        private void lnkUploadImage_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CustomerBookingCourt2));
        }


        private void calcFee_Click(object sender, RoutedEventArgs e)
        {
            int bookingNum = AddedList.Items.Count();
            int fee = bookingNum * 12;
            feees.Text = "Total booking fees are RM" + fee.ToString() + ".00.";
            qrcode.Visibility = Visibility.Visible;
        }


        private async void confirmOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((!string.IsNullOrEmpty(userName.Text)) && (!string.IsNullOrEmpty(userEmail.Text)))
                {
                    await firebaseHelper.AddPerson(userName.Text, userEmail.Text, feees.Text);
                    DisplayDialog("Success", "Booking is Success.");
                }
                else
                    DisplayDialog("Input", "Please key in all the information.");
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }
    }
}

