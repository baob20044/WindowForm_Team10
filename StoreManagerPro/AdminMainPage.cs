using StoreManagerPro.Components.AdminControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagerPro
{
    public partial class AdminMainPage : Form
    {
        private AdminHome homeInterface;
        private ProductManage productInterface;
        private CategoryManage categoryInterface;
        private SubcategoryManage subcategoryInterface;
        private ColorManage colorInterface;
        private SizeManage sizeInterface;
        private ProviderManage providerInterface;
        private CustomerManage customerInterface;
        private OrderManage orderInterface;
        private OrderDetailManage orderDetailInterface;
        public AdminMainPage()
        {
            InitializeComponent();
        }
        private void AdminMainPage_Load(object sender, EventArgs e)
        {
            Icon = new Icon(@"..\..\Resources\Logo-Yody-Yellow-1024x878.ico");
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            homeInterface = new AdminHome();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(homeInterface);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            productInterface = new ProductManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(productInterface);
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            categoryInterface = new CategoryManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(categoryInterface);
        }
        private void btnCustomer_Click(object sender, EventArgs e)
        {
            customerInterface = new CustomerManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(customerInterface);
        }

        private void btnSubcategory_Click(object sender, EventArgs e)
        {
            subcategoryInterface = new SubcategoryManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(subcategoryInterface);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorInterface = new ColorManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(colorInterface);
        }

        private void btnSize_Click(object sender, EventArgs e)
        {
            sizeInterface = new SizeManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(sizeInterface);
        }

        private void btnProvider_Click(object sender, EventArgs e)
        {
            providerInterface = new ProviderManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(providerInterface);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            orderInterface = new OrderManage();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.Add(orderInterface);
        }
    }
}
