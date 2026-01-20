using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Rental_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

           
            textBox4.TextChanged += textBox4_TextChanged;
            textBox3.TextChanged += textBox3_TextChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox3.Text = "";
            textBox3.UseSystemPasswordChar = true;

            label1.Text = "Username";
            label2.Text = "Password";
            label1.Visible = true;
            label2.Visible = true;
        }

   
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = textBox4.Text.Length == 0;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label2.Visible = textBox3.Text.Length == 0;
        }

        private void label1_Click(object sender, EventArgs e) => textBox4.Focus();
        private void label2_Click(object sender, EventArgs e) => textBox3.Focus();

        
        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox4.Text.Trim();
            string password = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    
                    string query = "SELECT user_type FROM users WHERE username = @u AND password = @p";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null) 
                        {
                            bool userType = Convert.ToBoolean(result);

                            
                            if (userType == true)
                            {
                                MessageBox.Show("You are not a renter.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return; 
                            }

                            
                            MessageBox.Show("Login Successful!");

                            Form3 f3 = new Form3(username); 
                            f3.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Wrong username or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Login Error: " + ex.Message);
            }
        }


        private void button4_Click_1(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            this.Hide();
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            Home h = new Home();
            h.Show();
            this.Hide();
        }

        
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void textBox3_TextChanged_1(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click_1(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label1_Click_1(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void button4_Click(object sender, EventArgs e) { }
    }
}