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
    public partial class CategoryManage : UserControl
    {
        private List<Category> allCategories; // Store all data fetched from API
        private int currentPage = 1;          // Current page number
        private int pageSize = 10;            // Number of records per page

        public CategoryManage()
        {
            InitializeComponent();
        }
        private async void CategoryManage_Load(object sender, EventArgs e)
        {
            DataGridViewSetting();
            allCategories = await FetchCategoriesAsync();
            currentPage = 1; // Start from the first page
            cbTargetCustomer.Items.Add(1);
            cbTargetCustomer.Items.Add(2);
            cbTargetCustomer.Items.Add(3);

            cbTargetCustomer.SelectedIndex = 0; // Default to "1"
            LoadPage();
        }
        public class Subcategory
        {
            public int SubcategoryId { get; set; }
            public string SubcategoryName { get; set; }
            public string Description { get; set; }
            public int CategoryId { get; set; }
        }

        public class Category
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public int TargetCustomerId { get; set; }
            public List<Subcategory> Subcategories { get; set; }
        }
        private void DataGridViewSetting()
        {
            DataGridViewCategory.CellClick += DataGridViewCategory_CellClick;

            DataGridViewCategory.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewCategory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewCategory.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewCategory.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewCategory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewCategory.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewCategory.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewCategory.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewCategory.ContextMenuStrip = contextMenuStrip1;
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
        private async void LoadCategoriesIntoDataGridView()
        {
            var categories = await FetchCategoriesAsync();

            // Ensure DataGridView is cleared before adding data
            DataGridViewCategory.Rows.Clear();
            DataGridViewCategory.Columns.Clear();

            // Define columns
            DataGridViewCategory.Columns.Add("CategoryId", "Category ID");
            DataGridViewCategory.Columns.Add("Name", "Name");
            DataGridViewCategory.Columns.Add("TargetCustomerId", "Target Customer ID");

            // Add rows
            foreach (var category in categories)
            {
                DataGridViewCategory.Rows.Add(category.CategoryId, category.Name, category.TargetCustomerId);
            }
        }

        private void LoadPage()
        {
            if (allCategories == null || allCategories.Count == 0)
                return;

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedData = allCategories.Skip(skip).Take(pageSize).ToList();

            // Clear and set up DataGridView
            DataGridViewCategory.Rows.Clear();
            DataGridViewCategory.Columns.Clear();

            DataGridViewCategory.Columns.Add("CategoryId", "Category ID");
            DataGridViewCategory.Columns.Add("Name", "Name");
            DataGridViewCategory.Columns.Add("TargetCustomerId", "Target Customer ID");

            foreach (var category in pagedData)
            {
                DataGridViewCategory.Rows.Add(category.CategoryId, category.Name, category.TargetCustomerId);
            }

            // Update page number label
            lbPageNumber.Text = $"{currentPage}/{Math.Ceiling((double)allCategories.Count / pageSize)}";

            // Enable/disable navigation buttons
            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < (int)Math.Ceiling((double)allCategories.Count / pageSize);
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPage();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < (int)Math.Ceiling((double)allCategories.Count / pageSize))
            {
                currentPage++;
                LoadPage();
            }
        }

        private void btnAddCategory_Click(object sender, EventArgs e) {
            flowLayoutAdd.Visible = true;
            flowLayoutEdit.Visible = false;
        }

        private void txtClose_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
        }

        private async void txtSave_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string categoryName = txtName.Text;
            int targetCustomerId = (int)cbTargetCustomer.SelectedItem; // Assuming the ComboBox has values set properly (e.g., IDs)

            // Validate inputs
            if (string.IsNullOrEmpty(categoryName) || targetCustomerId == 0)
            {
                MessageBox.Show("Please provide both the category name and target customer.");
                return;
            }

            // Create the category object to be sent in the request body
            var category = new Category
            {
                Name = categoryName,
                TargetCustomerId = targetCustomerId
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/categories", Method.Post);
            request.AddJsonBody(category);  // Add the category data as JSON

            try
            {
                // Execute the POST request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    MessageBox.Show("Category added successfully!");
                }
                else
                {
                    MessageBox.Show("Error adding category: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }

        private void btnCloseEdit_Click(object sender, EventArgs e)
        {
            flowLayoutEdit.Visible = false;
        }

        private int selectedCategoryId;  // Store selected categoryId globally for reference

        // Add the event handler for the DataGridView cell click event
        private void DataGridViewCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (ignoring header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridViewCategory.Rows[e.RowIndex];

                // Retrieve the categoryId from the specific column (assuming 'categoryId' is in the first column, index 0)
                selectedCategoryId = Convert.ToInt32(selectedRow.Cells["categoryId"].Value);

                // Fetch the category details (e.g., Name) for the selected category
                GetCategoryDetails(selectedCategoryId);
            }
        }
        private void GetCategoryDetails(int categoryId)
        {
            // Make an API call to get the category details
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/categories/{categoryId}",Method.Get);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // Assuming your response contains the category data (e.g., Name)
                var category = JsonConvert.DeserializeObject<Category>(response.Content);
                txtEditName.Text = category.Name;  // Set the category name in the TextBox
            }
            else
            {
                MessageBox.Show("Error fetching category details.");
            }
        }

        private async void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // Get the new category name from the TextBox
            string newCategoryName = txtEditName.Text;

            // Validate the new name
            if (string.IsNullOrEmpty(newCategoryName))
            {
                MessageBox.Show("Please enter a category name.");
                return;
            }

            // Create an object for the category with the new name
            var category = new
            {
                Name = newCategoryName
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/categories/{selectedCategoryId}",Method.Put);  // Use PUT for updating

            // Add the updated category data to the request body
            request.AddJsonBody(category);

            try
            {
                // Execute the PUT request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    MessageBox.Show("Category updated successfully!");

                    // Optionally, refresh the data grid or UI to reflect the updated category
                    // e.g., reload data, update grid, etc.
                }
                else
                {
                    MessageBox.Show("Error updating category: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

    }
}
