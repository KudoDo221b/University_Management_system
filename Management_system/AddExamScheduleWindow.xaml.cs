using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Management_system
{
    /// <summary>
    /// Interaction logic for AddExamScheduleWindow.xaml
    /// </summary>
    public partial class AddExamScheduleWindow : Window
    {
        public AddExamScheduleWindow()
        {
            InitializeComponent();
            LoadCourseClassList();
        }

        private void LoadCourseClassList()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT courseClass_id FROM courseclass";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                    cmbCourseClass.Items.Add(rd.GetString(0));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql =
                    "INSERT INTO examschedule VALUES (@id, @cid, @sch)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.Parameters.AddWithValue("@cid", cmbCourseClass.Text);
                cmd.Parameters.AddWithValue("@sch", txtSchedule.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm lịch thi thành công!");
            this.Close();
        }
    }
}
