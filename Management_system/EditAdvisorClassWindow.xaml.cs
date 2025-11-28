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
    /// Interaction logic for EditAdvisorClassWindow.xaml
    /// </summary>
    public partial class EditAdvisorClassWindow : Window
    {
        private string teacherId;

        public EditAdvisorClassWindow(string id)
        {
            InitializeComponent();
            teacherId = id;
            LoadAdvisorClass();
        }

        private void LoadAdvisorClass()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT cc.teacher_id, cc.khoa, cc.advisorClass_name, t.full_name
                       FROM Advisor_Class cc
                       LEFT JOIN teacher t ON cc.teacher_id = t.teacher_id
                       WHERE cc.teacher_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", teacherId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTeacherId.Text = reader["teacher_id"].ToString();
                            txtFullName.Text = reader["full_name"].ToString();
                            txtKhoa.Text = reader["khoa"].ToString();
                            txtAdvisorClass.Text = reader["advisorClass_name"].ToString();
                        }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                // 1) UPDATE bảng Advisor_Class
                string sql1 = @"UPDATE Advisor_Class
                            SET khoa = @khoa,
                            advisorClass_name = @advisorClass
                            WHERE teacher_id = @id";

                MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                cmd1.Parameters.AddWithValue("@khoa", txtKhoa.Text.Trim());
                cmd1.Parameters.AddWithValue("@advisorClass", txtAdvisorClass.Text.Trim());
                cmd1.Parameters.AddWithValue("@id", teacherId);
                cmd1.ExecuteNonQuery();

                // 2) UPDATE bảng teacher (cập nhật full name)
                string sql2 = @"UPDATE teacher
                                SET full_name = @fullname
                                WHERE teacher_id = @id";

                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                cmd2.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim());
                cmd2.Parameters.AddWithValue("@id", teacherId);
                cmd2.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thành công!");
                this.Close();
            }
        }
    }
}
