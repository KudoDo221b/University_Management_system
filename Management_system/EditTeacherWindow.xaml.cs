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
    /// Interaction logic for EditTeacherWindow.xaml
    /// </summary>
    public partial class EditTeacherWindow : Window
    {
        private string teacherID;

        public EditTeacherWindow(string id)
        {
            InitializeComponent();
            teacherID = id;
            LoadData();
        }

        private void LoadData()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM teacher WHERE teacher_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", teacherID);

                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    txtID.Text = rd["teacher_id"].ToString();
                    txtName.Text = rd["full_name"].ToString();
                    txtEmail.Text = rd["email"].ToString();
                    txtPhone.Text = rd["phone"].ToString();
                    txtAddress.Text = rd["address"].ToString();
                    txtFaculty.Text = rd["faculty"].ToString();

                    string gender = rd["gender"].ToString();
                    cbGender.SelectedIndex = gender == "Male" ? 0 : 1;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE teacher SET 
                                full_name=@name,
                                gender=@gender,
                                email=@email,
                                phone=@phone,
                                address=@address,
                                faculty=@faculty
                               WHERE teacher_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@gender",
                    (cbGender.SelectedItem as ComboBoxItem)?.Content.ToString());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@faculty", txtFaculty.Text.Trim());
                cmd.Parameters.AddWithValue("@id", teacherID);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Cập nhật thành công!");
            this.Close();
        }
    }
}
