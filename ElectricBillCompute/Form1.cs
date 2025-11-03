using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectricBillCompute
{
    public partial class Form1 : Form
    {
        List<HouseReading> houses = new List<HouseReading>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(txtHouseName.Text))
            {
                txtHouseName.BackColor = Color.LightCoral;
                hasError = true;
            }
            else txtHouseName.BackColor = Color.White;

            if (string.IsNullOrWhiteSpace(txtPrev.Text))
            {
                txtPrev.BackColor = Color.LightCoral;
                hasError = true;
            }
            else txtPrev.BackColor = Color.White;

            if (string.IsNullOrWhiteSpace(txtPres.Text))
            {
                txtPres.BackColor = Color.LightCoral;
                hasError = true;
            }
            else txtPres.BackColor = Color.White;

            if (hasError)
            {
                MessageBox.Show("Please fill in all highlighted fields.");
                return;
            }

            if (!double.TryParse(txtPrev.Text, out double prev) ||
                !double.TryParse(txtPres.Text, out double pres))
            {
                MessageBox.Show("Readings must be numbers.");
                return;
            }

            if (pres < prev)
            {
                MessageBox.Show("Present reading cannot be less than previous reading.");
                return;
            }

            houses.Add(new HouseReading
            {
                HouseName = txtHouseName.Text,
                PreviousReading = prev,
                PresentReading = pres
            });

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = houses;

           
            txtHouseName.Clear();
            txtPrev.Clear();
            txtPres.Clear();
        
    }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTotalBill.Text))
            {
                MessageBox.Show("Please enter the total Meralco bill.");
                return;
            }

            if (houses.Count < 2)
            {
                MessageBox.Show("Please add at least 2 houses before computing the total bill.");
                return;
            }

            if (!double.TryParse(txtTotalBill.Text, out double totalBill))
            {
                MessageBox.Show("Please enter a valid amount for total bill.");
                return;
            }

            double totalKwh = houses.Sum(h => h.Consumption);

            if (totalKwh <= 0)
            {
                MessageBox.Show("Total kWh must be greater than zero.");
                return;
            }

            double ratePerKwh = totalBill / totalKwh;

            foreach (var h in houses)
            {
                double unrounded = h.Consumption * ratePerKwh;
                h.Bill = Math.Round(unrounded, 2);
            }

            double roundedSum = houses.Sum(h => h.Bill);
            double diff = Math.Round(totalBill - roundedSum, 2);

            if (Math.Abs(diff) >= 0.01)
            {
                var maxCons = houses.OrderByDescending(h => h.Consumption).FirstOrDefault();
                if (maxCons != null)
                {
                    maxCons.Bill = Math.Round(maxCons.Bill + diff, 2);
                }
            }
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = houses;

            double totalBills = houses.Sum(h => h.Bill);
            lblSummary.Text = $"Total kWh: {totalKwh:F2} | Rate per kWh: ₱{ratePerKwh:F4} | Total Bills: ₱{totalBills:F2}";
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all data?", "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
                return;

            houses.Clear();
            dataGridView1.DataSource = null;

            txtHouseName.Clear();
            txtPrev.Clear();
            txtPres.Clear();
            txtTotalBill.Clear();
            lblSummary.Text = "";

            MessageBox.Show("All data cleared.");
        }

        private void txtPrev_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
            }

        private void txtPres_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selected = dataGridView1.SelectedRows[0].DataBoundItem as HouseReading;
                houses.Remove(selected);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = houses;
            }
            else
            {
                MessageBox.Show("Please select a house to remove.");
            }
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (houses.Count == 0)
            {
                MessageBox.Show("No data to sort.");
                return;
            }

            houses = houses.OrderByDescending(h => h.Consumption).ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = houses;

            MessageBox.Show("Sorted by highest consumption.");
        }
    }
}