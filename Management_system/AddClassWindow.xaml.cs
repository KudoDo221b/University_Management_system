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

            if (id == "" || course == "")
            {
                MessageBox.Show("Mã lớp học phần và Mã môn bắt buộc nhập!");
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    // 1️⃣ Kiểm tra course_id tồn tại
                    using (MySqlCommand checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM course WHERE course_id=@course", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@course", course);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            MessageBox.Show("Mã môn học không tồn tại trong hệ thống!");
                            return;
                        }
                    }
                    using (MySqlCommand checkClassCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM courseclass WHERE courseClass_id=@id", conn))
                    {
                        checkClassCmd.Parameters.AddWithValue("@id", id);
                        int classCount = Convert.ToInt32(checkClassCmd.ExecuteScalar());

                        if (classCount > 0)
                        {
                            MessageBox.Show("Mã lớp học phần đã tồn tại, vui lòng nhập mã khác!");
                            return;
                        }
                    }
                    // 2️⃣ Thêm lớp học phần vào courseclass
                    string sqlClass = @"
                        INSERT INTO courseclass
                        (courseClass_id, course_id, room, learnSchedule, duration,
                         semester, year, status, capacity)
                        VALUES
                        (@id, @course, @room, @learn, @duration,
                         @semester, @year, @status, @capacity)";

                    using (MySqlCommand cmdClass = new MySqlCommand(sqlClass, conn))
                    {
                        cmdClass.Parameters.AddWithValue("@id", id);
                        cmdClass.Parameters.AddWithValue("@course", course);
                        cmdClass.Parameters.AddWithValue("@room", room);
                        cmdClass.Parameters.AddWithValue("@learn", learn);
                        cmdClass.Parameters.AddWithValue("@duration", duration);
                        cmdClass.Parameters.AddWithValue("@semester", semester);
                        cmdClass.Parameters.AddWithValue("@year", year);
                        cmdClass.Parameters.AddWithValue("@status", status);
                        cmdClass.Parameters.AddWithValue("@capacity", capacity);

                        cmdClass.ExecuteNonQuery();
                    }
                    MessageBox.Show("Thêm lớp học phần thành công!");
                    this.Close();

                }
                
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
