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
    /// Interaction logic for EditExamScheduleWindow.xaml
    /// </summary>
    public partial class EditExamScheduleWindow : Window
    {
        private string examID;

        public EditExamScheduleWindow(string id)
        {
            InitializeComponent();
            examID = id;

            LoadCourseClassList();
            LoadOldData();
        }

        // -----------------------------
        // LOAD LIST COURSECLASS
        // -----------------------------
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

        // -----------------------------
        // LOAD OLD EXAM DATA
        // -----------------------------
        private void LoadOldData()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT exam_id, courseClass_id, schedule FROM examschedule WHERE exam_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", examID);

                MySqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    txtID.Text = rd.GetString("exam_id");
                    cmbCourseClass.Text = rd.GetString("courseClass_id");
                    txtSchedule.Text = rd.GetString("schedule");
                }
            }
        }

        // -----------------------------
        // SAVE CHANGES
        // -----------------------------
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE examschedule 
                               SET courseClass_id=@cid, schedule=@sch 
                               WHERE exam_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.Parameters.AddWithValue("@cid", cmbCourseClass.Text);
                cmd.Parameters.AddWithValue("@sch", txtSchedule.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Cập nhật lịch thi thành công!", "Thông báo");
            this.Close();
        }
    }
}
