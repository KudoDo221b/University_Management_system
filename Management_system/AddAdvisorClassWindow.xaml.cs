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
    /// Interaction logic for AddAdvisorClassWindow.xaml
    /// </summary>
    public partial class AddAdvisorClassWindow : Window
    {
        public AddAdvisorClassWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string teacherId = txtTeacherId.Text.Trim();
            string khoa = txtKhoa.Text.Trim();
            string advisorClass = txtAdvisorClass.Text.Trim();

            // ====== KIỂM TRA RỖNG ======
            if (teacherId == "" || khoa == "" || advisorClass == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                // ====== CHECK TEACHER TỒN TẠI TRONG BẢNG TEACHER ======
                string checkTeacherSql = "SELECT COUNT(*) FROM teacher WHERE teacher_id = @id";
                MySqlCommand checkCmd = new MySqlCommand(checkTeacherSql, conn);
                checkCmd.Parameters.AddWithValue("@id", teacherId);

                int countTeacher = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (countTeacher == 0)
                {
                    MessageBox.Show("Mã giảng viên không tồn tại trong hệ thống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string insertSql = @"INSERT INTO Advisor_Class (teacher_id, khoa, advisorClass_name)
                                    VALUES (@teacherId, @khoa, @className)";

                using (MySqlCommand cmdAdvisorClass = new MySqlCommand(insertSql, conn))
                {
                    cmdAdvisorClass.Parameters.AddWithValue("@teacherId", teacherId);
                    cmdAdvisorClass.Parameters.AddWithValue("@khoa", khoa);
                    cmdAdvisorClass.Parameters.AddWithValue("@className", advisorClass);

                    cmdAdvisorClass.ExecuteNonQuery();
                }
                

                MessageBox.Show("Thêm lớp cố vấn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }
        
    }
}
