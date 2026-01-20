using System;
using System.Windows.Forms;
using System.Drawing;         
using System.Data.SqlClient;  
using System.IO;              
using System.Threading.Tasks; 

namespace Rental_System
{
    public partial class Renter_Information : Form
    {
        // Dynamic Controls for Location
        TextBox locationInput = new TextBox();
        Label locationLabel = new Label();

        // Variable to hold the image data from the database
        byte[] currentVehicleImage = null;

        public Renter_Information()
        {
            InitializeComponent();

            
            locationLabel.Text = "Enter Your Location (Optional):";
            locationLabel.AutoSize = true;
            locationLabel.Location = new Point(richTextBox1.Left, richTextBox1.Bottom + 10);
            locationLabel.Font = new Font("Arial", 10, FontStyle.Bold);

            locationInput.Location = new Point(richTextBox1.Left, richTextBox1.Bottom + 35);
            locationInput.Width = richTextBox1.Width;
            locationInput.Font = new Font("Arial", 10);

            // Add controls to form
            this.Controls.Add(locationLabel);
            this.Controls.Add(locationInput);
        }

        private void Renter_Information_Load(object sender, EventArgs e)
        {
           
            richTextBox1.ReadOnly = true;
            richTextBox1.BackColor = Color.White;
            richTextBox1.Font = new Font("Consolas", 10, FontStyle.Regular);

            
            LoadFullVehicleDetails();
        }

        private void LoadFullVehicleDetails()
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    
                    string query = @"
                        SELECT 
                            V.color, 
                            V.fuel, 
                            V.transmission, 
                            V.picture,
                            U.phone AS OwnerPhone,
                            U.email AS OwnerEmail
                        FROM VEHICLE V
                        JOIN USERS U ON V.owner = U.username
                        WHERE V.reg = @reg";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@reg", BookingData.VehicleReg);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1. Get Vehicle Details
                                string color = reader["color"].ToString();
                                string fuel = reader["fuel"].ToString();
                                string trans = reader["transmission"].ToString();

                                // 2. Get Owner Contact Details
                                string ownerPhone = reader["OwnerPhone"].ToString();
                                string ownerEmail = reader["OwnerEmail"].ToString();

                                // 3. Update the Display Box
                                richTextBox1.Text =
                                    "=== VEHICLE DETAILS ===\n\n" +
                                    $"Type:         {BookingData.VehicleType}\n" +
                                    $"Model:        {BookingData.VehicleModel}\n" +
                                    $"Registration: {BookingData.VehicleReg}\n" +
                                    $"Color:        {color}\n" +
                                    $"Fuel:         {fuel}\n" +
                                    $"Transmission: {trans}\n\n" +

                                    "=== OWNER CONTACT INFO ===\n\n" +
                                    $"Phone:        {ownerPhone}\n" +
                                    $"Email:        {ownerEmail}\n\n" +

                                    "=======================\n" +
                                    "Please verify details before confirming.";

                                // 4. Get Image Data
                                if (reader["picture"] != DBNull.Value)
                                {
                                    currentVehicleImage = (byte[])reader["picture"];
                                }
                                else
                                {
                                    currentVehicleImage = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message);
            }
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            if (currentVehicleImage != null)
            {
                ShowImagePopup(currentVehicleImage);
            }
            else
            {
                MessageBox.Show("No image available for this vehicle.");
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
                MessageBox.Show("Image file is corrupted or invalid.\n" + ex.Message);
            }
        }

       
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = @"UPDATE VEHICLE 
                                     SET is_available = 0, RENTED_BY = @renter 
                                     WHERE reg = @reg";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@renter", BookingData.CurrentUser);
                        cmd.Parameters.AddWithValue("@reg", BookingData.VehicleReg);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Renting Confirmed");

                            // 1. Show Done Form
                            Done done = new Done();
                            done.Show();

                            // 2. Hide Current Form
                            this.Hide();

                            // 3. WAIT 5 SECONDS
                            await Task.Delay(5000);

                            // 4. Close Done Form
                            done.Close();

                            // 5. Open Dashboard
                            Form3 dashboard = new Form3(BookingData.CurrentUser);
                            dashboard.Show();

                            // 6. Close Current Form Completely
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Error: Vehicle not found or update failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

    
        private void button2_Click(object sender, EventArgs e)
        {
            Form3 dashboard = new Form3(BookingData.CurrentUser);
            dashboard.Show();
            this.Close();
        }

        // Unused Events
        private void label1_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void viewPhotoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}