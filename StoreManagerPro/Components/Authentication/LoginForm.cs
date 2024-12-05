using System;
using System.Drawing;
using System.Windows.Forms;
using RestSharp;

namespace StoreManagerPro
{
    public partial class LoginForm : Form
    {
        private Timer fadeTimer; // Dùng cho chuyển trang
        public LoginForm()
        {
            InitializeComponent();
            LoadSavedCredentials();
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
            NavigateToLoginForm();
        }

        //Chuyển trang 
        private void NavigateToLoginForm()
        {
            // Initialize the Timer for fade-out
            fadeTimer = new Timer();
            fadeTimer.Interval = 10; // Faster updates for smoother fade
            fadeTimer.Tick += FadeOut;
            fadeTimer.Start();
        }
        private void FadeOut(object sender, EventArgs e)
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
                signupForm.StartPosition = FormStartPosition.Manual;
                signupForm.Location = this.Location;
                signupForm.ShowDialog();  // Show the new form

                // After showing the new form, dispose of the current form
                this.Close(); // Close the form
                this.Dispose(); // Dispose of the form's resources

                // Ensure the old form is completely removed from memory and taskbar
                Application.DoEvents(); // Allow UI to refresh and clear pending events
            }
        }




    }
}
