using RestSharp;
using System;
using System.Windows.Forms;

namespace StoreManagerPro.Components.Authentication
{
    public partial class ForgotPasswordForm : Form
    {
        private Timer fadeTimer; // Declare Timer globally - Dùng cho chuyển trang 
        public ForgotPasswordForm()
        {
            InitializeComponent();
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Get the email and username input
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please fill in both username and email.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare the data to be sent
            var requestData = new
            {
                username = username,
                email = email
            };

            try
            {
                var client = new RestClient("http://localhost:5254");
                var request = new RestRequest("/api/EmailSender/send", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(requestData);

                // Send the API request
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    MessageBox.Show("A password reset email has been sent to your email address.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to send password reset email. Error: {response.Content}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void NavigateToLoginForm()
        {
            // Initialize the Timer for fade-out
            fadeTimer = new Timer();
            fadeTimer.Interval = 10; // Faster updates for smoother fade
            fadeTimer.Tick += FadeToLogin;
            fadeTimer.Start();
        }
        private void FadeToLogin(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.05; // Faster fade with larger decrement
            }
            else
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();

                // Hide the current form before opening the LoginForm
                this.Hide(); // Hide to prevent gaps or visual issues during transition

                // Open the LoginForm after fading out
                LoginForm loginForm = new LoginForm();
                loginForm.StartPosition = FormStartPosition.Manual;
                loginForm.Location = this.Location; // Preserve the form's position
                loginForm.ShowDialog(); // Show LoginForm modally

                // Properly close the current form after showing the LoginForm
                this.Close(); // Close the current form
                this.Dispose(); // Dispose of the form to free resources

                // Ensure the old form is removed from memory and taskbar
                Application.DoEvents(); // Force UI to update and remove the old form from taskbar
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            NavigateToLoginForm();
        }
    }
}
