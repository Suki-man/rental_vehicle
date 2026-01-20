using System;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Drawing;         // Required for Colors
using System.Text.RegularExpressions; // Required for Validation

namespace Rental_System
{
    public partial class O_Registration : Form
    {
        string selectedNIDPath = "";

        // Colors
        Color colorValid = Color.LightGreen;
        Color colorInvalid = Color.LightPink;
        Color colorDefault = Color.White;

        public O_Registration()
        {
            InitializeComponent();

            textBox7.UseSystemPasswordChar = true;
            textBox8.UseSystemPasswordChar = true;
            textBox6.ReadOnly = true;

            
            textBox1.TextChanged += (s, e) => {
                label1.Visible = textBox1.Text.Length == 0;
                ValidateNotEmpty(textBox1);
            };

            
            textBox2.TextChanged += (s, e) => {
                label2.Visible = textBox2.Text.Length == 0;
                ValidateNotEmpty(textBox2);
            };

            textBox3.TextChanged += (s, e) => {
                label3.Visible = textBox3.Text.Length == 0;
                ValidatePhone(textBox3);
            };

            
            textBox4.TextChanged += (s, e) => {
                label4.Visible = textBox4.Text.Length == 0;
                ValidateEmail(textBox4);
            };

            // 5. Passwords
            textBox7.TextChanged += (s, e) => {
                label7.Visible = textBox7.Text.Length == 0;
                ValidatePasswords();
            };
            textBox8.TextChanged += (s, e) => {
                label8.Visible = textBox8.Text.Length == 0;
                ValidatePasswords();
            };
        }

        private void O_Registration_Load(object sender, EventArgs e)
        {
           
            label1.Visible = true; label2.Visible = true;
            label3.Visible = true; label4.Visible = true;
            label7.Visible = true; label8.Visible = true;
        }

    

      
        private void ValidateNotEmpty(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text)) tb.BackColor = colorDefault;
            else tb.BackColor = colorValid;
        }

       
        private void ValidatePhone(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.BackColor = colorDefault;
                return;
            }

            
            if (Regex.IsMatch(tb.Text, @"^[0-9]{11}$"))
                tb.BackColor = colorValid;
            else
                tb.BackColor = colorInvalid;
        }

        
        private void ValidateEmail(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.BackColor = colorDefault;
                return;
            }

            
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (Regex.IsMatch(tb.Text, pattern))
                tb.BackColor = colorValid;
            else
                tb.BackColor = colorInvalid;
        }

        
        private void ValidatePasswords()
        {
          
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                textBox7.BackColor = colorDefault;
                textBox8.BackColor = colorDefault;
                return;
            }

            
            textBox7.BackColor = colorValid;

            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                textBox8.BackColor = colorDefault;
            }
            else if (textBox7.Text == textBox8.Text)
            {
                textBox8.BackColor = colorValid; // Match
            }
            else
            {
                textBox8.BackColor = colorInvalid; // Mismatch
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image/PDF|*.jpg;*.png;*.jpeg;*.pdf";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedNIDPath = ofd.FileName;
                textBox6.Text = System.IO.Path.GetFileName(selectedNIDPath);
                textBox6.BackColor = colorValid;
            }
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            
            if (textBox3.BackColor == colorInvalid || textBox4.BackColor == colorInvalid || textBox8.BackColor == colorInvalid)
            {
                MessageBox.Show("Please fix the invalid fields (marked in red).");
                return;
            }

            
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox7.Text) || string.IsNullOrEmpty(selectedNIDPath))
            {
                MessageBox.Show("Please fill all fields and upload your NID.");
                return;
            }

            
            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    if (IsTaken(conn, "username", textBox2.Text.Trim())) { MessageBox.Show("Username already exists."); return; }
                    if (IsTaken(conn, "email", textBox4.Text.Trim())) { MessageBox.Show("Email already exists."); return; }
                    if (IsTaken(conn, "phone", textBox3.Text.Trim())) { MessageBox.Show("Phone number already exists."); return; }

                    byte[] nidBytes = File.ReadAllBytes(selectedNIDPath);

                    string query = @"
                        INSERT INTO users 
                        (username, name, email, phone, password, nid, isAdmin, user_type, licence) 
                        VALUES 
                        (@u, @n, @e, @p, @pass, @nid, 0, 1, NULL)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@n", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@e", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@p", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", textBox7.Text);
                        cmd.Parameters.AddWithValue("@nid", nidBytes);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Owner Registration Successful!");
                    Owner_Login ownerLogin = new Owner_Login();
                    ownerLogin.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        
        private void button4_Click(object sender, EventArgs e)
        {
            Owner_Login ownerLogin = new Owner_Login();
            ownerLogin.Show();
            this.Close();
        }

        private bool IsTaken(SqlConnection conn, string column, string value)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE {column} = @val";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@val", value);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

  
        private void O_Registration_Load_1(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void textBox8_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}