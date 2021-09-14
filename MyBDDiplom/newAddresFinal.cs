using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace MyBDKursach
{
    public partial class NewAddresFinal : Form
    {
        DataSet ds;
        MySqlDataAdapter adapter;
        MySqlCommandBuilder commandBuilder;
        string connectionString = Program.GetStringConnection();
        string sql = "SELECT * FROM address ORDER BY id_Address";
        string sql2 = "select id_Address,nameCountry as Страна,nameCity as Город,nameStreet as Улица,House as Дом,Name as Поставщик from (select * from (select * from (select * from address left join postavshik on idPostavshik=id_Postavshik) as obshaya1 left join (select id_tp as id0,name as nameCountry from tp_adress where namevalue=1 ) as contries on idCountry=contries.id0 ) as obshaya2 left join (select id_tp as id1,name as nameCity from tp_adress where namevalue=2) as cities on idCity=cities.id1) as obshaya3 left join (select id_tp as id2,name as nameStreet from tp_adress where namevalue=3) as streets on idStreet=streets.id2 ORDER BY id_Address";
        DataSet ds2;


        public NewAddresFinal()
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
                DataTable fullName = ds.Tables.Add("Name");
                DataTable TableId = ds.Tables[0];
                adapter = new MySqlDataAdapter(sql2, connection);
                adapter.Fill(fullName);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[1].ReadOnly = true;
                dataGridView1.Columns[2].ReadOnly = true;
                dataGridView1.Columns[3].ReadOnly = true;
                dataGridView1.Columns[5].ReadOnly = true;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            dataGridView2.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn();
            col0.HeaderText = "id_Address";
            col0.Name = "id_Address";
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "Страна";
            col1.Name = "idCountry";
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "Город";
            col2.Name = "idCity";
            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "Улица";
            col3.Name = "idStreet";
            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "Дом";
            col4.Name = "House";
            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
            col5.HeaderText = "Поставщик";
            col5.Name = "idPostavshik";
            dataGridView2.Columns.Add(col0);
            dataGridView2.Columns.Add(col1);
            dataGridView2.Columns.Add(col2);
            dataGridView2.Columns.Add(col3);
            dataGridView2.Columns.Add(col4);
            dataGridView2.Columns.Add(col5);
            dataGridView2.Columns[0].ReadOnly = true;
            DataTable TableName = ds.Tables[1];
            foreach (DataRow roww in TableName.Rows)
            {
                dataGridView2.Rows.Add(roww[0], roww[1], roww[2], roww[3], roww[4], roww[5]);

            }

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql3 = "select id_TP, Name, NameValue from tp_adress order by id_TP";
                string sql4 = "select id_Postavshik, Name from postavshik order by id_Postavshik";
                connection.Open();
                adapter = new MySqlDataAdapter(sql3, connection);
                ds2 = new DataSet();
                DataTable addresName = ds2.Tables.Add("addres");
                adapter.Fill(addresName);
                adapter = new MySqlDataAdapter(sql4, connection);
                DataTable postavshik = ds2.Tables.Add("postavshik");
                adapter.Fill(postavshik);
            }

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
            dataGridView2.Rows.Add();
            DataRow row2 = ds.Tables[1].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[1].Rows.Add(row2);

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }

            DataGridViewRow row2 = dataGridView2.CurrentRow;
            dataGridView2.Rows.Remove(row2);

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                adapter = new MySqlDataAdapter(sql, connection);
                commandBuilder = new MySqlCommandBuilder(adapter);
                adapter.InsertCommand = new MySqlCommand("sp_creatAddress", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@CountryId", MySqlDbType.Int32, 0, "idCountry"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@CityId", MySqlDbType.Int64, 0, "idCity"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@StreetId", MySqlDbType.Int64, 0, "idStreet"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@HouseInt", MySqlDbType.Int64, 0, "House"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@PostavshikId", MySqlDbType.Int64, 0, "idPostavshik"));
                MySqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_Address", MySqlDbType.Int64, 0, "id_Address");
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



        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow FullRow = dataGridView2.CurrentRow;
            dataGridView1.ClearSelection();
            dataGridView1.Rows[FullRow.Index].Selected = true;


            int rowIndex = dataGridView2.CurrentRow.Index;
            int columIndex = dataGridView2.CurrentCell.ColumnIndex;
            DataTable NameTable = ds.Tables[1];
            DataTable addres = ds2.Tables[0];
            DataTable postavshik = ds2.Tables[1];
            var postavshiks = postavshik.Select();
            if (columIndex != 0 & columIndex != 4)
            {
                DataGridViewComboBoxCell ComboBoxCell1 = new DataGridViewComboBoxCell();
                //Страны
                if (columIndex == 1)
                {
                    var countries = addres.Select("NameValue='1'");
                    int i = 0;
                    string[] znach = new string[countries.Length];
                    foreach (DataRow country in countries)
                    {
                        // получаем все ячейки строки
                        var cells = country.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    ComboBoxCell1.Items.AddRange(znach);
                }
                //Города
                else if (columIndex == 2)
                {
                    var cities = addres.Select("NameValue='2'");
                    int i = 0;
                    string[] znach = new string[cities.Length];
                    foreach (DataRow city in cities)
                    {
                        // получаем все ячейки строки
                        var cells = city.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    ComboBoxCell1.Items.AddRange(znach);
                }
                //Улицы
                else if (columIndex == 3)
                {

                    var streets = addres.Select("NameValue='3'");
                    int i = 0;
                    string[] znach = new string[streets.Length];
                    foreach (DataRow street in streets)
                    {
                        // получаем все ячейки строки
                        var cells = street.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    ComboBoxCell1.Items.AddRange(znach);
                }
                //Поставщики
                else if (columIndex == 5)
                {

                    int i = 0;
                    string[] znach = new string[postavshiks.Length + 1];
                    foreach (DataRow postavsh in postavshiks)
                    {
                        // получаем все ячейки строки
                        var cells = postavsh.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    znach[i] = "Нет поставщика";
                    ComboBoxCell1.Items.AddRange(znach);
                }



                if (dataGridView2[columIndex, rowIndex].Value == null)
                {
                    dataGridView2[columIndex, rowIndex].Value = ComboBoxCell1.Items[0];
                }
                var vall = dataGridView2[columIndex, rowIndex].Value.ToString();
                if (vall == "") vall = "Нет поставщика";
                dataGridView2[columIndex, rowIndex] = ComboBoxCell1;
                ComboBoxCell1.Value = vall;
                ComboBoxCell1.DisplayStyleForCurrentCellOnly = true;
                dataGridView2.Rows[rowIndex].Cells[columIndex].Value = (dataGridView2.Rows[rowIndex].Cells[columIndex] as DataGridViewComboBoxCell).Items[0];
                if (ComboBoxCell1.Value != null)
                {
                    int indx = ComboBoxCell1.Items.IndexOf(vall);
                    dataGridView2.Rows[rowIndex].Cells[columIndex].Value = (dataGridView2.Rows[rowIndex].Cells[columIndex] as DataGridViewComboBoxCell).Items[indx];
                }
            }

        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataTable addres = ds2.Tables[0];
            DataTable TableId = ds.Tables[0];
            DataTable postavshik = ds2.Tables[1];
            int Row = dataGridView2.CurrentRow.Index;
            if (e.ColumnIndex != 0)
            {
                if (e.ColumnIndex == 4)
                {
                    TableId.Rows[Row][e.ColumnIndex] = dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString();
                }
                else if (e.ColumnIndex == 5)
                {
                    if (dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() == "Нет поставщика")
                    {
                        dataGridView1.Rows[Row].Cells[e.ColumnIndex].Value = DBNull.Value;
                    }
                    else
                    {
                        var postavsh = postavshik.Select("Name='" + dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() + "'");
                        var row = postavsh[0].ItemArray;
                        dataGridView1.Rows[Row].Cells[e.ColumnIndex].Value = row[0];
                    }
                }
                else
                {
                    var country = addres.Select("Name='" + dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() + "'");
                    var row = country[0].ItemArray;

                    dataGridView1.Rows[Row].Cells[e.ColumnIndex].Value = row[0];
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
