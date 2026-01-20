using System;
using System.Windows.Forms;

namespace Rental_System
{
    public partial class Form3 : Form
    {
        private string loggedInUser;

        public Form3(string username)
        {
            InitializeComponent();
            this.loggedInUser = username;

            // Setup User Display
            textBox3.Text = username;
            textBox3.ReadOnly = true;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            label1.Text = "Choose vehicle name";
        }

       
        private void button1_Click(object sender, EventArgs e) // Car
        {
            Car_List car = new Car_List(loggedInUser);
            car.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e) // Bike
        {
            Bike_List bike = new Bike_List(loggedInUser);
            bike.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e) // Pickup
        {
            Pickup_List pickup = new Pickup_List(loggedInUser);
            pickup.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e) // Cycle
        {
            Cycle_List cycle = new Cycle_List(loggedInUser);
            cycle.Show();
            this.Hide();
        }

        
        private void button6_Click(object sender, EventArgs e)
        {
            Return_Vehicle returnPage = new Return_Vehicle(loggedInUser);
            returnPage.Show();
            this.Hide();
        }

        
        private void button5_Click(object sender, EventArgs e)
        {
            
            Home home = new Home();

            
            home.Show();

           
            this.Close();
        }

       
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}