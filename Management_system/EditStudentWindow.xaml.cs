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
    public partial class EditStudentWindow : Window
    {
        private string studentID;

        public EditStudentWindow(string id)
        {
            InitializeComponent();
            studentID = id;
            LoadData();
        }

        private void LoadData()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM student WHERE student_id=@id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentID);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0) return;

                var r = dt.Rows[0];

                txtID.Text = r["student_id"].ToString();
                txtName.Text = r["full_name"].ToString();
                txtEmail.Text = r["email"].ToString();
                txtPhone.Text = r["phone"].ToString();
                txtAddress.Text = r["address"].ToString();
                txtKhoa.Text = r["khoa"].ToString();
                txtMajor.Text = r["major"].ToString();

                // Giới tính
                if (r["gender"].ToString() == "Nam")
                    cbGender.SelectedIndex = 0;
                else
                    cbGender.SelectedIndex = 1;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE student SET 
                                full_name=@name,
                                gender=@gender,
                                email=@email,
                                phone=@phone,
                                address=@address,
                                khoa=@khoa,
                                major=@major
                               WHERE student_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", txtID.Text);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@gender",
                    (cbGender.SelectedItem as ComboBoxItem).Content.ToString());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@khoa", txtKhoa.Text);
                cmd.Parameters.AddWithValue("@major", txtMajor.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo");
            this.Close();
        }
    }
}
