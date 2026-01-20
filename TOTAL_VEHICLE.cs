using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace Rental_System
{
    public partial class TOTAL_VEHICLE : Form
    {
        private string adminUser;

        public TOTAL_VEHICLE(string username)
        {
            InitializeComponent();
            this.adminUser = username;
            LoadVehicles();
        }

        private void TOTAL_VEHICLE_Load(object sender, EventArgs e) { }

       
        private void LoadVehicles()
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
                            V.owner AS OwnerUsername,
                            U.phone AS OwnerPhone, 
                            U.email AS OwnerEmail
                        FROM VEHICLE V
                        LEFT JOIN USERS U ON V.owner = U.username";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.DataSource = dt;

                        
                        DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                        linkCol.Name = "ViewPhoto";
                        linkCol.HeaderText = "Vehicle Image";
                        linkCol.Text = "View Image";
                        linkCol.UseColumnTextForLinkValue = true;
                        dataGridView1.Columns.Add(linkCol);

                        if (dataGridView1.Columns.Contains("picture"))
                            dataGridView1.Columns["picture"].Visible = false;

                        
                        if (dataGridView1.Columns.Contains("OwnerPhone")) dataGridView1.Columns["OwnerPhone"].HeaderText = "Owner Phone";
                        if (dataGridView1.Columns.Contains("OwnerEmail")) dataGridView1.Columns["OwnerEmail"].HeaderText = "Owner Email";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicles: " + ex.Message);
            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a vehicle to delete.");
                return;
            }

            
            string regNum = dataGridView1.SelectedRows[0].Cells["reg"].Value.ToString();
            string isAvail = dataGridView1.SelectedRows[0].Cells["is_available"].Value.ToString();
            string rentedBy = dataGridView1.SelectedRows[0].Cells["RENTED_BY"].Value.ToString();

          
            bool isRented = (isAvail == "False" || isAvail == "0") && !string.IsNullOrEmpty(rentedBy);

            if (isRented)
            {
                FetchAndShowRenterInfo(rentedBy);
                return;
            }

           
            DialogResult dr = MessageBox.Show($"Delete vehicle '{regNum}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;

            
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string deleteQuery = "DELETE FROM VEHICLE WHERE reg = @reg";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@reg", regNum);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Vehicle deleted successfully.");
                            LoadVehicles();
                        }
                        else
                        {
                            MessageBox.Show("Delete failed. Vehicle not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

     
        private void FetchAndShowRenterInfo(string renterUsername)
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = "SELECT username, phone, email FROM USERS WHERE username = @user";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", renterUsername);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["username"].ToString();
                                string phone = reader["phone"].ToString();
                                string email = reader["email"].ToString();

                                MessageBox.Show(
                                    $"This vehicle is currently being rented by: {name}\n\n" +
                                    $"Renter Details:\n" +
                                    $"Phone: {phone}\n" +
                                    $"Email: {email}\n\n" +
                                    "You cannot delete a vehicle while it is rented.",
                                    "Action Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            else
                            {
                                MessageBox.Show("Vehicle is marked as rented, but user info could not be found.", "Error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching renter info: " + ex.Message);
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            Admin_Dashboard ad = new Admin_Dashboard(adminUser);
            ad.Show();
            this.Close();
        }

       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ViewPhoto")
            {
                if (!dataGridView1.Columns.Contains("picture")) return;

                var cellVal = dataGridView1.Rows[e.RowIndex].Cells["picture"].Value;
                if (cellVal != null && cellVal != DBNull.Value)
                {
                    ShowImagePopup((byte[])cellVal);
                }
                else
                {
                    MessageBox.Show("No image available for this vehicle.");
                }
            }
        }

        private void ShowImagePopup(byte[] imgBytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    Image img = Image.FromStream(ms);
                    Form preview = new Form();
                    preview.StartPosition = FormStartPosition.CenterScreen;
                    preview.Size = new Size(600, 500);
                    preview.Text = "Vehicle Image Preview";

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = img;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    preview.Controls.Add(pb);
                    preview.ShowDialog();
                }
            }
            catch { MessageBox.Show("Invalid image data."); }
        }
    }
}