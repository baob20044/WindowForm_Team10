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
    public partial class CustomerManage : UserControl
    {
        private int currentPage = 1; // Current page number
        private int pageSize = 10;  // Number of records per page
        private List<Customer> allCustomers; // Store all data fetched from the API
        public CustomerManage()
        {
            InitializeComponent();
        }

        private async void CustomerManage_Load(object sender, EventArgs e)
        {
            //DataGridViewCustomer.CellClick += DataGridViewCustomer_CellClick;

            DataGridViewCustomer.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewCustomer.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewCustomer.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewCustomer.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewCustomer.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewCustomer.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewCustomer.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewCustomer.ContextMenuStrip = contextMenuStrip1;

            // Enable gridlines
            DataGridViewCustomer.GridColor = System.Drawing.Color.Black; // Set gridline color to black (or any color you prefer)
            DataGridViewCustomer.CellBorderStyle = DataGridViewCellBorderStyle.Single; // Define gridline style

            // Show row and column borders
            DataGridViewCustomer.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Row header borders
            DataGridViewCustomer.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Column header borders


            allCustomers = await FetchCustomersAsync();
            currentPage = 1; // Start from the first page
            LoadPage();
        }
        public class Customer
        {
            public string FullName { get; set; }
            public bool Male { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Email { get; set; }
            public string Avatar { get; set; }
        }
        private async Task<List<Customer>> FetchCustomersAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/Customer", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var customers = JsonConvert.DeserializeObject<List<Customer>>(response.Content);
                    return customers ?? new List<Customer>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch customers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Customer>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Customer>();
            }
        }
        private void LoadPage()
        {
            if (allCustomers == null || allCustomers.Count == 0)
            {
                MessageBox.Show("No customer available to display.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedSizes = allCustomers.Skip(skip).Take(pageSize).ToList();

            // Set up the DataGridView
            DataGridViewCustomer.Rows.Clear();
            DataGridViewCustomer.Columns.Clear();

            // Define columns
            DataGridViewCustomer.Columns.Add("fullName", "Full Name");
            DataGridViewCustomer.Columns.Add("male", "Gender");
            DataGridViewCustomer.Columns.Add("phoneNumber", "Phone");
            DataGridViewCustomer.Columns.Add("address", "Address");
            DataGridViewCustomer.Columns.Add("dateOfBirth", "Birthday");
            DataGridViewCustomer.Columns.Add("email", "Email");
            DataGridViewCustomer.Columns.Add("avatar", "Avatar URL");


            // Populate rows
            foreach (var customer in pagedSizes)
            {
                DataGridViewCustomer.Rows.Add(
                    customer.FullName,                      // Customer's full name
                    customer.Male ? "Male" : "Female",      // Gender (converted to text)
                    customer.PhoneNumber,                   // Phone number
                    customer.Address,                       // Address
                    customer.DateOfBirth.ToString("yyyy-MM-dd"), // Formatted date of birth
                    customer.Email,                         // Email address
                    customer.Avatar                         // Avatar URL
                );
            }



            // Update page information (optional UI labels/buttons)
            lbPageNumber.Text = $"{currentPage} / {Math.Ceiling((double)allCustomers.Count / pageSize)}";

            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < Math.Ceiling((double)allCustomers.Count / pageSize);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < Math.Ceiling((double)allCustomers.Count / pageSize))
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
    }
}
