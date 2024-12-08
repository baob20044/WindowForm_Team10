using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms.Suite;

namespace StoreManagerPro
{


    public partial class CartItem : Bunifu.UI.WinForms.BunifuUserControl
    {

        public int ProductId { get; } // To hold the product ID
        public string SelectedColorName { get; set; } // Selected color
        public string SelectedSize { get; set; }      // Selected size
        public event Action<int> OnShopItemClick; // Event to notify when the item is clicked
        public CartItem(int productId, string selectedColorName, string selectedSize)
        {
            InitializeComponent();
            ProductId = productId;
            LoadProductData(productId);

            SelectedColorName = selectedColorName;
            SelectedSize = selectedSize;

            this.Click += (s, e) => OnShopItemClick?.Invoke(ProductId);
            foreach (Control control in this.Controls)
            {
                control.Click += (s, e) => OnShopItemClick?.Invoke(ProductId);
            }
        }
        private async void LoadProductData(int productId)
        {
            try
            {
                var client = new RestClient("http://localhost:5254");
                var request = new RestRequest($"/api/products/{productId}", Method.Get);
                request.AddHeader("accept", "*/*");

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var product = JsonConvert.DeserializeObject<Product>(response.Content);

                    if (product != null)
                    {
                        // Set product details
                        lbName.Text = product.name;
                        lbPrice.Text = $"{product.price:N0} VND";

                        // Populate color dropdown
                        if (product.colors != null && product.colors.Count > 0)
                        {
                            DropdownColor.Items.Clear();
                            foreach (var color in product.colors)
                            {
                                DropdownColor.Items.Add(color.name);
                            }

                            // Set selected color based on the selected color name
                            if (!string.IsNullOrEmpty(SelectedColorName) && DropdownColor.Items.Contains(SelectedColorName))
                            {
                                DropdownColor.SelectedItem = SelectedColorName;
                            }
                            else
                            {
                                DropdownColor.SelectedIndex = 0; // Default to the first color
                            }
                        }

                        // Populate size dropdown
                        if (product.sizes != null && product.sizes.Count > 0)
                        {
                            DropdownSize.Items.Clear();
                            foreach (var size in product.sizes)
                            {
                                DropdownSize.Items.Add(size.sizeValue);
                            }

                            // Set selected size based on the selected size
                            if (!string.IsNullOrEmpty(SelectedSize) && DropdownSize.Items.Contains(SelectedSize))
                            {
                                DropdownSize.SelectedItem = SelectedSize;
                            }
                            else
                            {
                                DropdownSize.SelectedIndex = 0; // Default to the first size
                            }
                        }

                        // Handle product image based on selected color
                        string selectedColorName = DropdownColor.SelectedItem?.ToString();
                        var selectedColor = product.colors?.FirstOrDefault(c => c.name == selectedColorName);

                        if (selectedColor != null && selectedColor.images != null && selectedColor.images.Count > 0)
                        {
                            var imageUrl = selectedColor.images.FirstOrDefault()?.url;
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                await LoadProductImage(imageUrl);
                            }
                            else
                            {
                                pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                            }
                        }
                        else
                        {
                            pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                        }
                    }
                }
            }
            catch
            {
                pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
            }
        }

        // Lấy ảnh từ url và cài vào picture box pbProduct 
        private async Task LoadProductImage(string imageUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                    if (imageBytes.Length > 0)
                    {
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            pBProduct.Image = new Bitmap(ms);
                        }
                    }
                    else
                    {
                        pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                    }
                }
            }
            catch
            {
                pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
            }
        }

        // Product Model Classes
        public class Product
        {
            public int productId { get; set; }
            public string name { get; set; }
            public decimal price { get; set; }
            public List<Color> colors { get; set; }
            public List<Sizes> sizes { get; set; }
        }
        public class Sizes
        {
            private int v1;
            private int v2;

            public Sizes(int v1, int v2)
            {
                this.v1 = v1;
                this.v2 = v2;
            }

            public int sizeId { get; set; }
            public string sizeValue { get; set; } // e.g., "S", "M", "L"
        }

        public class Color
        {
            public int colorId { get; set; }
            public string name { get; set; }
            public List<ImageInfo> images { get; set; }
        }

        public class ImageInfo
        {
            public int imageId { get; set; }
            public string url { get; set; }
        }
        public string ItemLabel
        {
            get { return lbName.Text; }
            set { lbName.Text = value; }  // Set product name
        }

        public string ItemPrice
        {
            get { return lbPrice.Text; }
            set { lbPrice.Text = value; }  // Set product price
        }

        public Image ProductImage
        {
            get { return pBProduct.Image; }
            set { pBProduct.Image = value; }  // Set product image
        }
        public decimal NumericValue
        {
            get
            {
                return NumericUpDown1.Value;  // Return the current value of NumericUpDown
            }
            set
            {
                NumericUpDown1.Value = value;  // Set the value of NumericUpDown
            }
        }

        // Update khi xóa 
        private void pBDelete_Click(object sender, EventArgs e)
        {
            // Remove the CartItem from the parent container (flowLayoutCart)
            var mainPage = this.FindForm() as MainPage;
            mainPage?.RemoveCartItem(this); // Ensure that the total money gets updated
        }
        // Updagte khi thay đổi số lượng 
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var mainPage = this.FindForm() as MainPage;
            mainPage?.UpdateCartTotals();
        }
    }
}
