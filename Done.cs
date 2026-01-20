using System;
using System.Windows.Forms;

namespace Rental_System
{
    public partial class Done : Form
    {
        public Done()
        {
            InitializeComponent();
        }

        private void Done_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

            
            timer1.Interval = 40;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                
                progressBar1.Value += 1;

               
                if (progressBar1.Value < 100)
                {
                    progressBar1.Value += 1;
                    progressBar1.Value -= 1;
                }
            }
            else
            {
                timer1.Stop();
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }
    }
}