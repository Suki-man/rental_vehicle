using System;
using System.Windows.Forms;

namespace Rental_System
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
        }

        //  USER BUTTON 
        private void button1_Click(object sender, EventArgs e)
        {
            
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();   
        }

    
        private void button2_Click(object sender, EventArgs e)
        {
            Owner_Login ownerLogin = new Owner_Login();
            ownerLogin.Show();
            this.Hide();   
        }

        
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {
            Admin_Login adminLogin = new Admin_Login();
            adminLogin.Show();
            this.Hide();
        }

        private void Home_Load_1(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
