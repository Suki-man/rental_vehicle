using System;
using System.Windows.Forms;
using System.Data;             
using System.Data.SqlClient;    
using System.IO;                
using System.Drawing;           

namespace Rental_System
{
    public partial class Book_Request : Form
    {
        private string loggedInOwner;

        public Book_Request(string username)
        {
            InitializeComponent();
            this.loggedInOwner = username;
        }

       
        private void Book_Request_Load(object sender, EventArgs e)
        {
            LoadUserVehicles();
        }

     
        private void LoadUserVehicles()
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                  
                    string query = @"
                        SELECT 
                            V.reg, 
                            V.model, 
                            V.type, 
                            V.color, 
                            V.fuel, 
                            V.transmission, 
                            V.is_available, 
                            V.RENTED_BY, 
                            V.picture,
                            U.phone AS RenterPhone,
                            U.email AS RenterEmail
                        FROM VEHICLE V
                        LEFT JOIN USERS U ON V.RENTED_BY = U.username
                        WHERE V.owner = @owner";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@owner", loggedInOwner);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dataGridView1.DataSource = dt;

                    

                            
                            if (!dataGridView1.Columns.Contains("ViewPhoto"))
                            {
                                DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                                linkCol.Name = "ViewPhoto";
                                linkCol.HeaderText = "Image";
                                linkCol.Text = "View Photo";
                                linkCol.UseColumnTextForLinkValue = true;
                                dataGridView1.Columns.Add(linkCol);
                            }

                          
                            if (dataGridView1.Columns.Contains("picture"))
                            {
                                dataGridView1.Columns["picture"].Visible = false;
                            }

                            if (dataGridView1.Columns.Contains("RenterPhone"))
                                dataGridView1.Columns["RenterPhone"].HeaderText = "Renter Phone";

                            if (dataGridView1.Columns.Contains("RenterEmail"))
                                dataGridView1.Columns["RenterEmail"].HeaderText = "Renter Email";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicles: " + ex.Message);
            }
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a vehicle to delete.");
                return;
            }

          
            string regNumber = dataGridView1.SelectedRows[0].Cells["reg"].Value.ToString();

        
            object availableVal = dataGridView1.SelectedRows[0].Cells["is_available"].Value;
            bool isAvailable = (availableVal != DBNull.Value) && Convert.ToBoolean(availableVal);

            // DELETION CHECK 
            if (!isAvailable)
            {
              
                string renter = dataGridView1.SelectedRows[0].Cells["RENTED_BY"].Value.ToString();
                if (string.IsNullOrEmpty(renter)) renter = "SOMEONE";

                MessageBox.Show("YOUR VEHICLE IS CURRENTLY RENTED BY: " + renter + "\nYou cannot delete it while it is rented.");
                return;
            }

            // Confirm Delete
            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this vehicle?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            // Perform Delete
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = "DELETE FROM VEHICLE WHERE reg = @reg AND owner = @owner";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@reg", regNumber);
                        cmd.Parameters.AddWithValue("@owner", loggedInOwner);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Entry deleted");
                            LoadUserVehicles();
                        }
                        else
                        {
                            MessageBox.Show("Delete failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting vehicle: " + ex.Message);
            }
        }

       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ViewPhoto")
            {
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells["picture"].Value;

                if (cellValue != DBNull.Value)
                {
                    byte[] imgBytes = (byte[])cellValue;
                    ShowImagePopup(imgBytes);
                }
                else
                {
                    MessageBox.Show("No image uploaded for this vehicle.");
                }
            }
        }

        // ================= HELPER: SHOW IMAGE POPUP =================
        private void ShowImagePopup(byte[] imgBytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    Image vehicleImage = Image.FromStream(ms);
                    Form previewForm = new Form();
                    previewForm.Text = "Vehicle Preview";
                    previewForm.Size = new Size(600, 500);
                    previewForm.StartPosition = FormStartPosition.CenterScreen;

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = vehicleImage;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    previewForm.Controls.Add(pb);
                    previewForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not display image.\nError: " + ex.Message);
            }
        }

        // ================= BACK BUTTON (Button 1) =================
        private void button1_Click(object sender, EventArgs e)
        {
            O_Vehicle_Updates dash = new O_Vehicle_Updates(loggedInOwner);
            dash.Show();
            this.Close();
        }

        // Unused Events
        private void buttonAccept_Click(object sender, EventArgs e) { }
        private void buttonReject_Click(object sender, EventArgs e) { }
        private void Book_Request_Load_1(object sender, EventArgs e) { LoadUserVehicles(); }
    }
}