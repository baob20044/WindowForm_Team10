using RestSharp;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreManagerPro.Components
{
    public partial class ShopItem : Bunifu.UI.WinForms.BunifuUserControl
    {
        public event Action<int> OnShopItemClick; // Event to notify when the item is clicked
        public int ProductId { get; } // Property to hold product ID

        public ShopItem(int productId)
        {
            InitializeComponent();
            ProductId = productId;
            LoadProductData(productId);

            // Attach a click event to the user control
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
                        ItemLabel = product.name;
                        ItemPrice = $"{product.price:N0} VND";

                        // Load the product image based on the first available color
                        var imageUrl = product.colors?.FirstOrDefault()?.images?.FirstOrDefault()?.url;
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            await LoadProductImage(imageUrl);
                        }
                        else
                        {
                            ShowFallbackImage();
                        }
                    }
                    else
                    {
                        ShowFallbackImage();
                    }
                }
                else
                {
                    ShowFallbackImage();
                }
            }
            catch
            {
                ShowFallbackImage();
            }
        }

        private async Task LoadProductImage(string imageUrl)
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
                            pBImage.Image = new Bitmap(ms); // Set the image directly to the PictureBox
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

        private void ShowFallbackImage()
        {
            pBImage.Image = global::StoreManagerPro.Properties.Resources.cart; // Default fallback image
        }

        public string ItemLabel
        {
            get => lbName.Text;
            set => lbName.Text = value;
        }

        public string ItemPrice
        {
            get => lbPrice.Text;
            set => lbPrice.Text = value;
        }

        private void lbName_Click(object sender, EventArgs e)
        {
            // Optional: Handle label click events if needed
        }
    }

    // Model classes
    public class Product
    {
        public int productId { get; set; }
        public string name { get; set; }
        public string subcategoryName { get; set; }
        public decimal price { get; set; }
        public List<Color> colors { get; set; }
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
