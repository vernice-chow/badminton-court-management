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
    public sealed partial class CustomerBookingCourt2 : Page
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        FirebaseStorageHelper firebaseStorageHelper = new FirebaseStorageHelper();

        private Windows.Storage.StorageFile file;

        private List<ImageTitle> allImageTitle = new List<ImageTitle>();
        private List<FileTitle> allFileTitle = new List<FileTitle>();

        public CustomerBookingCourt2()
        {
            this.InitializeComponent();
        }


        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            display.Text = "";

            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                picker.FileTypeFilter.Add(".pdf");
                picker.FileTypeFilter.Add(".txt");

                file = await picker.PickSingleFileAsync();

                display.Text = file.DisplayName;


            }
            catch (Exception theException)
            {
                // Handle all other exceptions.
                DisplayDialog("Error", "Error Message: " + theException.Message);

            }
        }


        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(title.Text))
                {

                    if (file != null)
                    {
                        var stream = await file.OpenStreamForReadAsync();

                        Guid fileName = Guid.NewGuid();

                        var str = await firebaseStorageHelper.UploafFileKL(stream, fileName + file.FileType.ToString());

                        if (!string.IsNullOrEmpty(str))
                        {
                            await firebaseHelper.AddImageTitle(title.Text, fileName + file.FileType.ToString(), str);

                            allFileTitle = await firebaseHelper.GetAllFileTitle();
                            //imglist.ItemsSource = null;
                            //imglist.ItemsSource = allImageTitle;

                            display.Text = "Uploaded and inserted success";
                        }

                    }
                    else
                        DisplayDialog("Error", "Please select an image");
                }
                else
                {
                    DisplayDialog("Error", "Please enter the title");
                    title.Focus(FocusState.Programmatic);

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }
    }
}
