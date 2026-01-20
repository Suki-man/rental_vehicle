using System;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;
using System.Diagnostics;

namespace Rental_System
{
    public partial class Add_Vehicles : Form
    {
        private string imagePath = "";
        private string loggedInOwner;

        
        private LinkLabel viewImageLink = new LinkLabel();

     
        public Add_Vehicles(string username)
        {
            InitializeComponent();
            this.loggedInOwner = username;

            
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            // Vehicle Types
            comboBox1.Items.AddRange(new string[] { "Car", "Bike", "Pickup", "Cycle" });

            // Fuel Types 
           
            comboBox2.Items.AddRange(new string[] { "None", "Petrol", "Diesel", "Electric", "Hybrid" });

            
            comboBox3.Items.AddRange(new string[] { "None", "Manual", "Auto" });

            
            SetupFloating(label2, textBox1); // Reg No
            SetupFloating(label3, textBox2); // Color
            SetupFloating(label4, textBox3); // Model

            // view color link
            viewImageLink.Text = "View Selected Photo";
            viewImageLink.AutoSize = true;
            viewImageLink.Location = new Point(textBox4.Left, textBox4.Bottom + 5);
            viewImageLink.Visible = false;
            viewImageLink.BackColor = Color.Transparent;
            viewImageLink.LinkClicked += ViewImageLink_LinkClicked;

            this.Controls.Add(viewImageLink);
            viewImageLink.BringToFront();
        }

       
        private void ViewImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    Process.Start(imagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot open image: " + ex.Message);
                }
            }
        }

        private void SetupFloating(Label lbl, TextBox txt)
        {
            lbl.BackColor = System.Drawing.Color.Transparent;
            lbl.BringToFront();
            lbl.Visible = string.IsNullOrWhiteSpace(txt.Text);
            txt.TextChanged += (s, e) => { lbl.Visible = string.IsNullOrWhiteSpace(txt.Text); };
            lbl.Click += (s, e) => txt.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.png;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imagePath = ofd.FileName;
                textBox4.Text = "Photo Selected ✔";
                viewImageLink.Visible = true;
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1 ||
                comboBox3.SelectedIndex == -1 || textBox1.Text.Trim() == "" ||
                textBox2.Text.Trim() == "" || textBox3.Text.Trim() == "" || imagePath == "")
            {
                MessageBox.Show("Please fill all fields and select an image.");
                return;
            }

           
            try
            {
                byte[] imgBytes = File.ReadAllBytes(imagePath);

                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    string query = @"
                        INSERT INTO VEHICLE 
                        (reg, type, color, model, fuel, transmission, picture, owner, is_available) 
                        VALUES 
                        (@reg, @type, @color, @model, @fuel, @trans, @pic, @owner, 1)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@reg", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@type", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@color", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@model", textBox3.Text.Trim());

                        
                        cmd.Parameters.AddWithValue("@fuel", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@trans", comboBox3.Text);

                        cmd.Parameters.AddWithValue("@pic", imgBytes);
                        cmd.Parameters.AddWithValue("@owner", loggedInOwner);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Vehicle Added Successfully!");

                // Redirect back to Dashboard
                O_Vehicle_Updates dash = new O_Vehicle_Updates(loggedInOwner);
                dash.Show();
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627) 
                    MessageBox.Show("This Registration Number already exists.");
                else
                    MessageBox.Show("Database Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving vehicle: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            O_Vehicle_Updates dash = new O_Vehicle_Updates(loggedInOwner);
            dash.Show();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void Add_Vehicles_Load(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void button3_Click_1(object sender, EventArgs e) { button3_Click(sender, e); }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}