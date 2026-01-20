using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace Rental_System
{
    public partial class Form2 : Form
    {
        // VARIABLES
        string selectedLicensePath = ""; 
        string selectedNIDPath = "";     

        LinkLabel linkLicenseView = new LinkLabel();
        LinkLabel linkNIDView = new LinkLabel();

        // Validation Colors
        Color colorValid = Color.LightGreen;
        Color colorInvalid = Color.LightPink;
        Color colorDefault = Color.White;

        public Form2()
        {
            InitializeComponent();

            //  VALIDATION EVENT HANDLERS 
            textBox1.TextChanged += (s, e) => { label1.Visible = textBox1.Text.Length == 0; };
            textBox7.TextChanged += (s, e) => { label2.Visible = textBox7.Text.Length == 0; ValidateUsername(textBox7); };
            textBox6.TextChanged += (s, e) => { label3.Visible = textBox6.Text.Length == 0; ValidateEmail(textBox6); };
            textBox5.TextChanged += (s, e) => { label4.Visible = textBox5.Text.Length == 0; ValidatePhone(textBox5); };

            // Password Validation
            textBox3.TextChanged += (s, e) => { label6.Visible = textBox3.Text.Length == 0; ValidatePassword(); };
            textBox2.TextChanged += (s, e) => { label7.Visible = textBox2.Text.Length == 0; ValidatePassword(); };
            textBox3.UseSystemPasswordChar = true;
            textBox2.UseSystemPasswordChar = true;

           
            textBox4.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox4.TextChanged += (s, e) => { label5.Visible = textBox4.Text.Length == 0; };

            
            SetupLinkLabel(linkLicenseView, textBox4, "View License");
            linkLicenseView.LinkClicked += (s, e) => OpenFile(selectedLicensePath);

            SetupLinkLabel(linkNIDView, textBox8, "View NID");
            linkNIDView.LinkClicked += (s, e) => OpenFile(selectedNIDPath);
        }

        private void SetupLinkLabel(LinkLabel link, TextBox targetTb, string text)
        {
            link.Text = text;
            link.Visible = false;
            link.AutoSize = true;
            link.Location = new Point(targetTb.Left, targetTb.Bottom + 3);
            this.Controls.Add(link);
        }

        
        private void ValidateUsername(TextBox tb)
        {
            tb.BackColor = string.IsNullOrWhiteSpace(tb.Text) ? colorDefault : colorValid;
        }

        private void ValidateEmail(TextBox tb)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(tb.Text)) tb.BackColor = colorDefault;
            else tb.BackColor = Regex.IsMatch(tb.Text, pattern) ? colorValid : colorInvalid;
        }

        private void ValidatePhone(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text)) tb.BackColor = colorDefault;
            else tb.BackColor = Regex.IsMatch(tb.Text, @"^[0-9]{11}$") ? colorValid : colorInvalid;
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox3.BackColor = colorDefault;
                textBox2.BackColor = colorDefault;
            }
            else if (textBox3.Text == textBox2.Text)
            {
                textBox3.BackColor = colorValid;
                textBox2.BackColor = colorValid;
            }
            else
            {
                textBox3.BackColor = colorValid;
                textBox2.BackColor = colorInvalid;
            }
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox7.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrEmpty(selectedLicensePath) ||
                string.IsNullOrEmpty(selectedNIDPath)) 
            {
                MessageBox.Show("Please fill all fields and upload BOTH License and NID.");
                return;
            }

            
            if (textBox2.BackColor == colorInvalid || textBox3.Text != textBox2.Text)
            {
                MessageBox.Show("Password mismatch or invalid.");
                return;
            }

            try
            {
                using (SqlConnection conn = DBAccess.GetConnection())
                {
                    if (IsTaken(conn, "username", textBox7.Text)) { ShowError("Username taken!", textBox7); return; }
                    if (IsTaken(conn, "email", textBox6.Text)) { ShowError("Email registered!", textBox6); return; }
                    if (IsTaken(conn, "phone", textBox5.Text)) { ShowError("Phone registered!", textBox5); return; }

                    byte[] licenseBytes = File.ReadAllBytes(selectedLicensePath);
                    byte[] nidBytes = File.ReadAllBytes(selectedNIDPath); // Read NID

                   
                    string insertQuery = @"
                        INSERT INTO users 
                        (username, name, email, phone, password, licence, isAdmin, user_type, nid) 
                        VALUES 
                        (@u, @n, @e, @p, @pass, @lic, 0, 0, @nid)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", textBox7.Text.Trim());
                        cmd.Parameters.AddWithValue("@n", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@e", textBox6.Text.Trim());
                        cmd.Parameters.AddWithValue("@p", textBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", textBox3.Text);
                        cmd.Parameters.AddWithValue("@lic", licenseBytes);
                        cmd.Parameters.AddWithValue("@nid", nidBytes); // Add NID parameter

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Registration successful! You can now login.");
                    Form1 f = new Form1();
                    f.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message);
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            ChooseFile(ref selectedLicensePath, textBox4, linkLicenseView);
        }

       
        private void button4_Click(object sender, EventArgs e)
        {
            ChooseFile(ref selectedNIDPath, textBox8, linkNIDView);
        }

        public void btnUploadNID_Click(object sender, EventArgs e)
        {
            ChooseFile(ref selectedNIDPath, textBox8, linkNIDView);
        }

     
        private void ChooseFile(ref string pathVariable, TextBox displayTb, LinkLabel linkLabel)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image/PDF|*.jpg;*.png;*.jpeg;*.pdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pathVariable = ofd.FileName;
                    displayTb.Text = System.IO.Path.GetFileName(pathVariable);
                    displayTb.BackColor = colorValid;
                    linkLabel.Visible = true;
                }
            }
        }

        private void OpenFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
                catch (Exception ex) { MessageBox.Show("Cannot open file: " + ex.Message); }
            }
        }

  
        private bool IsTaken(SqlConnection conn, string column, string value)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE {column} = @val";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@val", value.Trim());
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void ShowError(string msg, TextBox tb)
        {
            MessageBox.Show(msg);
            tb.BackColor = colorInvalid;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = "Full Name"; label2.Text = "Username"; label3.Text = "Email";
            label4.Text = "Phone"; label5.Text = "Driving License"; label6.Text = "Password";
            label7.Text = "Confirm Password";
        }

    
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox8_TextChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void Form2_Load_1(object sender, EventArgs e) { }
    }
}