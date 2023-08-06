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
    public partial class frmCreateGame : Form
    {
        public frmCreateGame()
        {
            InitializeComponent();
        }

        private void frmCreateGame_Load(object sender, EventArgs e)
        {
            LoadComboBox();
            dataGridView1.DataSource = GetDataFromDatabase();
        }


        private void LoadComboBox()
        {
            try
            {
               
                string query = "SELECT * FROM tblUnit";

                using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Clear the ComboBox before data binding to avoid duplicates
                        cmbCodeType.Items.Clear();

                        // Add the "Please Select" item as the first item in the ComboBox
                        cmbCodeType.Items.Add("Please Select");

                        // Assuming you have a ComboBox control named "comboBox" in your form
                        cmbCodeType.DataSource = dataTable;
                        cmbCodeType.DisplayMember = "UnitName"; // The column name you want to display in the ComboBox
                        cmbCodeType.ValueMember = "UnitID"; // The column name you want to use as the value for each item
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string GameName = txtGameName.Text; // Assuming you have a TextBox named "txtName" to input the name
            int UnitID = (int)cmbCodeType.SelectedValue;
            int Price = int.Parse(txtGamePrice.Text); // Assuming you have a TextBox named "txtAge" to input the age

            string selectedValue = cmbCodeType.SelectedValue.ToString();

            if (selectedValue == "Please Select")
            {
                MessageBox.Show("Please select a valid option from the dropdown.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method and do not proceed further
            }
            else if (GameName == string.Empty)
            {
                MessageBox.Show("Please Enter a Game Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method and do not proceed further
            }
            else if (UnitID == 0)
            {
                MessageBox.Show("Please select a valid option from the dropdown.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method and do not proceed further
            }
            else if (Price == 0)
            {
                MessageBox.Show("Please Enter Price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method and do not proceed further
            }


          
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
                {
                    connection.Open();

                    string query = "INSERT INTO tblGameZone (GameName, UnitID,Price) VALUES (@GameName, @UnitID,@Price)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GameName", GameName);
                        command.Parameters.AddWithValue("@UnitID", UnitID);
                        command.Parameters.AddWithValue("@Price", Price);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data inserted successfully!");
                // Optionally, you can clear the input fields after successful insertion:
                txtGameName.Text = "";
                txtGamePrice.Text = "";
                dataGridView1.DataSource = GetDataFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtGamePrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 127)
            {
                e.Handled = true; // Suppress the input character
            }
        }

        private DataTable GetDataFromDatabase()
        {
            
            string query = "select g.GameID as 'ID',g.GameName as 'Game Name' ,u.UnitName as 'Type',g.Price as 'Price' from tblGameZone as g inner join tblUnit as u on g.UnitID = u.UnitID";

            using (SqlConnection connection = new SqlConnection(ConnectionString.ConnectionStrings))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }
    }
}
