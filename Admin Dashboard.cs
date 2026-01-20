using System;
using System.Windows.Forms;

namespace Rental_System
{
    public partial class Admin_Dashboard : Form
    {
        private string adminUser;

        public Admin_Dashboard(string username)
        {
            InitializeComponent();
            this.adminUser = username;
        }

        private void Admin_Dashboard_Load(object sender, EventArgs e)
        {
            this.Text = "Admin Dashboard - Logged in as: " + adminUser;
        }

      
        private void button1_Click(object sender, EventArgs e)
        {
            TOTAL_RENTER tr = new TOTAL_RENTER(adminUser);
            tr.Show();
            this.Hide();
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            TOTAL_OWNER to = new TOTAL_OWNER(adminUser);
            to.Show();
            this.Hide();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            
            TOTAL_VEHICLE tv = new TOTAL_VEHICLE(adminUser);
            tv.Show();
            this.Hide(); 
        }

        // TOTAL BOOKINGS
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Total Bookings: " + DataStore.TotalBookings);
        }

        //  LOGOUT
        private void button5_Click(object sender, EventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

      
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button4_Click_1(object sender, EventArgs e)
        {
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            Admin_Login adminLogin = new Admin_Login();
            adminLogin.Show();
            this.Hide();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}