using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using RestSharp;
using StoreManagerPro.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace StoreManagerPro
{
    public partial class LoginForm : Form
    {
        private Timer fadeTimer; // Dùng cho chuyển trang
        public LoginForm()
        {
            InitializeComponent();
            LoadSavedCredentials();
            Icon = new Icon(@"..\..\Resources\Logo-Yody-Yellow-1024x878.ico");
        }

        private void LoadSavedCredentials()
        {
            // Nạp tên đăng nhập đã lưu nếu có
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SavedUsername))
            {
                txtUsername.Text = Properties.Settings.Default.SavedUsername;
                toggleRememberme.Checked = true;
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var client = new RestClient("http://localhost:5254"); // URL API thật
            var request = new RestRequest("/api/account/login", Method.Post); // Endpoint đăng nhập

            request.AddJsonBody(new
            {
                username = username,
                password = password
            });

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Lưu tên đăng nhập nếu bật Remember Me
                    if (toggleRememberme.Checked)
                    {
                        Properties.Settings.Default.SavedUsername = username;
                    }
                    else
                    {
                        Properties.Settings.Default.SavedUsername = string.Empty;
                    }

                    Properties.Settings.Default.Save(); // Lưu vào cài đặt

                    // Chuyển sang màn hình chính (nếu cần)
                    NavigateToMainPage();
                }
                else
                {
                    MessageBox.Show("Login failed! Please check your credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            NavigateToSignupForm();
        }

        //Chuyển trang 
        private void NavigateToSignupForm()
        {
            // Initialize the Timer for fade-out
            fadeTimer = new Timer();
            fadeTimer.Interval = 10; // Faster updates for smoother fade
            fadeTimer.Tick += FadeToSignup;
            fadeTimer.Start();
        }
        private void NavigateToMainPage()
        {
            // Initialize the Timer for fade-out
            fadeTimer = new Timer();
            fadeTimer.Interval = 10; // Faster updates for smoother fade
            fadeTimer.Tick += FadeToMainPage;
            fadeTimer.Start();
        }
        private void FadeToSignup(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.1; // Faster fade with larger decrement
            }
            else
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();

                // Hide the current form before opening the SignupForm
                this.Hide();

                // Open SignupForm
                SignupForm signupForm = new SignupForm();
                signupForm.StartPosition = FormStartPosition.CenterScreen;
                signupForm.Location = this.Location;
                signupForm.ShowDialog();  // Show the new form

                // After showing the new form, dispose of the current form
                this.Close(); // Close the form
                this.Dispose(); // Dispose of the form's resources

                // Ensure the old form is completely removed from memory and taskbar
                Application.DoEvents(); // Allow UI to refresh and clear pending events
            }
        }
        private void FadeToMainPage(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.1; // Faster fade with larger decrement
            }
            else
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();

                // Hide the current form before opening the SignupForm
                this.Hide();

                // Open SignupForm
                MainPage mainPage = new MainPage();
                mainPage.StartPosition = FormStartPosition.CenterScreen;
                mainPage.Location = this.Location;
                mainPage.ShowDialog();  // Show the new form

                // After showing the new form, dispose of the current form
                this.Close(); // Close the form
                this.Dispose(); // Dispose of the form's resources

                // Ensure the old form is completely removed from memory and taskbar
                Application.DoEvents(); // Allow UI to refresh and clear pending events
            }
        }
        private void controlboxTurnOff_Click(object sender, EventArgs e)
        {
            if (!toggleRememberme.Checked)
            {
                Properties.Settings.Default.SavedUsername = string.Empty;
                Properties.Settings.Default.Save();
            }
        }
    }
}
