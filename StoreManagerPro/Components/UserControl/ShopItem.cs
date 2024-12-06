using RestSharp;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using System.IO;
using System.Net.Http;

namespace StoreManagerPro.Components
{
    public partial class ShopItem : Bunifu.UI.WinForms.BunifuUserControl
    {
        public ShopItem(int productId)
        {
            InitializeComponent();
            LoadProductData(productId);
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
                        var imageUrl = product.colors?.Find(c => c.images.Count > 0)?.images[0]?.url ?? "";

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            try
                            {
                                using (HttpClient httpClient = new HttpClient())
                                {
                                    // Tải dữ liệu byte từ URL
                                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                                    // Check if data was returned successfully
                                    if (imageBytes.Length > 0)
                                    {
                                        try
                                        {
                                            // Use Magick.NET to load the .webp image
                                            using (var ms = new MemoryStream(imageBytes))
                                            {
                                                using (var magickImage = new MagickImage(ms))
                                                {
                                                    // Resize image (optional step)
                                                    MagickGeometry geometry = new MagickGeometry(100, 100); // Set the desired size for the image
                                                    magickImage.Resize(geometry);

                                                    // Convert MagickImage to Bitmap
                                                    using (var memStream = new MemoryStream())
                                                    {
                                                        // Write the image to the memorystream
                                                        magickImage.Write(memStream);

                                                        // Create a Bitmap from the memory stream
                                                        pBImage.Image = new System.Drawing.Bitmap(memStream);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception imgEx)
                                        {
                                            // MessageBox.Show($"Error loading image: {imgEx.Message}. Using fallback image.");
                                            pBImage.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                                        }
                                    }
                                    else
                                    {
                                        // MessageBox.Show("No image data received.");
                                        pBImage.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                                    }
                                }
                            }
                            catch (Exception imgEx)
                            {
                                // MessageBox.Show($"Error downloading image: {imgEx.Message}");
                                pBImage.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                            }
                        }
                        else
                        {
                            // MessageBox.Show("No image URL available.");
                            pBImage.Image = global::StoreManagerPro.Properties.Resources.cart; // Fallback image
                        }

                        ItemLabel = product.name;
                        ItemPrice = $"{product.price:N0} VND";
                    }
                    else
                    {
                        // MessageBox.Show("Product data is null or malformed.");
                    }
                }
                else
                {
                    // MessageBox.Show($"Failed to fetch product data.\nStatus: {response.StatusCode}\nError: {response.ErrorMessage}\nContent: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public string ItemLabel
        {
            get { return lbName.Text; }
            set { lbName.Text = value; }
        }

        public string ItemPrice
        {
            get { return lbPrice.Text; }
            set { lbPrice.Text = value; }
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
