using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for EditClassWindow.xaml
    /// </summary>
    public partial class EditClassWindow : Window
    {
        private string classId;

        public EditClassWindow(string id)
        {
            InitializeComponent();
            classId = id;
            LoadClassInfo();
        }

        private void LoadClassInfo()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT cc.courseClass_id, cc.course_id, cc.room, cc.learnSchedule,
                           cc.duration, cc.semester, cc.year, cc.status, cc.capacity,
                           es.schedule
                    FROM courseclass cc
                    LEFT JOIN examschedule es ON cc.courseClass_id = es.courseClass_id
                    WHERE cc.courseClass_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", classId);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    var row = dt.Rows[0];

                    txtClassId.Text = row["courseClass_id"].ToString();
                    txtCourseId.Text = row["course_id"].ToString();
                    txtRoom.Text = row["room"].ToString();
                    txtSchedule.Text = row["learnSchedule"].ToString();
                    txtDuration.Text = row["duration"].ToString();
                    txtSemester.Text = row["semester"].ToString();
                    txtYear.Text = row["year"].ToString();
                    txtStatus.Text = row["status"].ToString();
                    txtCapacity.Text = row["capacity"].ToString();
                    txtExamSchedule.Text = row["schedule"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                // 1️⃣ Kiểm tra course_id có tồn tại trong bảng course không
                string courseId = txtCourseId.Text.Trim();
                using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM course WHERE course_id=@course", conn))
                {
                    checkCmd.Parameters.AddWithValue("@course", courseId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count == 0)
                    {
                        MessageBox.Show("Mã môn học không tồn tại trong hệ thống!");
                        return;
                    }
                }
                    string sql = @"
                    UPDATE courseclass SET
                        course_id=@course,
                        room=@room,
                        learnSchedule=@learn,
                        duration=@duration,
                        semester=@semester,
                        year=@year,
                        status=@status,
                        capacity=@capacity
                    WHERE courseClass_id=@id";

                using (MySqlCommand cmdClass = new MySqlCommand(sql, conn))
                {
                    cmdClass.Parameters.AddWithValue("@id", classId);
                    cmdClass.Parameters.AddWithValue("@course", courseId);
                    cmdClass.Parameters.AddWithValue("@room", txtRoom.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@learn", txtSchedule.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@duration", txtDuration.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@year", txtYear.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@status", txtStatus.Text.Trim());
                    cmdClass.Parameters.AddWithValue("@capacity", txtCapacity.Text.Trim());

                    cmdClass.ExecuteNonQuery();
                }

                string examSchedule = txtExamSchedule.Text.Trim();
                if (!string.IsNullOrEmpty(examSchedule))
                {
                    string sqlExam = @"UPDATE examSchedule SET schedule=@exam
                                        WHERE courseClass_id=@id";

                    using (MySqlCommand cmdExam = new MySqlCommand(sqlExam, conn))
                    {
                        cmdExam.Parameters.AddWithValue("@id", classId);
                        cmdExam.Parameters.AddWithValue("@exam", examSchedule);
                        cmdExam.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cập nhật lớp học phần thành công!");
                this.Close();
            }
        }
    }
}
