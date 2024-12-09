using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagerPro
{
    public partial class SignupForm : Form
    {
        private Timer fadeTimer; // Declare Timer globally - Dùng cho chuyển trang 
        public SignupForm()
        {
            InitializeComponent();
            Icon = new Icon(@"..\..\Resources\Logo-Yody-Yellow-1024x878.ico");
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate form fields
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtConfirmPassword.Text)
                || string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtEmail.Text)
                || string.IsNullOrEmpty(txtPhoneNumber.Text) || string.IsNullOrEmpty(txtAddress.Text) || !CbAcceptTerm.Checked)
            {
                lbError.Text = "Please fill in all required fields and accept the terms.";
                lbError.Visible = true;
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lbError.Text = "Passwords do not match.";
                lbError.Visible = true;
                return;
            }

            // Collect gender
            bool isMale = rBMale.Checked;

            var customerInfo = new
            {
                firstName = txtFirstName.Text,
                lastName = txtLastName.Text,
                male = isMale,
                phoneNumber = txtPhoneNumber.Text,
                address = txtAddress.Text,
                dateOfBirth = dtpBirthday.Value.ToString("yyyy-MM-dd"),
                email = txtEmail.Text
            };

            var signUpData = new
            {
                username = txtUsername.Text,
                password = txtPassword.Text,
                customerInfo
            };

            // Set up RestClient to send the data to the API
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/account/customer-register", Method.Post);
            request.AddJsonBody(signUpData);

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    lbError.Visible = false;
                    MessageBox.Show("🎉 Congratulations! Your account has been successfully created! 🎉", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    NavigateToLoginForm();
                }
                else
                {
                    lbError.Location = new Point(5, lbError.Location.Y);
                    lbError.Text = response.Content ?? "Sign up failed. Please check the details and try again.";
                    lbError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lbError.Text = $"An error occurred: {ex.Message}";
                lbError.Visible = true;
            }
        }

        private void lbLoginHere_Click(object sender, EventArgs e)
        {
            NavigateToLoginForm();
        }

        //Chuyển trang
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
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                loginForm.Location = this.Location; // Preserve the form's position
                loginForm.ShowDialog(); // Show LoginForm modally

                // Properly close the current form after showing the LoginForm
                this.Close(); // Close the current form
                this.Dispose(); // Dispose of the form to free resources

                // Ensure the old form is removed from memory and taskbar
                Application.DoEvents(); // Force UI to update and remove the old form from taskbar
            }
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtPassword.PasswordChar == '\0')
            {
                txtPassword.PasswordChar = '*'; // Ẩn mật khẩu
                txtPassword.IconRight = Image.FromFile("../../Resources/Closed_Eye.png"); // Biểu tượng nhắm mắt
            }
            else
            {
                txtPassword.PasswordChar = '\0'; // Hiển thị mật khẩu
                txtPassword.IconRight = Image.FromFile("../../Resources/Eye.png"); // Biểu tượng mở mắt
            }
        }

        private void txtConfirmPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtConfirmPassword.PasswordChar == '\0')
            {
                txtConfirmPassword.PasswordChar = '*'; // Ẩn mật khẩu
                txtConfirmPassword.IconRight = Image.FromFile("../../Resources/Closed_Eye.png"); // Biểu tượng nhắm mắt
            }
            else
            {
                txtConfirmPassword.PasswordChar = '\0'; // Hiển thị mật khẩu
                txtConfirmPassword.IconRight = Image.FromFile("../../Resources/Eye.png"); // Biểu tượng mở mắt
            }
        }
    }
}
