using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace Rental_System
{
    public partial class TOTAL_RENTER : Form
    {
        private string adminUser;

        public TOTAL_RENTER(string username)
        {
            InitializeComponent(); // Loads UI from Designer
            this.adminUser = username;

            // Load data immediately
            LoadRenters();
        }

        private void TOTAL_RENTER_Load(object sender, EventArgs e) { }

        private void LoadRenters()
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    // Select Renters (user_type = 0)
                    string query = "SELECT * FROM USERS WHERE user_type = 'False' OR user_type = 0";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.DataSource = dt;

                      
                        DataGridViewLinkColumn nidLink = new DataGridViewLinkColumn();
                        nidLink.Name = "ViewNID";
                        nidLink.HeaderText = "NID Document";
                        nidLink.Text = "View NID";
                        nidLink.UseColumnTextForLinkValue = true;
                        dataGridView1.Columns.Add(nidLink);

                        DataGridViewLinkColumn licLink = new DataGridViewLinkColumn();
                        licLink.Name = "ViewLicense";
                        licLink.HeaderText = "License Document";
                        licLink.Text = "View License";
                        licLink.UseColumnTextForLinkValue = true;
                        dataGridView1.Columns.Add(licLink);

                        
                        if (dataGridView1.Columns.Contains("nid"))
                            dataGridView1.Columns["nid"].Visible = false;

                        if (dataGridView1.Columns.Contains("licence"))
                            dataGridView1.Columns["licence"].Visible = false;

                        if (dataGridView1.Columns.Contains("picture"))
                            dataGridView1.Columns["picture"].Visible = false;

                        if (dataGridView1.Columns.Contains("password"))
                            dataGridView1.Columns["password"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading renters: " + ex.Message);
            }
        }

      
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            string usernameToDelete = dataGridView1.SelectedRows[0].Cells["username"].Value.ToString();

            DialogResult dr = MessageBox.Show($"Are you sure you want to delete user '{usernameToDelete}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                   
                    string updateVehicles = "UPDATE VEHICLE SET RENTED_BY = NULL, is_available = 1 WHERE RENTED_BY = @user";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateVehicles, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@user", usernameToDelete);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // 2. Delete the User
                    string deleteUser = "DELETE FROM USERS WHERE username = @user";
                    using (SqlCommand cmdDelete = new SqlCommand(deleteUser, conn))
                    {
                        cmdDelete.Parameters.AddWithValue("@user", usernameToDelete);
                        int rows = cmdDelete.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("User deleted successfully.");
                            LoadRenters(); // Refresh list
                        }
                        else
                        {
                            MessageBox.Show("Delete failed. User not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

       
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Header click check

            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            // CASE 1: NID Clicked
            if (colName == "ViewNID")
            {
                LoadAndShowImage(e.RowIndex, "nid", "NID Document");
            }
            // CASE 2: License Clicked
            else if (colName == "ViewLicense")
            {
                LoadAndShowImage(e.RowIndex, "licence", "Driving License");
            }
        }

        
        private void LoadAndShowImage(int rowIndex, string columnName, string title)
        {
            try
            {
                if (!dataGridView1.Columns.Contains(columnName))
                {
                    MessageBox.Show($"Column '{columnName}' not found.");
                    return;
                }

                var cellVal = dataGridView1.Rows[rowIndex].Cells[columnName].Value;

                if (cellVal != null && cellVal != DBNull.Value)
                {
                    byte[] imgBytes = (byte[])cellVal;
                    if (imgBytes.Length > 0)
                    {
                        ShowImagePopup(imgBytes, title);
                    }
                    else
                    {
                        MessageBox.Show("File is empty.");
                    }
                }
                else
                {
                    MessageBox.Show($"No {title} uploaded for this user.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        private void ShowImagePopup(byte[] imgBytes, string title)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    Image img = Image.FromStream(ms);
                    Form preview = new Form();
                    preview.StartPosition = FormStartPosition.CenterScreen;
                    preview.Size = new Size(600, 500);
                    preview.Text = title; // Dynamic Title

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = img;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    preview.Controls.Add(pb);
                    preview.ShowDialog();
                }
            }
            catch { MessageBox.Show("Invalid or corrupted image file."); }
        }


        private void BtnBack_Click(object sender, EventArgs e)
        {
            Admin_Dashboard ad = new Admin_Dashboard(adminUser);
            ad.Show();
            this.Close();
        }
    }
}