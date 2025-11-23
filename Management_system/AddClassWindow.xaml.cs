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
    /// Interaction logic for AddClassWindow.xaml
    /// </summary>
    public partial class AddClassWindow : Window
    {
        public AddClassWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string id = txtClassId.Text.Trim();
            string course = txtCourseId.Text.Trim();
            string room = txtRoom.Text.Trim();
            string learn = txtSchedule.Text.Trim();
            string duration = txtDuration.Text.Trim();
            string semester = txtSemester.Text.Trim();
            string year = txtYear.Text.Trim();
            string status = txtStatus.Text.Trim();
            string capacity = txtCapacity.Text.Trim();
            string exam = txtExamSchedule.Text.Trim();

            if (id == "" || course == "")
            {
                MessageBox.Show("Mã lớp học phần và Mã môn bắt buộc nhập!");
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO courseclass
                    (courseClass_id, course_id, room, learnSchedule, duration,
                     semester, year, status, capacity, examSchedule)
                    VALUES
                    (@id, @course, @room, @learn, @duration,
                     @semester, @year, @status, @capacity, @exam)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@room", room);
                cmd.Parameters.AddWithValue("@learn", learn);
                cmd.Parameters.AddWithValue("@duration", duration);
                cmd.Parameters.AddWithValue("@semester", semester);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@capacity", capacity);
                cmd.Parameters.AddWithValue("@exam", exam);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm lớp học phần thành công!");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Lỗi: Mã lớp bị trùng hoặc dữ liệu không hợp lệ!");
                }
            }
        }
    }
}
