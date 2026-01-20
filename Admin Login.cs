using System;
using System.Windows.Forms;
using System.Data.SqlClient; // Required for Database

namespace Rental_System
{
    public partial class Admin_Login : Form
    {
        public Admin_Login()
        {
            InitializeComponent();

      
            textBox1.TextChanged += (s, e) => { label1.Visible = textBox1.Text.Length == 0; };
            textBox2.TextChanged += (s, e) => { label2.Visible = textBox2.Text.Length == 0; };
            textBox2.UseSystemPasswordChar = true;
        }

        private void Admin_Login_Load(object sender, EventArgs e)
        {
            label1.Text = "Admin Username";
            label2.Text = "Admin Password";

            
            label1.Visible = string.IsNullOrEmpty(textBox1.Text);
            label2.Visible = string.IsNullOrEmpty(textBox2.Text);
        }

    
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                   
                    string query = "SELECT password, isAdmin FROM USERS WHERE username = @u";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                               
                                string dbPass = reader["password"].ToString();

                                
                                bool isAdmin = false;
                                var adminVal = reader["isAdmin"];
                                if (adminVal != DBNull.Value)
                                {
                                   
                                    isAdmin = Convert.ToBoolean(adminVal);
                                }

                                
                                if (dbPass == password)
                                {
                                   
                                    if (isAdmin)
                                    {
                                        MessageBox.Show("Admin Login Successful");

                                  
                                        Admin_Dashboard ad = new Admin_Dashboard(username);
                                        ad.Show();
                                        this.Hide();
                                    }
                                    else
                                    {
                                        MessageBox.Show("You are not an Admin");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid Password");
                                }
                            }
                            else
                            {
                       
                                MessageBox.Show("User not registered");
                            }
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
            Home h = new Home();
            h.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e) { textBox1.Focus(); }
        private void label2_Click(object sender, EventArgs e) { textBox2.Focus(); }

        private void label4_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void Admin_Login_Load_1(object sender, EventArgs e) { }

        
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
    }
}