using StoreManagerPro.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagerPro
{
    public partial class MainPage : Form
    {
        private Timer fadeTimer; // Dùng cho chuyển trang

        private List<ShopItem> shopItems; // All items
        private List<ShopItem> shopItemsCategory; // All items
        private List<ShopItem> filteredItems; // Filtered items based on search
        private List<ShopItem> filteredItemsCategory; 
        private int currentPage = 0;      // Current page index
        private int pageSize = 15;         // Number of items per page
        private int totalProduct = 5;

        public MainPage()
        {
            InitializeComponent();
            Icon = new Icon(@"..\..\Resources\Logo-Yody-Yellow-1024x878.ico");

            // Initialize products and load the first page
            InitializeShopItems();
            filteredItems = new List<ShopItem>(shopItems); // Initially, no filtering
            filteredItemsCategory = new List<ShopItem>(shopItemsCategory); // Initially, no filtering
            LoadPage(0);
            UpdateCartTotals();
        }

        // Load toàn bộ ảnh và apply nút click cho ShopItem 
        private void InitializeShopItems()
        {
            shopItems = new List<ShopItem>();
            shopItemsCategory = new List<ShopItem>();
            for (int i = 1; i <= totalProduct; i++) // Inclusive range to handle all products
            {
                var shopItem = new ShopItem(i);
                shopItem.OnShopItemClick += HandleShopItemClick; // Subscribe to the click event
                shopItems.Add(shopItem); // Add to the list

                var shopItemCategory = new ShopItem(i);
                shopItem.OnShopItemClick += HandleShopItemClick; // Subscribe to the click event
                shopItemsCategory.Add(shopItemCategory); // Add to the list
            }
        }


        // Khi chọn product thì hiện sản phẩm
        public void HandleShopItemClick(int productId) 
        {
            // Navigate to the "Product" page and add the ItemDetail
            pages.SetPage("Product");
            ItemDetail detail = new ItemDetail(productId,flowLayoutCart);
            flowLayoutProduct.Controls.Clear(); // Clear existing details
            flowLayoutProduct.Controls.Add(detail); // Add the new detail view
        }

        // Load trang gồm nhiều sản phảm để lựa chọn + load theo trang dung` filteredItems
        private void LoadPage(int pageIndex) /* Home Page */
        {
            flowLayout.Controls.Clear(); // Clear existing controlsy
            // Calculate the range of items to display
            int startIndex = pageIndex * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, filteredItems.Count);

            // Reuse preloaded controls from filtered list
            for (int i = startIndex; i < endIndex; i++)
            {
                flowLayout.Controls.Add(filteredItems[i]);
            }
            for (int i = 0; i < totalProduct; i++)
            {
                flowLayoutFiltered.Controls.Add(filteredItemsCategory[i]);
            }
            // Update navigation buttons
            btnPrevious.Enabled = pageIndex > 0;
            btnNext.Enabled = endIndex < filteredItems.Count;

            int totalPage = (filteredItems.Count % pageSize) == 0 ? filteredItems.Count / pageSize : (filteredItems.Count / pageSize) + 1;
            lbPageNumber.Text = $"{currentPage + 1}/{totalPage}";
        }

        // Trang sau
        private void btnNext_Click(object sender, EventArgs e) /* Home Page */
        {
            if ((currentPage + 1) * pageSize < filteredItems.Count)
            {
                currentPage++;
                LoadPage(currentPage);
            }
        }

        // Trang trước 
        private void btnPrevious_Click(object sender, EventArgs e) /* Home Page */
        {
            if (currentPage > 0)
            {
                currentPage--;
                LoadPage(currentPage);
            }
        }

        // Chức năng tìm kiếm 
        private void txtSearch_TextChanged(object sender, EventArgs e) /* Home Page */
        {
            string searchText = txtSearch.Text.ToLower();

            // Filter the items based on the search text
            filteredItems = shopItems
                .Where(item => item.ItemLabel.ToLower().Contains(searchText)) // Assuming ItemLabel is a property of ShopItem
                .ToList();

            // Reset current page and reload the page
            currentPage = 0;
            LoadPage(currentPage);
        }

        // Chức năng chuyển trang
        private void btnSideMenu_Click(object sender, EventArgs e) 
        {
            
            pages.SetPage(((Control)sender).Text);
        }


        public void ChangeToCartPage() /* Cart Page */
        {
            pages.SetPage("Cart"); // Change the page to the Cart page
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            // This is already handled by the constructor
        }

        // Khi xóa cartItem trong mainpage, tự động cập nhật  
        public void RemoveCartItem(CartItem cartItem) /* Cart Page */
        {
            // Remove the item from the cart
            flowLayoutCart.Controls.Remove(cartItem);

            // Update the cart totals after removing the item
            UpdateCartTotals();
        }
        // Cập nhật giỏ hàng khi click thêm giỏ hàng hoặc mua ngay từ trang ItemDetail
        public void UpdateCartTotals() /* Cart Page */
        {
            decimal totalMoney = 0;
            decimal transportFee = 30000;  // Static transport fee (VND)
            decimal discount = 0.20m;      // 20% discount

            // Calculate the total money based on the items in the cart
            foreach (Control control in flowLayoutCart.Controls)
            {
                if (control is CartItem cartItem)
                {
                    // For each CartItem, calculate the price and quantity
                    decimal price = decimal.Parse(cartItem.ItemPrice.Replace(" VND", "").Replace(",", ""));
                    decimal quantity = cartItem.NumericValue;
                    totalMoney += price * quantity;
                }
            }

            // If the cart is empty, set the totals to zero or default values
            if (totalMoney == 0)
            {
                lbTotalMoney.Text = "0 VND";
                lbDiscount.Text = "0%";
                lbTransportFee.Text = $"0 VND";
                lbTotalOrder.Text = $"0 VND"; // Final total with only transport fee
            }
            else
            {
                // Apply discount and add transport fee
                decimal totalAfterDiscount = totalMoney * (1 - discount);
                decimal finalTotal = totalAfterDiscount + transportFee;

                // Update the labels on MainPage
                lbTotalMoney.Text = $"{totalMoney:N0} VND"; // Total before discount
                lbDiscount.Text = $"{discount * 100}%";     // Discount percentage
                lbTransportFee.Text = $"{transportFee:N0} VND"; // Transport fee
                lbTotalOrder.Text = $"{finalTotal:N0} VND"; // Final total after discount and transport fee
            }
        }


        private void NavigateToLoginPage() /* Setting Page */
        {
            // Initialize the Timer for fade-out
            fadeTimer = new Timer();
            fadeTimer.Interval = 10; // Faster updates for smoother fade
            fadeTimer.Tick += FadeToLogin;
            fadeTimer.Start();
        }
        private void FadeToLogin(object sender, EventArgs e) /* Setting Page */
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.05; // Faster fade with larger decrement
            }
            else
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();

                // Hide the current form before opening the LoginForm
                this.Hide(); // Hide to prevent gaps or visual issues during transition

                // Open the LoginForm after fading out
                LoginForm loginForm = new LoginForm();
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                loginForm.Location = this.Location; // Preserve the form's position
                loginForm.ShowDialog(); // Show LoginForm modally

                // Properly close the current form after showing the LoginForm
                this.Close(); // Close the current form
                this.Dispose(); // Dispose of the form to free resources

                // Ensure the old form is removed from memory and taskbar
                Application.DoEvents(); // Force UI to update and remove the old form from taskbar
            }
        }
        private void lbLogout_Click(object sender, EventArgs e) /* Setting Page */
        {
            NavigateToLoginPage();
        }

        private void ApplyCategoryFilters()
        {
            var selectedColors = new List<string>();
            var selectedSizes = new List<string>();
            decimal minPrice = 0, maxPrice = decimal.MaxValue;

            // Collect selected colors
            if (cbRed.Checked) selectedColors.Add("Red");
            if (cbBlack.Checked) selectedColors.Add("Black");
            if (cbYellow.Checked) selectedColors.Add("Yellow");
            if (cbOrange.Checked) selectedColors.Add("Orange");
            if (cbGray.Checked) selectedColors.Add("Gray");
            if (cbPink.Checked) selectedColors.Add("LightPink");
            if (cbPurple.Checked) selectedColors.Add("Purple");
            if (cbBrown.Checked) selectedColors.Add("Brown");
            if (cbWhite.Checked) selectedColors.Add("White");

            // Collect selected sizes
            if (cbSizeS.Checked) selectedSizes.Add("S");
            if (cbSizeM.Checked) selectedSizes.Add("M");
            if (cbSizeL.Checked) selectedSizes.Add("L");
            if (cbSizeXl.Checked) selectedSizes.Add("XL");

            // Determine price range
            if (cbBelow350.Checked)
            {
                minPrice = 0;
                maxPrice = 349999;
            }
            if (cbMiddle.Checked)
            {
                minPrice = 350000;
                maxPrice = 749999;
            }
            if (cbAbove750.Checked)
            {
                minPrice = 750000;
                maxPrice = decimal.MaxValue;
            }
            if(cbBelow350.Checked && cbMiddle.Checked || cbAbove750.Checked && cbMiddle.Checked || cbBelow350.Checked && cbAbove750.Checked)
            {
                minPrice = decimal.MaxValue;
                maxPrice = decimal.MaxValue;
            }
                // Apply filters to the product list (shopItemsCategory)
                filteredItemsCategory = shopItemsCategory
                .Where(shopItem =>
                    shopItem.LoadedProduct != null && // Ensure the product is loaded
                    (selectedColors.Count == 0 ||
                     selectedColors.All(color => shopItem.LoadedProduct.Colors.Any(c => c.Name == color))) && // Ensure the product contains all selected colors
                    (selectedSizes.Count == 0 ||
                     selectedSizes.All(size => shopItem.LoadedProduct.Sizes.Any(s => s.SizeValue == size))) && // Ensure the product contains all selected sizes
                    shopItem.LoadedProduct.Price >= minPrice &&
                    shopItem.LoadedProduct.Price <= maxPrice)
                .ToList();
        }

        private void LoadFilteredItems()
        {
            flowLayoutFiltered.Controls.Clear(); // Clear previous controls

            // Add filtered items to the flow layout
            foreach (var item in filteredItemsCategory)
            {
                flowLayoutFiltered.Controls.Add(item);
            }
        }

        private void cbBlack_CheckedChanged(object sender, EventArgs e)
        {
            ApplyCategoryFilters();  // Apply the filter logic
            LoadFilteredItems();     // Update the UI with filtered items
        }
    }
}
