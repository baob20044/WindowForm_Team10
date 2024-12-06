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
        private List<ShopItem> shopItems; // All items
        private List<ShopItem> filteredItems; // Filtered items based on search
        private int currentPage = 0;      // Current page index
        private int pageSize = 15;         // Number of items per page
        private int totalProduct = 28;

        public MainPage()
        {
            InitializeComponent();
            Icon = new Icon(@"..\..\Resources\Logo-Yody-Yellow-1024x878.ico");

            // Initialize products and load the first page
            InitializeShopItems();
            filteredItems = new List<ShopItem>(shopItems); // Initially, no filtering
            LoadPage(0);
        }

        // Load toàn bộ ảnh và apply nút click cho ShopItem 
        private void InitializeShopItems()
        {
            shopItems = new List<ShopItem>();

            for (int i = 1; i <= totalProduct; i++) // Inclusive range to handle all products
            {
                var shopItem = new ShopItem(i);
                shopItem.OnShopItemClick += HandleShopItemClick; // Subscribe to the click event
                shopItems.Add(shopItem); // Add to the list
            }
        }

        // Khi chọn product thì hiện sản phẩm
        private void HandleShopItemClick(int productId)
        {
            // Navigate to the "Product" page and add the ItemDetail
            pages.SetPage("Product");
            ItemDetail detail = new ItemDetail(productId);
            flowLayoutProduct.Controls.Clear(); // Clear existing details
            flowLayoutProduct.Controls.Add(detail); // Add the new detail view
        }

        // Load trang gồm nhiều sản phảm để lựa chọn + load theo trang dung` filteredItems
        private void LoadPage(int pageIndex)
        {
            flowLayout.Controls.Clear(); // Clear existing controls

            // Calculate the range of items to display
            int startIndex = pageIndex * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, filteredItems.Count);

            // Reuse preloaded controls from filtered list
            for (int i = startIndex; i < endIndex; i++)
            {
                flowLayout.Controls.Add(filteredItems[i]);
            }

            // Update navigation buttons
            btnPrevious.Enabled = pageIndex > 0;
            btnNext.Enabled = endIndex < filteredItems.Count;

            int totalPage = (filteredItems.Count % pageSize) == 0 ? filteredItems.Count / pageSize : (filteredItems.Count / pageSize) + 1;
            lbPageNumber.Text = $"{currentPage + 1}/{totalPage}";
        }

        // Trang sau
        private void btnNext_Click(object sender, EventArgs e)
        {
            if ((currentPage + 1) * pageSize < filteredItems.Count)
            {
                currentPage++;
                LoadPage(currentPage);
            }
        }

        // Trang trước 
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                LoadPage(currentPage);
            }
        }

        // Chức năng tìm kiếm 
        private void txtSearch_TextChanged(object sender, EventArgs e)
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

        private void MainPage_Load(object sender, EventArgs e)
        {
            // This is already handled by the constructor
        }
    }
}
