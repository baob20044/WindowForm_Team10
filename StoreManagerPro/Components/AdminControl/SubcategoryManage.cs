using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Guna.UI2.Native.WinApi;
using static StoreManagerPro.Components.AdminControl.CategoryManage;

namespace StoreManagerPro.Components.AdminControl
{
    public partial class SubcategoryManage : UserControl
    {
        private int currentPage = 1; // Current page number
        private int pageSize = 10;  // Number of records per page
        private List<Subcategory> allSubcategories; // Store all data fetched from the API
        public SubcategoryManage()
        {
            InitializeComponent();
        }

        private async void SubcategoryManage_Load(object sender, EventArgs e)
        {
            DataGridViewSubcategory.CellClick += DataGridViewSubcategory_CellClick;

            DataGridViewSubcategory.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewSubcategory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewSubcategory.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewSubcategory.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewSubcategory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewSubcategory.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewSubcategory.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewSubcategory.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewSubcategory.ContextMenuStrip = contextMenuStrip1;

            // Enable gridlines
            DataGridViewSubcategory.GridColor = System.Drawing.Color.Black; // Set gridline color to black (or any color you prefer)
            DataGridViewSubcategory.CellBorderStyle = DataGridViewCellBorderStyle.Single; // Define gridline style

            // Show row and column borders
            DataGridViewSubcategory.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Row header borders
            DataGridViewSubcategory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Column header borders


            allSubcategories = await FetchSubcategoriesAsync();
            currentPage = 1; // Start from the first page
            LoadPage();
            PopulateCategoryComboBox();
        }
        public class Subcategory
        {
            public int SubcategoryId { get; set; }
            public string SubcategoryName { get; set; }
            public string Description { get; set; }
            public int CategoryId { get; set; }
        }

        private async void LoadSubcategoriesIntoDataGridView()
        {
            var subcategories = await FetchSubcategoriesAsync();

            // Set up the DataGridView
            DataGridViewSubcategory.Rows.Clear();
            DataGridViewSubcategory.Columns.Clear();

            // Define columns
            DataGridViewSubcategory.Columns.Add("subcategoryId", "Subcategory ID");
            DataGridViewSubcategory.Columns.Add("subcategoryName", "Subcategory Name");
            DataGridViewSubcategory.Columns.Add("description", "Description");
            DataGridViewSubcategory.Columns.Add("categoryId", "Category ID");
        }
        private async Task<List<Subcategory>> FetchSubcategoriesAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/subcategories", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var sizes = JsonConvert.DeserializeObject<List<Subcategory>>(response.Content);
                    return sizes ?? new List<Subcategory>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch subcategories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Subcategory>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching subcategories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Subcategory>();
            }
        }
        private void LoadPage()
        {
            if (allSubcategories == null || allSubcategories.Count == 0)
            {
                MessageBox.Show("No subcategory available to display.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedSizes = allSubcategories.Skip(skip).Take(pageSize).ToList();

            // Set up the DataGridView
            DataGridViewSubcategory.Rows.Clear();
            DataGridViewSubcategory.Columns.Clear();

            // Define columns
            DataGridViewSubcategory.Columns.Add("subcategoryId", "Subcategory ID");
            DataGridViewSubcategory.Columns.Add("subcategoryName", "Subcategory Name");
            DataGridViewSubcategory.Columns.Add("description", "Description");
            DataGridViewSubcategory.Columns.Add("categoryId", "Category ID");

            // Populate rows
            foreach (var subcategory in pagedSizes)
            {
                DataGridViewSubcategory.Rows.Add(
                    subcategory.SubcategoryId,
                    subcategory.SubcategoryName,
                    subcategory.Description,
                    subcategory.CategoryId
                );
            }


            // Update page information (optional UI labels/buttons)
            lbPageNumber.Text = $"{currentPage} / {Math.Ceiling((double)allSubcategories.Count / pageSize)}";

            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < Math.Ceiling((double)allSubcategories.Count / pageSize);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < Math.Ceiling((double)allSubcategories.Count / pageSize))
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

        private void btnCloseEdit_Click(object sender, EventArgs e)
        {
            flowLayoutEdit.Visible = false;
        }

        private async void txtSave_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string subcategoryName = txtName.Text;
            string description = txtDescription.Text;
            int categoryId = (int)cbCategoryId.SelectedItem; 
            // Validate inputs
            if (string.IsNullOrEmpty(subcategoryName) || string.IsNullOrEmpty(description) || categoryId == 0)
            {
                MessageBox.Show("Please provide the subcategory name, description and categoryId.");
                return;
            }

            // Create the category object to be sent in the request body
            var subcategory = new Subcategory
            {
                CategoryId = categoryId,
                SubcategoryName = subcategoryName,
                Description = description,
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/subcategories", Method.Post);
            request.AddJsonBody(subcategory);  // Add the category data as JSON

            try
            {
                // Execute the POST request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    SubcategoryManage_Load(sender, e);
                    MessageBox.Show("Subcategory added successfully!");
                    flowLayoutAdd.Visible = false;
                }
                else
                {
                    MessageBox.Show("Error adding subcategory: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        private async Task<List<Category>> FetchCategoriesAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/categories", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var categories = JsonConvert.DeserializeObject<List<Category>>(response.Content);
                    return categories ?? new List<Category>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Category>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Category>();
            }
        }

        private async void PopulateCategoryComboBox()
        {
            var categories = await FetchCategoriesAsync();

            if (categories != null && categories.Count > 0)
            {
                cbCategoryId.Items.Clear(); // Clear existing items if any

                foreach (var category in categories)
                {
                    // Add the categoryId to the ComboBox
                    cbCategoryId.Items.Add(category.CategoryId);
                }
            }
            else
            {
                MessageBox.Show("No categories available to load.");
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
                var filteredSizes = allSubcategories.Where(c => c.SubcategoryName.ToLower().Contains(searchTerm)).ToList();

                // Load filtered colors into the DataGridView
                LoadSubcategoriesintoDataGridView(filteredSizes);
            }
        }
        private void LoadSubcategoriesintoDataGridView(List<Subcategory> subcategories)
        {
            // Set up the DataGridView
            DataGridViewSubcategory.Rows.Clear();
            DataGridViewSubcategory.Columns.Clear();

            // Define columns
            DataGridViewSubcategory.Columns.Add("subcategoryId", "Subcategory ID");
            DataGridViewSubcategory.Columns.Add("subcategoryName", "Subcategory Name");
            DataGridViewSubcategory.Columns.Add("description", "Description");
            DataGridViewSubcategory.Columns.Add("categoryId", "Category ID");

            // Populate rows
            foreach (var subcategory in subcategories)
            {
                DataGridViewSubcategory.Rows.Add(subcategory.SubcategoryId,subcategory.SubcategoryName,subcategory.Description,subcategory.CategoryId);
            }
        }

        private async void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string subcategoryName = txtSubNameEdit.Text;
            string description = txtDescriptionEdit.Text;
            // Validate inputs
            if (string.IsNullOrEmpty(subcategoryName) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please provide the subcategory name, description.");
                return;
            }

            // Create the category object to be sent in the request body
            var subcategory = new Subcategory
            {
                SubcategoryName = subcategoryName,
                Description = description,
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/subcategories/{selectedSubcategoryId}", Method.Put); // Use PUT for updating
            request.AddJsonBody(subcategory);  // Add the color data as JSON

            try
            {
                // Execute the PUT request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Refresh the color data and update UI
                    SubcategoryManage_Load(sender, e);
                    MessageBox.Show("Subcategory updated successfully!");
                }
                else
                {
                    MessageBox.Show("Error updating Subcategory: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
        private int selectedSubcategoryId;  // Store selected categoryId globally for reference
        private void DataGridViewSubcategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (ignoring header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridViewSubcategory.Rows[e.RowIndex];

                // Validate the cell value
                if (selectedRow.Cells["subcategoryId"].Value != null && int.TryParse(selectedRow.Cells["subcategoryId"].Value.ToString(), out int subcategoryId))
                {
                    selectedSubcategoryId = subcategoryId;
                    GetSubcategoryDetails(selectedSubcategoryId);
                }
                else
                {
                    MessageBox.Show("Invalid subcategory selected.");
                }
            }
        }

        private async void GetSubcategoryDetails(int subcategoryId)
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/subcategories/{subcategoryId}", Method.Get);

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var subcategory = JsonConvert.DeserializeObject<Subcategory>(response.Content);
                    if (subcategory != null)
                    {
                        txtSubNameEdit.Text = subcategory.SubcategoryName;
                        txtDescriptionEdit.Text = subcategory.Description;
                    }
                    else
                    {
                        MessageBox.Show("No subcategory details found.");
                    }
                }
                else
                {
                    MessageBox.Show("Error fetching subcategory details.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching subcategory details: {ex.Message}");
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }
    }
}
