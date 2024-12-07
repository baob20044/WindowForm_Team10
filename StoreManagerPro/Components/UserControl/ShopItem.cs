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
        public Product LoadedProduct { get; private set; } // Expose the product

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
                    LoadedProduct = JsonConvert.DeserializeObject<Product>(response.Content);

                    if (LoadedProduct != null)
                    {
                        // Set product details
                        ItemLabel = LoadedProduct.Name;
                        ItemPrice = $"{LoadedProduct.Price:N0} VND";

                        // Load the product image based on the first available color
                        var imageUrl = LoadedProduct.Colors?.FirstOrDefault()?.Images?.FirstOrDefault()?.Url;
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
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string SubcategoryName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int InStock { get; set; } // Stock quantity
        public List<Color> Colors { get; set; }
        public List<Size> Sizes { get; set; }
    }

    public class Size
    {
        public int SizeId { get; set; }
        public string SizeValue { get; set; } // e.g., "S", "M", "L"

        public Size(int sizeId, string sizeValue)
        {
            SizeId = sizeId;
            SizeValue = sizeValue;
        }
    }

    public class Color
    {
        public int ColorId { get; set; }
        public string HexaCode { get; set; }
        public string Name { get; set; }
        public List<ImageInfo> Images { get; set; }
    }

    public class ImageInfo
    {
        public int ImageId { get; set; }
        public string Url { get; set; }
        public string Alt { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
    }

}
