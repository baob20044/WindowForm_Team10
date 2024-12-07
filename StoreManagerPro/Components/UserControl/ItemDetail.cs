using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagerPro
{
    public partial class ItemDetail : Bunifu.UI.WinForms.BunifuUserControl
    {
        private Dictionary<string, Color> colorMap; // Map color names to Color objects
        private int productId;  // Store productId for CartItem
        private FlowLayoutPanel flowLayoutCart; // Reference to the parent's FlowLayoutPanel
        public ItemDetail(int productId, FlowLayoutPanel flowLayoutCart = null)
        {
            InitializeComponent();
            this.productId = productId;
            this.flowLayoutCart = flowLayoutCart;

            LoadProductDetails(productId);
            DropdownColor.SelectedIndexChanged += DropdownColor_SelectedIndexChanged; // Attach event handler
        }

        // API product theo productId 
        private async void LoadProductDetails(int productId)
        {
            try
            {
                var productService = new ProductService("http://localhost:5254");
                var product = await productService.GetProductByIdAsync(productId);

                if (product != null)
                {
                    UpdateUIWithProduct(product);
                    await LoadProductImage(product.colors?.FirstOrDefault());
                }
                else
                {
                    ShowFallbackImage();
                }
            }
            catch (Exception ex)
            {
                ShowFallbackImage();
            }
        }

        // Cập nhật UI thanh toán khi thêm xóa sửa cartItem 
        private void UpdateUIWithProduct(Product product)
        {
            lbName.Text = product.name;
            lbPrice.Text = $"{product.price:N0} VND";
            lbDescription.Text = product.description ?? "No description available";
            lbCost.Text = $"Cost: {product.price + product.price*10/100:N0} VND";
            lbInStock.Text = $"Còn {product.inStock} sản phẩm trong kho";

            // Populate colors
            colorMap = new Dictionary<string, Color>();
            DropdownColor.Items.Clear();
            foreach (var color in product.colors ?? new List<Color>())
            {
                DropdownColor.Items.Add(color.name);
                colorMap[color.name] = color;
            }
            if (DropdownColor.Items.Count > 0)
            {
                DropdownColor.SelectedIndex = 0;
            }

            // Populate sizes
            DropdownSize.Items.Clear();
            foreach (var size in product.sizes ?? new List<Size>())
            {
                DropdownSize.Items.Add(size.sizeValue); // Add size values like "S", "M", etc.
            }
            if (DropdownSize.Items.Count > 0)
            {
                DropdownSize.SelectedIndex = 0; // Default to the first size
            }
        }

        // Load ảnh sản phẩm theo màu chọn
        private async Task LoadProductImage(Color color)
        {
            var imageUrl = color?.images?.FirstOrDefault()?.url;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                        if (imageBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                pBProduct.Image = new Bitmap(ms); // Create the Bitmap from the MemoryStream
                            }
                        }
                        else
                        {
                            ShowFallbackImage();
                        }
                    }
                }
                catch
                {
                    ShowFallbackImage();
                }
            }
            else
            {
                ShowFallbackImage();
            }
        }

        private void ShowFallbackImage()
        {
            pBProduct.Image = global::StoreManagerPro.Properties.Resources.cart; // Placeholder fallback image
        }

        // Khi thay đổi dropdowncolor thì đổi cả label và đổi ảnh 
        private async void DropdownColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropdownColor.SelectedIndex >= 0)
            {
                string selectedColorName = DropdownColor.SelectedItem.ToString();

                if (colorMap.TryGetValue(selectedColorName, out var selectedColor))
                {
                    // Update color label
                    lbColor.Text = $"Color: {selectedColor.name}";

                    // Load the first image of the selected color
                    await LoadProductImage(selectedColor);
                }
            }
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            bool productExistsInCart = false;
            string selectedColorName = DropdownColor.SelectedItem?.ToString();
            string selectedSize = DropdownSize.SelectedItem?.ToString();
            foreach (Control control in flowLayoutCart.Controls)
            {
                if (control is CartItem existingCartItem &&
                    existingCartItem.ProductId == productId &&
                    existingCartItem.SelectedColorName == selectedColorName &&
                    existingCartItem.SelectedSize == selectedSize) // Compare color and size
                {
                    existingCartItem.NumericValue += NumericUpDown1.Value;
                    productExistsInCart = true;
                    break;
                }
            }

            if (!productExistsInCart)
            {
                // Add new item to the cart with selected color and size
                CartItem newCartItem = new CartItem(productId, selectedColorName, selectedSize)
                {
                    ItemLabel = lbName.Text,
                    ItemPrice = lbPrice.Text,
                    ProductImage = pBProduct.Image,
                    NumericValue = NumericUpDown1.Value
                };
                flowLayoutCart.Controls.Add(newCartItem);
            }

            var mainPage = this.FindForm() as MainPage;
            mainPage?.UpdateCartTotals();
            MessageBox.Show($"Added {lbName.Text} to the cart.");
        }



        private void btnBuy_Click(object sender, EventArgs e)
        {
            bool productExistsInCart = false;
            string selectedColorName = DropdownColor.SelectedItem?.ToString();
            string selectedSize = DropdownSize.SelectedItem?.ToString();
            foreach (Control control in flowLayoutCart.Controls)
            {
                if (control is CartItem existingCartItem &&
                    existingCartItem.ProductId == productId &&
                    existingCartItem.SelectedColorName == selectedColorName &&
                    existingCartItem.SelectedSize == selectedSize) // Compare color and size
                {
                    existingCartItem.NumericValue += NumericUpDown1.Value;
                    productExistsInCart = true;
                    break;
                }
            }

            if (!productExistsInCart)
            {
                // Add new item to the cart with selected color and size
                CartItem newCartItem = new CartItem(productId, selectedColorName, selectedSize)
                {
                    ItemLabel = lbName.Text,
                    ItemPrice = lbPrice.Text,
                    ProductImage = pBProduct.Image,
                    NumericValue = NumericUpDown1.Value
                };
                flowLayoutCart.Controls.Add(newCartItem);
            }

            var mainPage = this.FindForm() as MainPage;
            mainPage?.UpdateCartTotals();
            mainPage?.ChangeToCartPage();
        }


    }

    // Example Product Service Class (shared logic)
    public class ProductService
    {
        private readonly RestClient _client;

        public ProductService(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var request = new RestRequest($"/api/products/{productId}", Method.Get);
            request.AddHeader("accept", "*/*");
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful
                ? JsonConvert.DeserializeObject<Product>(response.Content)
                : null;
        }
    }

    // Example Models (shared)
    public class Product
    {
        public int productId { get; set; }
        public string name { get; set; }
        public string subcategoryName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public decimal cost { get; set; }
        public int inStock { get; set; } // Add this line
        public List<Color> colors { get; set; }
        public List<Size> sizes { get; set; } 
    }

    public class Size
    {
        private int v1;
        private int v2;

        public Size(int v1, int v2)
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
        public string hexaCode { get; set; }
        public string name { get; set; }
        public List<ImageInfo> images { get; set; }
    }

    public class ImageInfo
    {
        public int imageId { get; set; }
        public string url { get; set; }
        public string alt { get; set; }
        public int productId { get; set; }
        public int colorId { get; set; }
    }
}
