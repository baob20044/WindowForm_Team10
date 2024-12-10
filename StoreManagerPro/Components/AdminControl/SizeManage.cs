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
using System.Xml.Linq;

namespace StoreManagerPro.Components.AdminControl
{
    public partial class SizeManage : UserControl
    {
        private int currentPage = 1; // Current page number
        private int pageSize = 10;  // Number of records per page
        private List<Size> allSizes; // Store all data fetched from the API
        public SizeManage()
        {
            InitializeComponent();
        }

        private async void SizeManage_Load(object sender, EventArgs e)
        {
            DataGridViewSize.CellClick += DataGridViewSize_CellClick;

            DataGridViewSize.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewSize.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewSize.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewSize.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewSize.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewSize.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewSize.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewSize.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewSize.ContextMenuStrip = contextMenuStrip1;

            // Enable gridlines
            DataGridViewSize.GridColor = System.Drawing.Color.Black; // Set gridline color to black (or any color you prefer)
            DataGridViewSize.CellBorderStyle = DataGridViewCellBorderStyle.Single; // Define gridline style

            // Show row and column borders
            DataGridViewSize.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Row header borders
            DataGridViewSize.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Column header borders


            allSizes = await FetchSizesAsync();
            currentPage = 1; // Start from the first page
            LoadPage();
        }
        private async void LoadSizesIntoDataGridView()
        {
            var colors = await FetchSizesAsync();

            // Set up the DataGridView
            DataGridViewSize.Rows.Clear();
            DataGridViewSize.Columns.Clear();

            // Define columns
            DataGridViewSize.Columns.Add("sizeId", "Size Id");
            DataGridViewSize.Columns.Add("sizeValue", "Size Value");
        }
        private async Task<List<Size>> FetchSizesAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/sizes", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var sizes = JsonConvert.DeserializeObject<List<Size>>(response.Content);
                    return sizes ?? new List<Size>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch sizes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Size>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching sizes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Size>();
            }
        }


        private void LoadPage()
        {
            if (allSizes == null || allSizes.Count == 0)
            {
                MessageBox.Show("No sizes available to display.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedSizes = allSizes.Skip(skip).Take(pageSize).ToList();

            // Set up the DataGridView
            DataGridViewSize.Rows.Clear();
            DataGridViewSize.Columns.Clear();

            // Define columns
            DataGridViewSize.Columns.Add("sizeId", "Size ID");
            DataGridViewSize.Columns.Add("sizeValue", "Size Value");

            // Populate rows
            foreach (var size in pagedSizes)
            {
                DataGridViewSize.Rows.Add(size.SizeId, size.SizeValue);
            }

            // Update page information (optional UI labels/buttons)
            lbPageNumber.Text = $"{currentPage} / {Math.Ceiling((double)allSizes.Count / pageSize)}";

            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < Math.Ceiling((double)allSizes.Count / pageSize);
        }

        public class Sizes
        {
            public int SizeId { get; set; } // Adjust to match API field names
            public string sizeValue { get; set; }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < Math.Ceiling((double)allSizes.Count / pageSize))
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }
        private void txtClose_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
        }

        private async void txtSave_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string sizeName = txtSize.Text;

            // Validate inputs
            if (string.IsNullOrEmpty(sizeName) || string.IsNullOrEmpty(sizeName))
            {
                MessageBox.Show("Please provide both the size name.");
                return;
            }

            // Create the category object to be sent in the request body
            var size = new Sizes
            {
                sizeValue = sizeName,
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/sizes", Method.Post);
            request.AddJsonBody(size);  // Add the category data as JSON

            try
            {
                // Execute the POST request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    SizeManage_Load(sender, e);
                    MessageBox.Show("Size added successfully!");
                }
                else
                {
                    MessageBox.Show("Error adding size: " + response.ErrorMessage);
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
                var filteredColors = allSizes.Where(c => c.SizeValue.ToLower().Contains(searchTerm)).ToList();

                // Load filtered colors into the DataGridView
                LoadSizesIntoDataGridView(filteredColors);
            }
        }
        private void LoadSizesIntoDataGridView(List<Size> sizes)
        {
            // Set up the DataGridView
            DataGridViewSize.Rows.Clear();
            DataGridViewSize.Columns.Clear();

            // Define columns
            DataGridViewSize.Columns.Add("sizeId", "Size ID");
            DataGridViewSize.Columns.Add("sizeValue", "Size Value");

            // Populate rows
            foreach (var size in sizes)
            {
                DataGridViewSize.Rows.Add(size.SizeId, size.SizeValue);
            }
        }

        private void btnCloseEdit_Click(object sender, EventArgs e)
        {
            flowLayoutEdit.Visible = false;
        }

        private async void btnSaveEdit_Click(object sender, EventArgs e)
        {
            string sizeName = txtEditSize.Text;

            // Validate inputs
            if (string.IsNullOrEmpty(sizeName))
            {
                MessageBox.Show("Please provide the size name.");
                return;
            }
            // Create the category object to be sent in the request body
            var size = new Sizes
            {
                sizeValue = sizeName,
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/sizes/{selectedSizeId}", Method.Put); // Use PUT for updating
            request.AddJsonBody(size);  // Add the color data as JSON

            try
            {
                // Execute the PUT request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Refresh the color data and update UI
                    SizeManage_Load(sender, e);
                    MessageBox.Show("Size updated successfully!");
                }
                else
                {
                    MessageBox.Show("Error updating size: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private int selectedSizeId;  // Store selected categoryId globally for reference
        private void DataGridViewSize_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (ignoring header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridViewSize.Rows[e.RowIndex];

                // Validate the cell value
                if (selectedRow.Cells["sizeId"].Value != null && int.TryParse(selectedRow.Cells["sizeId"].Value.ToString(), out int sizeId))
                {
                    selectedSizeId = sizeId;
                    GetSizeDetails(selectedSizeId);
                }
                else
                {
                    MessageBox.Show("Invalid size selected.");
                }
            }
        }

        private async void GetSizeDetails(int sizeId)
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/sizes/{sizeId}", Method.Get);

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var size = JsonConvert.DeserializeObject<Size>(response.Content);
                    if (size != null)
                    {
                        txtEditSize.Text = size.SizeValue;  // Set the size name in the TextBox
                    }
                    else
                    {
                        MessageBox.Show("No size details found.");
                    }
                }
                else
                {
                    MessageBox.Show("Error fetching size details.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching size details: {ex.Message}");
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }
    }
}
