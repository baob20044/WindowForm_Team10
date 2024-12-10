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
using static StoreManagerPro.Components.AdminControl.CategoryManage;

namespace StoreManagerPro.Components.AdminControl
{
    public partial class ColorManage : UserControl
    {
        private int currentPage = 1; // Current page number
        private int pageSize = 10;  // Number of records per page
        private List<Color> allColors; // Store all data fetched from the API

        public ColorManage()
        {
            InitializeComponent();
        }

        private async void ColorManage_Load(object sender, EventArgs e)
        {
            DataGridViewColor.CellClick += DataGridViewColor_CellClick;

            DataGridViewColor.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DataGridViewColor.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewColor.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            DataGridViewColor.ColumnHeadersHeight = 40; // Adjust height to accommodate larger fonts
            DataGridViewColor.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridViewColor.DefaultCellStyle.Font = new Font("Arial", 10);
            DataGridViewColor.AllowUserToAddRows = false; // Disable manual row addition
            DataGridViewColor.ReadOnly = true;           // Make DataGridView read-only
            DataGridViewColor.ContextMenuStrip = contextMenuStrip1;

            // Enable gridlines
            DataGridViewColor.GridColor = System.Drawing.Color.Black; // Set gridline color to black (or any color you prefer)
            DataGridViewColor.CellBorderStyle = DataGridViewCellBorderStyle.Single; // Define gridline style

            // Show row and column borders
            DataGridViewColor.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Row header borders
            DataGridViewColor.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Column header borders


            allColors = await FetchColorsAsync();
            currentPage = 1; // Start from the first page
            LoadPage();
        }

        private async void LoadColorsIntoDataGridView()
        {
            var colors = await FetchColorsAsync();

            // Set up the DataGridView
            DataGridViewColor.Rows.Clear();
            DataGridViewColor.Columns.Clear();

            // Define columns
            DataGridViewColor.Columns.Add("ColorId", "Color ID");
            DataGridViewColor.Columns.Add("HexaCode", "Hexa Code");
            DataGridViewColor.Columns.Add("Name", "Name");

            // Add an image column
            var imageColumn = new DataGridViewImageColumn
            {
                Name = "ColorPreview",
                HeaderText = "Color Preview",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            DataGridViewColor.Columns.Add(imageColumn);

            // Populate rows
            foreach (var color in colors)
            {
                Bitmap colorBitmap = CreateBitmapFromHex(color.HexaCode);
                DataGridViewColor.Rows.Add(color.ColorId, color.HexaCode, color.Name, colorBitmap);
            }
        }

        // Fetch colors from the API
        private async Task<List<Color>> FetchColorsAsync()
        {
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/colors", Method.Get);
            request.AddHeader("accept", "application/json");

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var colors = JsonConvert.DeserializeObject<List<Color>>(response.Content);
                    return colors ?? new List<Color>();
                }
                else
                {
                    MessageBox.Show("Failed to fetch colors.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Color>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching colors: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Color>();
            }
        }


        // Generate a bitmap from a hex color code
        private Bitmap CreateBitmapFromHex(string hexCode)
        {
            // Ensure hexCode is not null or empty
            if (string.IsNullOrEmpty(hexCode))
                throw new ArgumentException("Hex code cannot be null or empty.");

            // Validate hex code format
            if (!System.Text.RegularExpressions.Regex.IsMatch(hexCode, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
                throw new ArgumentException("Invalid hex code format. Example of valid format: #FF5733");

            try
            {
                // Remove the '#' character
                string cleanHex = hexCode.TrimStart('#');

                // Expand shorthand hex code (e.g., #RGB to #RRGGBB)
                if (cleanHex.Length == 3)
                {
                    cleanHex = string.Concat(cleanHex.Select(c => $"{c}{c}"));
                }

                // Ensure the hex string is exactly 6 characters long
                if (cleanHex.Length != 6)
                    throw new ArgumentException("Hex code must be exactly 6 characters after the '#' symbol.");

                // Parse RGB values
                byte r = Convert.ToByte(cleanHex.Substring(0, 2), 16);
                byte g = Convert.ToByte(cleanHex.Substring(2, 2), 16);
                byte b = Convert.ToByte(cleanHex.Substring(4, 2), 16);

                // Create a Color object
                System.Drawing.Color color = System.Drawing.Color.FromArgb(255, r, g, b);
                // Create a bitmap
                Bitmap bitmap = new Bitmap(180, 20);

                // Fill the bitmap with the color
                using (Graphics gGraphics = Graphics.FromImage(bitmap))
                {
                    gGraphics.Clear(color);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error creating color from hex code: {hexCode}. Details: {ex.Message}");
            }
        }

        private void LoadPage()
        {
            if (allColors == null || allColors.Count == 0)
                return;

            // Calculate start and end indexes
            int skip = (currentPage - 1) * pageSize;
            var pagedColors = allColors.Skip(skip).Take(pageSize).ToList();

            // Set up the DataGridView
            DataGridViewColor.Rows.Clear();
            DataGridViewColor.Columns.Clear();

            // Define columns
            DataGridViewColor.Columns.Add("ColorId", "Color ID");
            DataGridViewColor.Columns.Add("HexaCode", "Hexa Code");
            DataGridViewColor.Columns.Add("Name", "Name");

            // Add an image column
            var imageColumn = new DataGridViewImageColumn
            {
                Name = "ColorPreview",
                HeaderText = "Color Preview",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            DataGridViewColor.Columns.Add(imageColumn);

            // Populate rows
            foreach (var color in pagedColors)
            {
                Bitmap colorBitmap = CreateBitmapFromHex(color.HexaCode);
                DataGridViewColor.Rows.Add(color.ColorId, color.HexaCode, color.Name, colorBitmap);
            }

            // Update page information (optional UI labels/buttons)
            lbPageNumber.Text = $"{currentPage} / {Math.Ceiling((double)allColors.Count / pageSize)}";

            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < Math.Ceiling((double)allColors.Count / pageSize);
        }



        // Color model
        public class Color
        {
            public int ColorId { get; set; }
            public string HexaCode { get; set; }
            public string Name { get; set; }
            public List<Image> Images { get; set; }
        }

        public class Image
        {
            public int ImageId { get; set; }
            public string Url { get; set; }
            public string Alt { get; set; }
            public int ProductId { get; set; }
            public int ColorId { get; set; }
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < Math.Ceiling((double)allColors.Count / pageSize))
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

        private async void txtSave_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string colorName = txtName.Text;
            string hexaCode = txtHexacode.Text;

            // Validate inputs
            if (string.IsNullOrEmpty(colorName) || string.IsNullOrEmpty(hexaCode))
            {
                MessageBox.Show("Please provide both the color name and Hexacode.");
                return;
            }

            // Create the category object to be sent in the request body
            var color = new Color
            {
                Name = colorName,
                HexaCode = hexaCode
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest("/api/colors", Method.Post);
            request.AddJsonBody(color);  // Add the category data as JSON

            try
            {
                // Execute the POST request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    ColorManage_Load(sender, e);
                    MessageBox.Show("Color added successfully!");
                }
                else
                {
                    MessageBox.Show("Error adding color: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void txtClose_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
        }

        private async void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // Get the name from the text box and selected target customer ID from ComboBox
            string colorName = txtEditName.Text;
            string hexaCode = txtHexacodeEdit.Text;

            // Validate inputs
            if (string.IsNullOrEmpty(colorName) || string.IsNullOrEmpty(hexaCode))
            {
                MessageBox.Show("Please provide both the color name and Hexacode.");
                return;
            }

            // Create the color object to be sent in the request body
            var color = new Color
            {
                ColorId = selectedColorId,  // Set the selected colorId for updating
                Name = colorName,
                HexaCode = hexaCode
            };

            // Initialize RestSharp client and request
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/colors/{selectedColorId}", Method.Put); // Use PUT for updating
            request.AddJsonBody(color);  // Add the color data as JSON

            try
            {
                // Execute the PUT request asynchronously
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Refresh the color data and update UI
                    ColorManage_Load(sender, e);
                    MessageBox.Show("Color updated successfully!");
                }
                else
                {
                    MessageBox.Show("Error updating color: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }


        private void btnCloseEdit_Click(object sender, EventArgs e)
        {
            flowLayoutEdit.Visible = false;
        }

        private int selectedColorId;  // Store selected categoryId globally for reference
        private void DataGridViewColor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a valid row is clicked (ignoring header row)
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = DataGridViewColor.Rows[e.RowIndex];

                // Retrieve the categoryId from the specific column (assuming 'categoryId' is in the first column, index 0)
                selectedColorId = Convert.ToInt32(selectedRow.Cells["colorId"].Value);

                // Fetch the category details (e.g., Name) for the selected category
                GetColorDetails(selectedColorId);
            }
        }
        private async void GetColorDetails(int colorId)
        {
            // Make an API call to get the color details asynchronously
            var client = new RestClient("http://localhost:5254");
            var request = new RestRequest($"/api/colors/{colorId}", Method.Get);

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Assuming your response contains the color data (e.g., Name)
                    var color = JsonConvert.DeserializeObject<Color>(response.Content);
                    txtEditName.Text = color.Name;  // Set the color name in the TextBox
                    txtHexacodeEdit.Text = color.HexaCode;  // Set the HexaCode in the TextBox
                }
                else
                {
                    MessageBox.Show("Error fetching color details.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching color details: {ex.Message}");
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flowLayoutAdd.Visible = false;
            flowLayoutEdit.Visible = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
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
                var filteredColors = allColors.Where(c => c.Name.ToLower().Contains(searchTerm) || c.HexaCode.ToLower().Contains(searchTerm)).ToList();

                // Load filtered colors into the DataGridView
                LoadColorsIntoDataGridView(filteredColors);
            }
        }
        private void LoadColorsIntoDataGridView(List<Color> colors)
        {
            // Set up the DataGridView for displaying filtered colors
            DataGridViewColor.Rows.Clear();
            DataGridViewColor.Columns.Clear();

            // Define columns
            DataGridViewColor.Columns.Add("ColorId", "Color ID");
            DataGridViewColor.Columns.Add("HexaCode", "Hexa Code");
            DataGridViewColor.Columns.Add("Name", "Name");

            // Add an image column
            var imageColumn = new DataGridViewImageColumn
            {
                Name = "ColorPreview",
                HeaderText = "Color Preview",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            DataGridViewColor.Columns.Add(imageColumn);

            // Populate rows with filtered colors
            foreach (var color in colors)
            {
                Bitmap colorBitmap = CreateBitmapFromHex(color.HexaCode);
                DataGridViewColor.Rows.Add(color.ColorId, color.HexaCode, color.Name, colorBitmap);
            }
        }

    }
}
