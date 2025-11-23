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
    /// Interaction logic for AddCourseWindow.xaml
    /// </summary>
    public partial class AddCourseWindow : Window
    {
        public AddCourseWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string id = txtCourseId.Text.Trim();
            string name = txtName.Text.Trim();
            string credit = txtCredit.Text.Trim();
            string desc = txtDescription.Text.Trim();
            string prereq = txtPrereq.Text.Trim();

            if (id == "" || name == "" || credit == "")
            {
                MessageBox.Show("Vui lòng nhập đủ Mã môn, Tên môn và Tín chỉ!");
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    INSERT INTO course(course_id, name, credit, description, pre_requisite)
                    VALUES (@id, @name, @credit, @desc, @prereq)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@credit", credit);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@prereq", prereq == "" ? null : prereq);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm môn học thành công!");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Lỗi: Mã môn bị trùng hoặc dữ liệu không hợp lệ!");
                }
            }
        }
    }
}
