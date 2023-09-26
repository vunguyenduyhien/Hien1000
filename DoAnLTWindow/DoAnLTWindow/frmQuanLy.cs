using DoAnLTWindow.DAO;
using DoAnLTWindow.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnLTWindow
{
    public partial class frmQuanLy : Form
    {
        private Account loginAcc;
        public Account LoginAcc
        {
            get
            {
                return loginAcc;
            }
            set
            {
                loginAcc = value;
                typeAcc(loginAcc.Type);
            }
        }
        public frmQuanLy(Account acc)
        {
            InitializeComponent();
            this.LoginAcc = acc;
            loadTable();
            loadCategory();
        }
        #region METHODS
        void typeAcc(int type)
        {
            mnuAdmin.Enabled = type == 1;
            mnuTaiKhoan.Text += " (" + LoginAcc.Displayname + ")";
        }
        void loadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.getListCategory();
            cboCategory.DataSource = listCategory;
            cboCategory.DisplayMember = "NAME";
        }
        void loadFoodByCategory(int id)
        {
            List<Food> listFood = FoodDAO.Instance.getFoodList(id);
            cboFood.DataSource = listFood;
            cboFood.DisplayMember = "NAME";
        }
        void loadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.loadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button()
                {
                    Width = TableDAO.TableWidth,
                    Height = TableDAO.TableHeight
                };
                btn.Text = item.Name + Environment.NewLine + "(" + item.Status + ")";
                btn.Click += btn_Click;
                btn.Tag = item;
                if (item.Status == "Trống")
                {
                    btn.BackColor = Color.Green;
                }
                else
                {
                    btn.BackColor = Color.Red;
                }
                flpTable.Controls.Add(btn);
            }
        }
        void showBill(int id)
        {
            lvwBill.Items.Clear();
            List<Menu> listbilldetail = MenuDAO.Instance.getListMenu(id);
            float TotalPrice = 0;
            foreach (Menu item in listbilldetail)
            {
                ListViewItem lvwItem = new ListViewItem(item.FoodName.ToString());
                lvwItem.SubItems.Add(item.Count.ToString());
                lvwItem.SubItems.Add(item.Price.ToString());
                lvwItem.SubItems.Add(item.TotalPrice.ToString());
                TotalPrice += item.TotalPrice;
                lvwBill.Items.Add(lvwItem);
            }
            //CultureInfo culture = new CultureInfo("vi-VN");
            txtTotalPrice.Text = TotalPrice.ToString();
            
        }

        #endregion
        #region EVENTS
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lvwBill.Tag = (sender as Button).Tag;
            showBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAccProfile f = new frmAccProfile(LoginAcc);
            
            f.ShowDialog();
        }      
        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmAdmin f = new frmAdmin();
            f.LoginAccount = LoginAcc;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.ShowDialog();
        }
        void f_InsertFood(object sender, EventArgs e)
        {
            loadFoodByCategory((cboCategory.SelectedItem as Category).ID);
            if (lvwBill.Tag != null)
            {
                showBill((lvwBill.Tag as Table).ID);
            }
            
        }
        void f_DeleteFood(object sender, EventArgs e)
        {
            loadFoodByCategory((cboCategory.SelectedItem as Category).ID);
            if (lvwBill.Tag != null)
            {
                showBill((lvwBill.Tag as Table).ID);
            }
            loadTable();
        }
        void f_UpdateFood(object sender, EventArgs e)
        {
            loadFoodByCategory((cboCategory.SelectedItem as Category).ID);
            if (lvwBill.Tag != null)
            {
                showBill((lvwBill.Tag as Table).ID);
            }
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cbo = sender as ComboBox;
            if (cbo.SelectedItem == null)
            {
                return;
            }
            Category selected = cbo.SelectedItem as Category;
            id = selected.ID;
            loadFoodByCategory(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            
            Table table = lvwBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn trước khi thêm");
                return;
            }
            int idbill = BillDAO.Instance.getUncheckBill(table.ID);
            int idfood = (cboFood.SelectedItem as Food).ID;
            int count = (int)updFoodCount.Value;
            
            if (idbill == -1)
            {
                BillDAO.Instance.insertBill(table.ID);
                BillDetailDAO.Instance.insertBillDetail(BillDAO.Instance.getMaxIdBill(), idfood, count);
            }
            else
            {
                BillDetailDAO.Instance.insertBillDetail(idbill, idfood, count);
            }
            showBill(table.ID);
            loadTable();        
        }
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            Table table = lvwBill.Tag as Table;
            int idbill = BillDAO.Instance.getUncheckBill(table.ID);
            double totalPrice = Convert.ToDouble(txtTotalPrice.Text);
            if (idbill != -1)
            {
                if (MessageBox.Show("Thanh toán hóa đơn cho bàn " + table.Name, "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.checkOut(idbill, (float)totalPrice);
                    showBill(table.ID);
                    loadTable();
                }
            }
        }

        #endregion


    }

}
