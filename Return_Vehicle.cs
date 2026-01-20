using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient; // Required for Database
using System.Drawing;
using System.IO; // Required for Image handling

namespace Rental_System
{
    public partial class Return_Vehicle : Form
    {
        private string currentUser;

        public Return_Vehicle(string username)
        {
            InitializeComponent();
            this.currentUser = username;

            // Mapping buttons (Button1 = Return, Button2 = Back)
            this.button1.Click += new EventHandler(this.buttonReturn_Click);
            this.button2.Click += new EventHandler(this.buttonBack_Click);

            this.dataGridView1.CellContentClick += new DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);

            LoadRentedVehicles();
        }

        
        private void LoadRentedVehicles()
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
                            V.picture,
                            U.name AS OwnerName,
                            U.phone AS OwnerPhone,
                            U.email AS OwnerEmail
                        FROM VEHICLE V
                        LEFT JOIN USERS U ON V.owner = U.username
                        WHERE V.RENTED_BY = @user";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", currentUser);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dataGridView1.DataSource = null;
                            dataGridView1.Columns.Clear();
                            dataGridView1.DataSource = dt;

                           
                            DataGridViewLinkColumn linkCol = new DataGridViewLinkColumn();
                            linkCol.Name = "ViewPhoto";
                            linkCol.HeaderText = "Image";
                            linkCol.Text = "View Photo";
                            linkCol.UseColumnTextForLinkValue = true;
                            dataGridView1.Columns.Add(linkCol);

                            if (dataGridView1.Columns.Contains("picture"))
                            {
                                dataGridView1.Columns["picture"].Visible = false;
                            }

                            if (dataGridView1.Columns.Contains("OwnerName"))
                                dataGridView1.Columns["OwnerName"].HeaderText = "Owner Name";

                            if (dataGridView1.Columns.Contains("OwnerPhone"))
                                dataGridView1.Columns["OwnerPhone"].HeaderText = "Owner Phone";

                            if (dataGridView1.Columns.Contains("OwnerEmail"))
                                dataGridView1.Columns["OwnerEmail"].HeaderText = "Owner Email";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rentals: " + ex.Message);
            }
        }

      
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select the vehicle you want to return.");
                return;
            }

            string reg = dataGridView1.SelectedRows[0].Cells["reg"].Value.ToString();
            string model = dataGridView1.SelectedRows[0].Cells["model"].Value.ToString();

            DialogResult confirm = MessageBox.Show($"Are you sure you want to return the {model} ({reg})?", "Confirm Return", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = @"UPDATE VEHICLE 
                                     SET is_available = 1, RENTED_BY = NULL 
                                     WHERE reg = @reg";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@reg", reg);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("VEHICLE RETURNED");
                            LoadRentedVehicles(); // Refresh grid
                        }
                        else
                        {
                            MessageBox.Show("Return failed. Vehicle not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        
        private void buttonBack_Click(object sender, EventArgs e)
        {
            Form3 dashboard = new Form3(currentUser);
            dashboard.Show();
            this.Close();
        }

       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ViewPhoto")
            {
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells["picture"].Value;
                if (cellValue != DBNull.Value)
                {
                    ShowImagePopup((byte[])cellValue);
                }
                else
                {
                    MessageBox.Show("No image available.");
                }
            }
        }

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
                MessageBox.Show("Image Error: " + ex.Message);
            }
        }

        private void Form4_Load(object sender, EventArgs e) { }
        private void Return_Vehicle_Load(object sender, EventArgs e) { }
    }
}
