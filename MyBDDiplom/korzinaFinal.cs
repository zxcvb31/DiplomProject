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
    public partial class korzinaFinal : Form
    {
        DataSet ds;
        MySqlDataAdapter adapter;
        MySqlCommandBuilder commandBuilder;
        string connectionString = Program.GetStringConnection();
        string sql = "SELECT * FROM korzina ORDER BY id_korzina";
        string sql2 ="select id_korzina,  Concat(First_name, ' ',Last_name) as fio,Nazvanie,kol_vo, DataPokupki  from (select id_korzina, korzina.id_fio, id_tovar,kol_vo, DataPokupki,First_name, Last_name  from korzina inner join fio on korzina.id_fio=fio.id_fio) as obsh1 inner join tovar on obsh1.id_tovar = tovar.id_Tovar ORDER BY id_korzina";
        DataSet ds2;
        public korzinaFinal()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                adapter = new MySqlDataAdapter(sql, connection);
                ds = new DataSet();//Создание основной таблицы значений
                adapter.Fill(ds);//Первичное заполнение основной таблицы значений
                DataTable fullName = ds.Tables.Add("Name");//Создание вспомогательной таблицы значений
                adapter = new MySqlDataAdapter(sql2, connection);
                dataGridView1.DataSource = ds.Tables[0];
                adapter.Fill(fullName);//Первичное заполнение вспомогательной таблицы значений
                dataGridView1.ReadOnly = true;
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            dataGridView2.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn();
            col0.HeaderText = "id_korzina";
            col0.Name = "id_korzina";
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "ФИО";
            col1.Name = "fio";
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "Название";
            col2.Name = "Nazvanie";
            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "Количество";
            col3.Name = "kol_vo";
            CalendarColumn col4 = new CalendarColumn();
            col4.HeaderText = "Дата Покупки";
            col4.Name = "DataPokupki";
            dataGridView2.Columns.Add(col0);
            dataGridView2.Columns.Add(col1);
            dataGridView2.Columns.Add(col2);
            dataGridView2.Columns.Add(col3);
            dataGridView2.Columns.Add(col4);
            dataGridView2.Columns[0].ReadOnly = true;
            DataTable TableName = ds.Tables[1];
            foreach (DataRow roww in TableName.Rows)
            {
                dataGridView2.Rows.Add(roww[0], roww[1], roww[2], roww[3], (DateTime)roww[4]);

            }
            DataGridViewColumn stolb = dataGridView2.Columns[2];
            stolb.Width = 180;

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql3 = "select id_fio, concat(First_name,' ',Last_name) as fullName from fio order by id_fio";
                string sql4 = "select id_Tovar, Nazvanie from tovar order by id_Tovar";
                connection.Open();
                adapter = new MySqlDataAdapter(sql3, connection);
                ds2 = new DataSet();
                DataTable addresName = ds2.Tables.Add("fio");
                adapter.Fill(addresName);
                adapter = new MySqlDataAdapter(sql4, connection);
                DataTable postavshik = ds2.Tables.Add("tovar");
                adapter.Fill(postavshik);
            }
        }
        //Функция, добавляющая новую строку в основную таблицу значений
        private void AddButton_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[0].Rows.Add(row);
            dataGridView2.Rows.Add();
            DataRow row2 = ds.Tables[1].NewRow(); // добавляем новую строку в DataTable
            ds.Tables[1].Rows.Add(row2);
        }
        // Функция, удаляющая значения из основной таблицы
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
            DataGridViewRow row2 = dataGridView2.CurrentRow;
            dataGridView2.Rows.Remove(row2);
        }
        //Функция, сохраняющая данные из основной таблицы значений в базу данных
        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                adapter = new MySqlDataAdapter(sql, connection);
                commandBuilder = new MySqlCommandBuilder(adapter);
                adapter.InsertCommand = new MySqlCommand("sp_createKorzina", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@fioId", MySqlDbType.Int32, 0, "id_fio"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@TovarId", MySqlDbType.Int64, 0, "id_tovar"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@kolvo", MySqlDbType.Int64, 0, "kol_vo"));
                adapter.InsertCommand.Parameters.Add(new MySqlParameter("@DataP", MySqlDbType.DateTime, 0, "DataPokupki"));

                MySqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_korzina", MySqlDbType.Int64, 0, "id_korzina");
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
        //Функция, предоставляющая возможность выбирать текущие значения из базы данных
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow FullRow = dataGridView2.CurrentRow;
            dataGridView1.ClearSelection();
            dataGridView1.Rows[FullRow.Index].Selected = true;

            int rowIndex = dataGridView2.CurrentRow.Index;
            int columIndex = dataGridView2.CurrentCell.ColumnIndex;

            DataTable fios = ds2.Tables[0];
            DataTable tovars = ds2.Tables[1];
            var fiosRow = fios.Select();
            var tovarsRow = tovars.Select();
            if (columIndex == 1 || columIndex == 2)
            {
                DataGridViewComboBoxCell ComboBoxCell1 = new DataGridViewComboBoxCell();
                //ФИО
                if (columIndex == 1)
                {
                    int i = 0;
                    string[] znach = new string[fiosRow.Length];
                    foreach (DataRow fio in fiosRow)
                    {
                        // получаем все ячейки строки
                        var cells = fio.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    ComboBoxCell1.Items.AddRange(znach);
                }
                //товар
                if (columIndex == 2)
                {
                    int i = 0;
                    string[] znach = new string[tovarsRow.Length];
                    foreach (DataRow tovar in tovarsRow)
                    {
                        // получаем все ячейки строки
                        var cells = tovar.ItemArray;
                        znach[i] = cells[1].ToString();
                        i++;
                    }
                    ComboBoxCell1.Items.AddRange(znach);
                }

                if (dataGridView2[columIndex, rowIndex].Value == null)
                {
                    dataGridView2[columIndex, rowIndex].Value = ComboBoxCell1.Items[0];
                }
                var vall = dataGridView2[columIndex, rowIndex].Value.ToString();
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
        //Функция, которая записывает данные в основную таблицу после редактирования
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataTable TableId = ds.Tables[0];
            DataTable fios = ds2.Tables[0];
            DataTable tovars = ds2.Tables[1];
            int Row = dataGridView2.CurrentRow.Index;

            if (e.ColumnIndex != 0)
            {
                if (e.ColumnIndex == 1)
                {
                    var fio = fios.Select("fullName='" + dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() + "'");
                    var row = fio[0].ItemArray;
                    dataGridView1.Rows[Row].Cells[e.ColumnIndex].Value = row[0];
                }
                if (e.ColumnIndex == 2)
                {
                    var tovar = tovars.Select("Nazvanie ='" + dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString() + "'");
                    var row = tovar[0].ItemArray;
                    dataGridView1.Rows[Row].Cells[e.ColumnIndex].Value = row[0];
                }
                if (e.ColumnIndex == 3)
                {
                    
                        TableId.Rows[Row][e.ColumnIndex] = dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString();
                }
                if (e.ColumnIndex == 4)
                {
                    if (dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value != null)
                    TableId.Rows[Row][e.ColumnIndex] = dataGridView2.CurrentRow.Cells[e.ColumnIndex].Value.ToString();
                }
            }

        }
        //Функции для возможности перехода по другим частям приложения 
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
    //Класс для возможности указать нужную дату при покупке
    public class CalendarColumn : DataGridViewColumn
    {
        public CalendarColumn()
            : base(new CalendarCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
                {
                    throw new InvalidCastException("Must be a CalendarCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class CalendarCell : DataGridViewTextBoxCell
    {

        public CalendarCell()
            : base()
        {
            // Использование короткого формата записи
            this.Style.Format = "d";
        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Устанавливает значение элемента управления редактированием на текущее значение ячейки
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            CalendarEditingControl ctl =
                DataGridView.EditingControl as CalendarEditingControl;
            // Использование значения строки по умолчанию, когда свойство Value имеет значение null.
            if (this.Value == null)
            {
                ctl.Value = (DateTime)this.DefaultNewRowValue;
            }
            else
            {
                ctl.Value = (DateTime)this.Value;
            }
        }

        public override Type EditType
        {
            get
            {
                // Возвращает тип элемента управления редактированием, который использует CalendarCell.
                return typeof(CalendarEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Возвращает тип значения, которое содержит CalendarCell.

                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Использует текущую дату и время в качестве значения по умолчанию.
                return DateTime.Now;
            }
        }
    }

    class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public CalendarEditingControl()
        {
            this.Format = DateTimePickerFormat.Short;
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToShortDateString();
            }
            set
            {
                if (value is String)
                {
                    try
                    {
                        // This will throw an exception of the string is 
                        // null, empty, or not in the format of a date.
                        this.Value = DateTime.Parse((String)value);
                    }
                    catch
                    {
                        // In the case of an exception, just use the 
                        // default value so we're not left with a null
                        // value.
                        this.Value = DateTime.Now;
                    }
                }
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Позволяет DateTimePicker обрабатывать перечисленные ключи.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            // Сообщаем DataGridView, что содержимое ячейки
            // изменились.
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }

}
