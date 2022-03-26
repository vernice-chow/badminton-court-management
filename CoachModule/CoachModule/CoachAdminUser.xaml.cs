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
    public sealed partial class CoachAdminUser : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        private List<Hire> allHires = new List<Hire>();

        public CoachAdminUser()
        {
            this.InitializeComponent();
            SelectHire();
        }

        private async void SelectHire()
        {
            try
            {
                allHires = await FirebaseHelper.GetAllHires();
                myHire.ItemsSource = allHires;
            }
            catch (Exception theException)
            {
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }


        private async void deleteCButton_Click(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                Image img = sender as Image;
                await FirebaseHelper.DeleteHire(img.Tag.ToString());
                DisplayDialog("Success", "Customer Deleted Successfully");
                //SelectHire();
            }
            catch (Exception theException)
            {
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

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CoachAdminUser));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SideAdmin));
        }
    }
}
