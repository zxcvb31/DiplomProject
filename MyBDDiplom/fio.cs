using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MyBDKursach
{
    public partial class fio : Form
    {
        DataSet ds;
        MySqlDataAdapter adapter;
        MySqlCommandBuilder commandBuilder;
        string connectionString = Program.GetStringConnection();
        string sql = "SELECT * FROM fio";
        public fio()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                adapter = new MySqlDataAdapter(sql, connection);
                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                // делаем недоступным столбец id для изменения
                dataGridView1.Columns["id_fio"].ReadOnly = true;
            }

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                adapter = new MySqlDataAdapter(sql, connection);
                commandBuilder = new MySqlCommandBuilder(adapter);
                adapter.InsertCommand = new MySqlCommand("sp_CreateFIO", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@FirstName", MySqlDbType.VarChar, 50, "First_name"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@LastName", MySqlDbType.VarChar, 50, "Last_name"));

                MySqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_fio", MySqlDbType.Int64, 0, "id_fio");
                parameter.Direction = ParameterDirection.Output;

                try
                {
                    adapter.Update(ds);
                    MessageBox.Show("Готово!");
                }
                catch (Exception l)
                {
                    Console.WriteLine("Error: " + l.Message);
                }
            }
        }
        private void поставщикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            postavshik newForm = new postavshik();
            newForm.Show();
        }

        private void товарToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tovar newForm = new tovar();
            newForm.Show();
        }

        private void клиентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fio newForm = new fio();
            newForm.Show();
        }

        private void корзинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            korzinaFinal newForm = new korzinaFinal();
            newForm.Show();
        }

        private void складыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sklads newForm = new Sklads();
            newForm.Show();
        }

        private void адресаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewAddresFinal newForm = new NewAddresFinal();
            newForm.Show();
        }

        private void полкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Polki newForm = new Polki();
            newForm.Show();
        }
    }
}
