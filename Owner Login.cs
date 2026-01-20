using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Rental_System
{
    public partial class Owner_Login : Form
    {
        public Owner_Login()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;

           
            textBox1.TextChanged += (s, e) => label1.Visible = textBox1.Text.Length == 0;
            textBox2.TextChanged += (s, e) => label2.Visible = textBox2.Text.Length == 0;
        }

        private void Owner_Login_Load(object sender, EventArgs e)
        {
            label1.Visible = true;
            label2.Visible = true;
            label1.Text = "Username";
            label2.Text = "Password";
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
               
                    string query = @"SELECT COUNT(*) FROM users 
                                     WHERE username = @u AND password = @p AND user_type = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Owner Login Successful!");

                            
                            O_Vehicle_Updates ov = new O_Vehicle_Updates(username);
                            ov.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid login or not an Owner.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Login Error: " + ex.Message);
            }
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            O_Registration reg = new O_Registration();
            reg.Show();
            this.Hide();
        }

   
        private void button3_Click(object sender, EventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}