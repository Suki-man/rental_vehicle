using System;
using System.Windows.Forms;

namespace Rental_System
{
    public partial class O_Vehicle_Updates : Form
    {
        
        private string loggedInOwner;

        
        public O_Vehicle_Updates(string username)
        {
            InitializeComponent();

           
            this.loggedInOwner = username;

            textBox3.Text = username;
            textBox3.ReadOnly = true;
        }

        private void O_Vehicle_Updates_Load(object sender, EventArgs e)
        {

        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            Add_Vehicles av = new Add_Vehicles(loggedInOwner);
            av.Show();
            this.Hide();
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            Book_Request br = new Book_Request(loggedInOwner);
            br.Show();
            this.Hide();
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close(); 
        }

        
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
    }
}