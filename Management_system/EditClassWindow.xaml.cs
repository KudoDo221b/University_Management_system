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
                string sql = "SELECT * FROM courseclass WHERE courseClass_id=@id";
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
                    txtExamSchedule.Text = row["examSchedule"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    UPDATE courseclass SET
                        course_id=@course,
                        room=@room,
                        learnSchedule=@learn,
                        duration=@duration,
                        semester=@semester,
                        year=@year,
                        status=@status,
                        capacity=@capacity,
                        examSchedule=@exam
                    WHERE courseClass_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", classId);
                cmd.Parameters.AddWithValue("@course", txtCourseId.Text.Trim());
                cmd.Parameters.AddWithValue("@room", txtRoom.Text.Trim());
                cmd.Parameters.AddWithValue("@learn", txtSchedule.Text.Trim());
                cmd.Parameters.AddWithValue("@duration", txtDuration.Text.Trim());
                cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());
                cmd.Parameters.AddWithValue("@year", txtYear.Text.Trim());
                cmd.Parameters.AddWithValue("@status", txtStatus.Text.Trim());
                cmd.Parameters.AddWithValue("@capacity", txtCapacity.Text.Trim());
                cmd.Parameters.AddWithValue("@exam", txtExamSchedule.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật lớp học phần thành công!");
                this.Close();
            }
        }
    }
}
