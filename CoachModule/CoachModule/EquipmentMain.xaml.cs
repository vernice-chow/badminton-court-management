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
    public sealed partial class EquipmentMain : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        // FirebaseStorageHelper firebaseStorageHelper = new FirebaseStorageHelper();

        private List<Merchandise> allMerchandise = new List<Merchandise>();

        public EquipmentMain()
        {
            this.InitializeComponent();

            SelectMerch();
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

        private async void SelectMerch()
        {
            try
            {
                allMerchandise = await FirebaseHelper.GetAllMerchandise();
                MerchList.ItemsSource = allMerchandise;
            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }

        }

        private void MerchList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var Clicked = (Merchandise)e.ClickedItem;
            this.Frame.Navigate(typeof(MerchDetail), Clicked);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }
    }
}
