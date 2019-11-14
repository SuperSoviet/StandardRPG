using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventureFx
{
    public partial class TradingScreen : Form
    {
        public TradingScreen(Player player)
        {
            _currentPlayer = player;

            InitializeComponent();
            //style to display numeric column values
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //populate the datagrid for the players inventory
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;
            // this hidden column holds the item id so we know which item to sell
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });
            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Sell 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });
            // bind the players inventory to the datagridview
            dgvMyItems.DataSource = _currentPlayer.Inventory;
            //when the user clicks on a row call this function
            dgvMyItems.CellClick += dgvMyItems_CellClick;
            // populate the datagrid for the vendors inventory
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;
            //this hidden column holds the item id so we know which item to sell
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });
            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Buy 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });
            // bind the vendors inventory to the datagridview
            dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

            //when the user clicks on a row cfall this function
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // the first column of a datagridview has a columnIndex = 0
            // this is known as a "zero-based" array/collection/list.
            // you start counting with 0.
            //
            // the 5th column (columnindex = 4 ) is the column with the button
            // so, if the player clicked the button column, we will sell an item from that row
            if (e.ColumnIndex == 4)
            {
                //this gets the id value of the item from the hidden 1st column
                // remember, colmnindex = 0 for the first column
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;
                
                // get the item object for the selected item row
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));
                if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                {
                    MessageBox.Show("You cannot sell the" + itemBeingSold.Name);
                }
                else
                {
                    //Remove one of these items from the players inventory
                    _currentPlayer.RemoveItemFromInventory(itemBeingSold);
                    // give the player the gold for the item being sold
                    _currentPlayer.Gold += itemBeingSold.Price;
                }
            }
        }


        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // the 4th column (columnindex = 3 ) has the "Buy 1" button.
            if (e.ColumnIndex == 3)
            {
                // this gets the id value of the item, from the hidden 1st column
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;
                // get the item object for the selected item row
                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));
                //check if the player has enough gold to buy the item
                if (_currentPlayer.Gold >= itemBeingBought.Price)
                {
                    //add one of the items to the players inventory
                    _currentPlayer.AddItemToInventory(itemBeingBought);
                    //remove the gold to pay for the item
                    _currentPlayer.Gold -= itemBeingBought.Price;
                }
                else
                {
                    MessageBox.Show("You do not have enough gold to buy the" + itemBeingBought.Name);
                }
            }
        }

        public Player CurrentPlayer { get; set; }

        private void label1_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Player _currentPlayer;
    }
}