using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace Rental_System
{
    public partial class Bike_List : Form
    {
        private string currentUser;

        public Bike_List(string username)
        {
            InitializeComponent();
            this.currentUser = username;

            this.button2.Click += new EventHandler(this.button2_Click);

            
            this.dataGridView1.CellContentClick += new DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);

            LoadVehicleData();
        }

        private void LoadVehicleData()
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = "SELECT reg, model, color, fuel, transmission, RENTED_BY, picture FROM VEHICLE WHERE type = 'Bike'";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
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
                            dataGridView1.Columns["picture"].Visible = false;
                    }
                }
                ApplyColorCoding();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void ApplyColorCoding()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var rentedBy = row.Cells["RENTED_BY"].Value;

                if (rentedBy == DBNull.Value || string.IsNullOrEmpty(rentedBy?.ToString()))
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }

     
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ViewPhoto")
            {
                try
                {
                    var cellValue = dataGridView1.Rows[e.RowIndex].Cells["picture"].Value;
                    if (cellValue != DBNull.Value)
                        ShowImagePopup((byte[])cellValue);
                    else
                        MessageBox.Show("No image stored for this bike.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading image: " + ex.Message);
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
                    previewForm.Text = "Bike Preview";
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
            catch (Exception ex) { MessageBox.Show("Image error: " + ex.Message); }
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a bike first.");
                return;
            }

            var selectedRow = dataGridView1.SelectedRows[0];
            if (selectedRow.Cells["reg"].Value == null) return;

            string reg = selectedRow.Cells["reg"].Value.ToString();
            string model = selectedRow.Cells["model"].Value.ToString();
            var rentedBy = selectedRow.Cells["RENTED_BY"].Value;

            //  Availability Check
            if (rentedBy != DBNull.Value && !string.IsNullOrEmpty(rentedBy?.ToString()))
            {
                MessageBox.Show("THE VEHICLE IS NOT AVAILABLE FOR RENT");
                return;
            }

            // Set Booking Data
            BookingData.VehicleReg = reg;
            BookingData.VehicleModel = model;
            BookingData.VehicleType = "Bike";
            BookingData.CurrentUser = currentUser;

            //  Navigate
            Renter_Information info = new Renter_Information();
            info.Show();
            this.Close();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(currentUser);
            form3.Show();
            this.Close();
        }

        // Unused
        private void Bike_List_Load(object sender, EventArgs e) { }
        private void Bike_List_Load_1(object sender, EventArgs e) { }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) { }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}