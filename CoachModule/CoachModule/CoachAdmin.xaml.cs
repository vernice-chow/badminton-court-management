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
    public sealed partial class CoachAdmin : Page
    {
        FirebaseHelper FirebaseHelper = new FirebaseHelper();
        private List<Coach> allCoachs = new List<Coach>();
        private string edit = "";
      

        public CoachAdmin()
        {
            this.InitializeComponent();

            SelectCoach();
        }

        private async void SelectCoach()
        {
            try
            {
                allCoachs = await FirebaseHelper.GetAllCoach();
                myCoach.ItemsSource = allCoachs;
            }
            catch (Exception theException)
            {
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }

        private async void deleteCoachButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Image img = sender as Image;
                await FirebaseHelper.DeleteCoach(img.Tag.ToString());
                DisplayDialog("Success", "Person Deleted Successfully");
                SelectCoach();

            }
            catch (Exception theException)
            {
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }

        private async void addCoachButton_Click(object sender, RoutedEventArgs e)
        {
            int phoneCoach; 
            try
            {
                if (int.TryParse(coachPhoneTextBox.Text, out phoneCoach))
                {
                    if ((!string.IsNullOrEmpty(coachNameTextBox.Text)) && (!string.IsNullOrEmpty(coachPriceTextBox.Text)) && (!string.IsNullOrEmpty(coachDescriptionTextBox.Text)) && (!string.IsNullOrEmpty(coachCourtTextBox.Text)))
                    {
                        await FirebaseHelper.AddCoach(coachNameTextBox.Text, coachPhoneTextBox.Text, coachPriceTextBox.Text, coachDescriptionTextBox.Text, coachCourtTextBox.Text);

                        DisplayDialog("Success", "Coach Added Successfully");

                        SelectCoach();

                        coachNameTextBox.Text = "";
                        coachPhoneTextBox.Text = "";
                        coachPriceTextBox.Text = "";
                        coachDescriptionTextBox.Text = "";
                        coachCourtTextBox.Text = "";
                    }
                    else
                        DisplayDialog("Input", "Please key in all the information.");
                }
                else
                {
                    DisplayDialog("Input Incorrect", "Please make sure all the information are correct.");
                }


            }
            catch (Exception theException)
            {
                DisplayDialog("Error", "Error Message: " + theException.Message);
            }
        }

        private void editCoachButton_Click(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            edit = img.Tag.ToString();

            var editing = allCoachs.SingleOrDefault(r => r.CoachID == img.Tag.ToString());
            if (editing != null)
            {
                //set that the name of coach cannot be change
                coachNameTextBox.IsEnabled = false;
                coachNameTextBox.Text = editing.CoachName.ToString();
                coachPhoneTextBox.Text = editing.CoachPhone.ToString();
                coachPriceTextBox.Text = editing.CoachPrice.ToString();
                coachDescriptionTextBox.Text = editing.CoachDescription.ToString();
                coachCourtTextBox.Text = editing.CoachCourt.ToString();
            }
        }

        private async void updateCoachButton_Click(object sender, RoutedEventArgs e)
        {
            int phoneCoach;
            try
            {
                if (int.TryParse(coachPhoneTextBox.Text, out phoneCoach))
                {
                    if ((!string.IsNullOrEmpty(coachNameTextBox.Text)) && (!string.IsNullOrEmpty(coachPriceTextBox.Text)) && (!string.IsNullOrEmpty(coachDescriptionTextBox.Text)) && (!string.IsNullOrEmpty(coachCourtTextBox.Text)))
                    {
                        await FirebaseHelper.UpdateCoach(edit, coachPhoneTextBox.Text, coachPriceTextBox.Text, coachDescriptionTextBox.Text, coachCourtTextBox.Text);

                        DisplayDialog("Success", "Coach Updated Successfully");

                        SelectCoach();

                        coachNameTextBox.IsEnabled = true;
                        coachNameTextBox.Text = "";
                        coachPhoneTextBox.Text = "";
                        coachPriceTextBox.Text = "RM";
                        coachDescriptionTextBox.Text = "";
                        coachCourtTextBox.Text = "Court ";

                    }
                    else
                        DisplayDialog("Input", "Please key in all the information.");

                }
                else
                {
                    DisplayDialog("Input Incorrect", "Please make sure all the information are correct.");
                }

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SideAdmin));
        }
    }
}
