namespace SuperAdventureFx {
  partial class TradingScreen {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.lblMyInventory = new System.Windows.Forms.Label();
      this.lblVendorInventory = new System.Windows.Forms.Label();
      this.dgvMyItems = new System.Windows.Forms.DataGridView();
      this.dgvVendorItems = new System.Windows.Forms.DataGridView();
      this.btnClose = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize) (this.dgvMyItems)).BeginInit();
      ((System.ComponentModel.ISupportInitialize) (this.dgvVendorItems)).BeginInit();
      this.SuspendLayout();
      // 
      // lblMyInventory
      // 
      this.lblMyInventory.Location = new System.Drawing.Point(13, 12);
      this.lblMyInventory.Name = "lblMyInventory";
      this.lblMyInventory.Size = new System.Drawing.Size(108, 32);
      this.lblMyInventory.TabIndex = 0;
      this.lblMyInventory.Text = "My Inventory";
      this.lblMyInventory.Click += new System.EventHandler(this.label1_Click);
      // 
      // lblVendorInventory
      // 
      this.lblVendorInventory.Location = new System.Drawing.Point(360, 12);
      this.lblVendorInventory.Name = "lblVendorInventory";
      this.lblVendorInventory.Size = new System.Drawing.Size(144, 36);
      this.lblVendorInventory.TabIndex = 1;
      this.lblVendorInventory.Text = "Vendor\'s Inventory";
      // 
      // dgvMyItems
      // 
      this.dgvMyItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dgvMyItems.ColumnHeadersHeightSizeMode =
        System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvMyItems.Location = new System.Drawing.Point(13, 39);
      this.dgvMyItems.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.dgvMyItems.Name = "dgvMyItems";
      this.dgvMyItems.RowTemplate.Height = 24;
      this.dgvMyItems.Size = new System.Drawing.Size(341, 216);
      this.dgvMyItems.TabIndex = 2;
      // 
      // dgvVendorItems
      // 
      this.dgvVendorItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dgvVendorItems.ColumnHeadersHeightSizeMode =
        System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvVendorItems.Location = new System.Drawing.Point(360, 39);
      this.dgvVendorItems.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.dgvVendorItems.Name = "dgvVendorItems";
      this.dgvVendorItems.RowTemplate.Height = 24;
      this.dgvVendorItems.Size = new System.Drawing.Size(240, 216);
      this.dgvVendorItems.TabIndex = 3;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(525, 259);
      this.btnClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 32);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // TradingScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(612, 302);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.dgvVendorItems);
      this.Controls.Add(this.dgvMyItems);
      this.Controls.Add(this.lblVendorInventory);
      this.Controls.Add(this.lblMyInventory);
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MaximumSize = new System.Drawing.Size(630, 349);
      this.Name = "TradingScreen";
      this.Text = "Trade";
      ((System.ComponentModel.ISupportInitialize) (this.dgvMyItems)).EndInit();
      ((System.ComponentModel.ISupportInitialize) (this.dgvVendorItems)).EndInit();
      this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label lblMyInventory;
    private System.Windows.Forms.Label lblVendorInventory;
    private System.Windows.Forms.DataGridView dgvMyItems;
    private System.Windows.Forms.DataGridView dgvVendorItems;
    private System.Windows.Forms.Button btnClose;
  }
}