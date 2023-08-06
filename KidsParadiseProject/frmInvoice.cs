using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KidsParadiseProject
{
    public partial class frmInvoice : Form
    {
        public frmInvoice()
        {
            InitializeComponent();
        }



        private void frmInvoice_Load(object sender, EventArgs e)
        {
            LoadProduct();

        }

        private void LoadProduct()
        {
            try
            {

                string query = "select g.GameID, g.GameName,g.Price from tblGameZone g inner join tblUnit u on g.UnitID = u.UnitID ";

                using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);



                        // Assuming you have a ComboBox control named "comboBox" in your form
                        dgvProduct.DataSource = dataTable;
                        dgvProduct.Columns[0].Width = 25;
                        dgvProduct.Columns[1].Width = 75;
                        dgvProduct.Columns[2].Width = 75;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // Handle exceptions or display an error message
                // ex.Message will contain the error message
            }
        }



        private DataTable GetDataFromDatabase(string selectedValue)
        {
            // Replace these with your actual database connection details
            //string connectionString = "Data Source=your_server_name;Initial Catalog=your_database_name;User ID=your_username;Password=your_password;";

            DataTable data = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
                {
                    connection.Open();

                    string query = "select g.GameID as 'ID',g.GameName as 'GameName' ,u.UnitName as 'Type',g.UnitID as 'UnitID',g.Price as 'Price' from tblGameZone as g inner join tblUnit as u on g.UnitID = u.UnitID where g.GameID = @ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", selectedValue);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(data);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return data;
        }

        private void txtMinuts_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 127)
            {
                e.Handled = true; // Suppress the input character
            }
        }

        private void CalculateTotalPrice()
        {
            if (decimal.TryParse(txtMinuts.Text, out decimal unitPrice) && int.TryParse(dgvProduct.SelectedRows[0].Cells[2].Value.ToString(), out int quantity))
            {
                decimal totalPrice = unitPrice * quantity;
                txtTotal.Text = totalPrice.ToString();
            }
            else
            {
                // If the entered values are not valid numbers, clear the total price TextBox
                txtTotal.Text = string.Empty;
            }
        }

        private void txtMinuts_TextChanged(object sender, EventArgs e)
        {
            if (txtMinuts.Text != string.Empty)
            {
                CalculateTotalPrice();
            }
        }

        private int CalculateColumnTotal(DataGridView dataGridView, string columnName)
        {
            int total = 0;

            if (dataGridView.Columns.Contains(columnName))
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells[columnName].Value != null &&
                        int.TryParse(row.Cells[columnName].Value.ToString(), out int value))
                    {
                        total += value;
                    }
                }
            }
            else
            {
                // Column not found, handle the error as needed (e.g., throw an exception, return a default value, etc.)
            }

            return total;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lblInvoiceID.Text == "000")
            {
                MessageBox.Show("Please Add New Order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
                {
                    connection.Open();

                    string query = "insert into tblInvoice (InvoiceID,InvTime,InvDate,GameID,Price,Qty) values (@InvoiceID,@InvTime,@InvDate,@GameID,@Price,@Qty)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceID", Convert.ToInt32(lblInvoiceID.Text));
                        command.Parameters.AddWithValue("@InvTime", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@InvDate", DateTime.Now);
                        command.Parameters.AddWithValue("@GameID", dgvProduct.SelectedRows[0].Cells[0].Value.ToString());
                        command.Parameters.AddWithValue("@Price", dgvProduct.SelectedRows[0].Cells[2].Value.ToString());
                        command.Parameters.AddWithValue("@Qty", txtMinuts.Text);
                        command.ExecuteNonQuery();
                    }
                }

                //MessageBox.Show("Data inserted successfully!");
                // Optionally, you can clear the input fields after successful insertion:
                //cmbGame.SelectedIndex = 0;
                //txtPrice.Text = "";
                txtMinuts.Text = "";
                txtTotal.Text = "";
                //txtGameType.Text = "";
                FillBilling();
                int Total = CalculateColumnTotal(dataGridView1, "Total");
                txtGrandTotal.Text = Total.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void FillBilling()
        {

            string query = string.Format("select i.ID,g.GameName,i.Price,i.Qty,i.Price * i.Qty as Total from tblInvoice i inner join tblGameZone g on g.GameID = i.GameID inner join tblUnit u on u.UnitID = g.UnitID where i.InvoiceID = {0}", Convert.ToInt32(lblInvoiceID.Text));

            using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;

                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            string query = "select ISNULL(max(ID),0) as NewIvoiceID from tblInvoice";

            using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    // Execute the query and get the maximum ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to an integer
                    if (result != null && int.TryParse(result.ToString(), out int maxID))
                    {
                        // Display the maximum ID in the TextBox
                        maxID++;
                        lblInvoiceID.Text = "00" + maxID.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Unable to fetch the maximum ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           

            string query = "DELETE FROM tblInvoice WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data has been Deleted");
                        FillBilling();
                    }

                }
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            string query = "select ISNULL(Min(ID),0) as NewIvoiceID from tblInvoice";

            using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    // Execute the query and get the maximum ID
                    object result = command.ExecuteScalar();

                    // Check if the result is not null and is convertible to an integer
                    if (result != null && int.TryParse(result.ToString(), out int maxID))
                    {
                        // Display the maximum ID in the TextBox
                        
                        lblInvoiceID.Text = "00" + maxID.ToString();
                        FillBilling();
                    }
                    else
                    {
                        MessageBox.Show("Unable to fetch the maximum ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int NewID = Convert.ToInt32(lblInvoiceID.Text);
            NewID++;
            lblInvoiceID.Text = "00" + NewID.ToString();
            FillBilling();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int Prev = Convert.ToInt32(lblInvoiceID.Text);
            if (Prev > 0)
            {
                Prev--;
                lblInvoiceID.Text = "00" + Prev.ToString();
                FillBilling();
            }
            else
            {
                MessageBox.Show("This is Last ID");
            }
          
           
        }
    }
}

