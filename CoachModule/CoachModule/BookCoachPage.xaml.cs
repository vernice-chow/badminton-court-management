using Firebase.Database;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
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
    public sealed partial class BookCoachPage : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        private List<Coach> allCoachs = new List<Coach>();
        private List<Hire> allHire = new List<Hire>();
        FirebaseClient firebase = new FirebaseClient(GlobalData.firebaseDatabase);


        public BookCoachPage()
        {
            this.InitializeComponent();

            preferDate.MinDate = DateTimeOffset.Now.AddDays(1);
            preferDate.MaxDate = DateTimeOffset.Now.AddMonths(6);

            SelectCoach();
        }

        private async void SelectCoach()
        {
            try
            {
                allCoachs = await FirebaseHelper.GetAllCoach();
                CoachList2.ItemsSource = allCoachs;

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


        private void CoachButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.IsEnabled)
            {
                coachchoose.Text = btn.Tag.ToString();
                //btn.Background = new SolidColorBrush(Colors.Aqua);
            }
        }


        private async void infoCButton_Click(object sender, RoutedEventArgs e)
        {
            int userPhone;

            try
            {
                if (int.TryParse(userPhoneTextBox.Text, out userPhone))
                {
                    if (((!string.IsNullOrEmpty(userEmailTextBox.Text)) && preferDate != null && preferTime != null))
                    {
                        //retrieve date
                        var date = preferDate.Date;
                        DateTime mydate = date.Value.DateTime;
                        var formatedtime = mydate.ToString("dd/M/yyyy");
                        datechoose.Text = formatedtime.ToString();

                        //retrieve time
                        var time = preferTime.SelectedTime;
                        timechoose.Text = time.ToString();

                        await FirebaseHelper.AddHire(userEmailTextBox.Text, userPhoneTextBox.Text, datechoose.Text, timechoose.Text, coachchoose.Text);
                        infoButton.IsEnabled = false;

                        SelectCoach();

                        //display
                        emailchoose.Text = userEmailTextBox.Text;
                        phonechoose.Text = userPhoneTextBox.Text;

                        string messageString = "Email Address: " + userEmailTextBox.Text + "\n" + "Phone Number: " + userPhoneTextBox.Text + "\n" + "Date Choosen: " + formatedtime.ToString() + "\n" + "Time Choosen: " + time.ToString() + "\n" + "Selected Coach: " + coachchoose.Text;

                        DisplayDialog("Summary", messageString);

                    }
                    else
                    {
                        DisplayDialog("Input", "Please key in all the information.");
                    }
                }
                else
                {
                    DisplayDialog("Input Incorrect", "Please make sure all the information are correct.");
                }
                
            }
            catch (Exception exp)
            {
                DisplayDialog("Error: ", "Error: " + exp.Message);
            }
        }

        private void saveBookingCButton_Click(object sender, RoutedEventArgs e)
        {
            string timing = DateTime.Now.ToString("h:mm:ss tt");
            string today = DateTime.Today.ToString("d/M/yyyy");

            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

                //Draw the text
                graphics.DrawString(titleInfo.Text + "\n\nEmail Address: " + emailchoose.Text + "\nContact Number: " + phonechoose.Text + "\nPrefer Date: " + datechoose.Text + "\nPrefer Time: " + timechoose.Text + "\nCoach Choosen: " + coachchoose.Text + "\n\nAuto generate pdf file at \nCurrent Date: " + today + "\nCurrent Time: " + timing, font, PdfBrushes.Black, new PointF(0, 0));
             

                MemoryStream memorystream = new MemoryStream();
                document.Save(memorystream);
                document.Close(true);
                SaveCoachTopdf(memorystream, "Sample.pdf");
            }
        }

        public async void SaveCoachTopdf(Stream stream, string filename)
        {
            stream.Position = 0;

            StorageFile stFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".pdf";
                savePicker.SuggestedFileName = "BookingCoachInformation";
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                stFile = await savePicker.PickSaveFileAsync();
            }

            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }

            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
                st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
                MessageDialog messagedialog = new MessageDialog("Do you want to view the Document?", "File created.");
                UICommand yesCmd = new UICommand("Yes");
                messagedialog.Commands.Add(yesCmd);
                UICommand noCmd = new UICommand("No");
                messagedialog.Commands.Add(noCmd);
                IUICommand cmd = await messagedialog.ShowAsync();
                if (cmd == yesCmd)
                {
                    bool success = await Windows.System.Launcher.LaunchFileAsync(stFile);
                }
            }
        }

        private void exitCButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void returnCButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.TryGoBack();
        }
    }
}