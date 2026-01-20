using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace Rental_System
{
    public partial class TOTAL_OWNER : Form
    {
        private string adminUser;

        public TOTAL_OWNER(string username)
        {
            InitializeComponent();
            this.adminUser = username;
            LoadOwners();
        }

        private void TOTAL_OWNER_Load(object sender, EventArgs e) { }

        private void LoadOwners()
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = "SELECT * FROM USERS WHERE user_type = 'True' OR user_type = 1";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.DataSource = dt;

                        // Add Link Column for NID
                        DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                        linkCol.Name = "ViewNID";
                        linkCol.HeaderText = "NID Document";
                        linkCol.Text = "View NID";
                        linkCol.UseColumnTextForLinkValue = true;
                        dataGridView1.Columns.Add(linkCol);

                        // Hide internal columns
                        if (dataGridView1.Columns.Contains("picture")) dataGridView1.Columns["picture"].Visible = false;
                        if (dataGridView1.Columns.Contains("password")) dataGridView1.Columns["password"].Visible = false;
                        if (dataGridView1.Columns.Contains("nid")) dataGridView1.Columns["nid"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading owners: " + ex.Message);
            }
        }

       
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an owner to delete.");
                return;
            }

            string ownerToDelete = dataGridView1.SelectedRows[0].Cells["username"].Value.ToString();

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    
                    string checkQuery = @"
                        SELECT V.RENTED_BY, U.email, U.phone 
                        FROM VEHICLE V
                        JOIN USERS U ON V.RENTED_BY = U.username
                        WHERE V.owner = @owner AND V.is_available = 0";

                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@owner", ownerToDelete);

                        using (SqlDataReader reader = cmdCheck.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                         
                                string renterUser = reader["RENTED_BY"].ToString();
                                string renterEmail = reader["email"].ToString();
                                string renterPhone = reader["phone"].ToString(); // Changed to phone

                                MessageBox.Show(
                                    $"THIS USERS VEHICLE IS BEING RENTED BY: {renterUser}\n\n" +
                                    $"Renter Details:\n" +
                                    $"Email: {renterEmail}\n" +
                                    $"Phone: {renterPhone}",
                                    "Cannot Delete Owner", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                return;
                            }
                        }
                    }

                    
                    DialogResult dr = MessageBox.Show(
                        $"Are you sure you want to delete owner '{ownerToDelete}'?\n" +
                        "This will also delete ALL vehicles uploaded by this owner.",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dr == DialogResult.No) return;

                    
                    string deleteVehicles = "DELETE FROM VEHICLE WHERE owner = @owner";
                    using (SqlCommand cmdDelVeh = new SqlCommand(deleteVehicles, conn))
                    {
                        cmdDelVeh.Parameters.AddWithValue("@owner", ownerToDelete);
                        cmdDelVeh.ExecuteNonQuery();
                    }

                   
                    string deleteUser = "DELETE FROM USERS WHERE username = @user";
                    using (SqlCommand cmdDelUser = new SqlCommand(deleteUser, conn))
                    {
                        cmdDelUser.Parameters.AddWithValue("@user", ownerToDelete);
                        int rows = cmdDelUser.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Owner and their vehicles deleted successfully.");
                            LoadOwners(); // Refresh Grid
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
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ViewNID")
            {
                try
                {
                    if (!dataGridView1.Columns.Contains("nid"))
                    {
                        MessageBox.Show("NID data not found (Hidden column missing).");
                        return;
                    }

                    var cellVal = dataGridView1.Rows[e.RowIndex].Cells["nid"].Value;

                    if (cellVal != null && cellVal != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])cellVal;
                        if (imgBytes.Length > 0) ShowImagePopup(imgBytes);
                        else MessageBox.Show("NID file is empty.");
                    }
                    else
                    {
                        MessageBox.Show("No NID document uploaded for this user.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error displaying image: " + ex.Message);
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
                    preview.Text = "NID Document Preview";

                    PictureBox pb = new PictureBox();
                    pb.Dock = DockStyle.Fill;
                    pb.Image = img;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;

                    preview.Controls.Add(pb);
                    preview.ShowDialog();
                }
            }
            catch { MessageBox.Show("Invalid or corrupted NID image data."); }
        }

        // --- BACK BUTTON ---
        private void BtnBack_Click(object sender, EventArgs e)
        {
            Admin_Dashboard ad = new Admin_Dashboard(adminUser);
            ad.Show();
            this.Close();
        }
    }
}