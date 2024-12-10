using Newtonsoft.Json;
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

namespace StoreManagerPro.Components.AdminControl
{
    public partial class ProviderManage : UserControl
    {
        private int currentPage = 1; // Current page number
        private int pageSize = 10;  // Number of records per page
        private List<Provider> allProviders; // Store all data fetched from the API
        public ProviderManage()
        {
            InitializeComponent();
        }

        private async void ProviderManage_Load(object sender, EventArgs e)
        {
            DataGridViewProvider.CellClick += DataGridViewProvider_CellClick;

            DataGridViewProvider.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewProvider.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewProvider.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewProvider.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewProvider.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewProvider.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewProvider.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewProvider.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewProvider.ContextMenuStrip = contextMenuStrip1;

            // Enable gridlines
            DataGridViewProvider.GridColor = System.Drawing.Color.Black; // Set gridline color to black (or any color you prefer)
            DataGridViewProvider.CellBorderStyle = DataGridViewCellBorderStyle.Single; // Define gridline style

            // Show row and column borders
            DataGridViewProvider.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Row header borders
            DataGridViewProvider.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Column header borders


            allProviders = await FetchProvidersAsync();
            currentPage = 1; // Start from the first page
            LoadPage();
        }
        public class Provider
        {
            public int providerId { get; set; }
            public string ProviderEmail { get; set; }
            public string ProviderPhone { get; set; }
            public string ProviderCompanyName { get; set; }
        }
        private async void LoadProvidersIntoDataGridView()
        {
            var colors = await FetchProvidersAsync();

            // Set up the DataGridView
            DataGridViewProvider.Rows.Clear();
            DataGridViewProvider.Columns.Clear();

            // Define columns
            DataGridViewProvider.Columns.Add("providerId", "Provider Id");
            DataGridViewProvider.Columns.Add("providerEmail", "Email");
            DataGridViewProvider.Columns.Add("providerPhone", "Phone");
            DataGridViewProvider.Columns.Add("providerCompanyName", "Company Name");
        }
        private async Task<List<Provider>> FetchProvidersAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/providers", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var providers = JsonConvert.DeserializeObject<List<Provider>>(response.Content);
                    return providers ?? new List<Provider>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch providers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Provider>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching providers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Provider>();
            }
        }
        private void LoadPage()
        {
            if (allProviders == null || allProviders.Count == 0)
            {
                MessageBox.Show("No providers available to display.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedSizes = allProviders.Skip(skip).Take(pageSize).ToList();

            // Set up the DataGridView
            DataGridViewProvider.Rows.Clear();
            DataGridViewProvider.Columns.Clear();

            // Define columns
            DataGridViewProvider.Columns.Add("providerId", "Provider Id");
            DataGridViewProvider.Columns.Add("providerEmail", "Email");
            DataGridViewProvider.Columns.Add("providerPhone", "Phone");
            DataGridViewProvider.Columns.Add("providerCompanyName", "Company Name");

            // Populate rows
            foreach (var provider in pagedSizes)
            {
                DataGridViewProvider.Rows.Add(provider.providerId,provider.ProviderEmail, provider.ProviderPhone, provider.ProviderCompanyName);
            }

            // Update page information (optional UI labels/buttons)
            lbPageNumber.Text = $"{currentPage} / {Math.Ceiling((double)allProviders.Count / pageSize)}";

            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < Math.Ceiling((double)allProviders.Count / pageSize);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < Math.Ceiling((double)allProviders.Count / pageSize))
            {
                currentPage++;
                LoadPage();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPage();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = true;
            flowLayoutEdit.Visible = false;
        }

        private void txtClose_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
        }


        private void btnCloseEdit_Click(object sender, EventArgs e)
        {
            flowLayoutEdit.Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }
        private async void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string email = txtEmailEdit.Text;
            string phone = txtPhoneEdit.Text;
            string companyName = txtCompanyEdit.Text;
            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(companyName))
            {
                MessageBox.Show("Please provide the email, phone number and company name.");
                return;
            }

            // Create the category object to be sent in the request body
            var provider = new Provider
            {
                ProviderEmail = email,
                ProviderPhone = phone,
                ProviderCompanyName = companyName
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/providers/{selectedProviderId}", Method.Put); // Use PUT for updating
            request.AddJsonBody(provider);  // Add the color data as JSON

            try
            {
                // Execute the PUT request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Refresh the color data and update UI
                    ProviderManage_Load(sender, e);
                    MessageBox.Show("Provider updated successfully!");
                }
                else
                {
                    MessageBox.Show("Error updating provider: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private async void txtSave_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string email = txtProviderEmail.Text;
            string phone = txtProviderPhone.Text;
            string companyName = txtProviderCompany.Text;
            // Validate inputs
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(companyName))
            {
                MessageBox.Show("Please provide the email, phone number and company name.");
                return;
            }

            // Create the category object to be sent in the request body
            var provider = new Provider
            {
                ProviderEmail = email,
                ProviderPhone = phone,
                ProviderCompanyName = companyName
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/providers", Method.Post);
            request.AddJsonBody(provider);  // Add the category data as JSON

            try
            {
                // Execute the POST request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    ProviderManage_Load(sender, e);
                    MessageBox.Show("Provider added successfully!");
                    flowLayoutAdd.Visible = false;
                }
                else
                {
                    MessageBox.Show("Error adding provider: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();  // Get the search term and convert to lowercase for case-insensitive comparison

            if (string.IsNullOrEmpty(searchTerm))
            {
                // If the search box is empty, load all colors (or you can call LoadPage() if paging is needed)
                LoadPage();
            }
            else
            {
                // Filter colors based on the search term
                var filteredProviders = allProviders.Where(c => c.ProviderEmail.ToLower().Contains(searchTerm) || c.ProviderPhone.ToLower().Contains(searchTerm) || c.ProviderCompanyName.ToLower().Contains(searchTerm)).ToList();

                // Load filtered colors into the DataGridView
                LoadProvidersIntoDataGridView(filteredProviders);
            }
        }
        private void LoadProvidersIntoDataGridView(List<Provider> providers)
        {
            // Set up the DataGridView
            DataGridViewProvider.Rows.Clear();
            DataGridViewProvider.Columns.Clear();

            // Define columns
            DataGridViewProvider.Columns.Add("providerId", "Provider Id");
            DataGridViewProvider.Columns.Add("providerEmail", "Email");
            DataGridViewProvider.Columns.Add("providerPhone", "Phone");
            DataGridViewProvider.Columns.Add("providerCompanyName", "Company Name");

            // Populate rows
            foreach (var provider in providers)
            {
                DataGridViewProvider.Rows.Add(provider.providerId,provider.ProviderEmail, provider.ProviderPhone, provider.ProviderCompanyName);
            }
        }

        private int selectedProviderId;  // Store selected categoryId globally for reference
        private void DataGridViewProvider_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (ignoring header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridViewProvider.Rows[e.RowIndex];

                // Validate the cell value
                if (selectedRow.Cells["providerId"].Value != null && int.TryParse(selectedRow.Cells["providerId"].Value.ToString(), out int providerId))
                {
                    selectedProviderId = providerId;
                    GetProviderDetails(selectedProviderId);
                }
                else
                {
                    MessageBox.Show("Invalid provider selected.");
                }
            }
        }

        private async void GetProviderDetails(int providerId)
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/providers/{providerId}", Method.Get);

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var provider = JsonConvert.DeserializeObject<Provider>(response.Content);
                    if (provider != null)
                    {
                        txtEmailEdit.Text = provider.ProviderEmail;
                        txtPhoneEdit.Text = provider.ProviderPhone;
                        txtCompanyEdit.Text = provider.ProviderCompanyName;
                    }
                    else
                    {
                        MessageBox.Show("No provider details found.");
                    }
                }
                else
                {
                    MessageBox.Show("Error fetching provider details.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching provider details: {ex.Message}");
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }
    }
}
