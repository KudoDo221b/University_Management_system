using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Management_system
{
    public partial class EditAdminWindow : Window
    {
        private string adminId;
        private string currentAvatar;

        public EditAdminWindow(string id)
        {
            InitializeComponent();
            adminId = id;
            LoadAdminInfo();
        }

        private void LoadAdminInfo()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM admin WHERE admin_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", adminId);

                MySqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    txtId.Text = rd["admin_id"].ToString();
                    txtName.Text = rd["admin_name"].ToString();
                    txtEmail.Text = rd["email"].ToString();
                    txtPhone.Text = rd["phone"].ToString();
                    txtAddress.Text = rd["address"].ToString();
                    txtPassword.Password = rd["password"].ToString();

                    currentAvatar = rd["avatar"]?.ToString();

                    LoadAvatar(currentAvatar);

                    string gender = rd["gender"].ToString();
                    cbGender.SelectedIndex = (gender == "Nữ") ? 1 : 0;
                }
            }
        }

        private void LoadAvatar(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                    bmp.EndInit();

                    imgAvatar.Fill = new ImageBrush(bmp);
                }
                else
                {
                    imgAvatar.Fill = new ImageBrush(
                        new BitmapImage(new Uri("images/admin_avatar.png", UriKind.Relative)));
                }
            }
            catch
            {
                imgAvatar.Fill = new ImageBrush(
                    new BitmapImage(new Uri("images/admin_avatar.png", UriKind.Relative)));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string gender = ((ComboBoxItem)cbGender.SelectedItem).Content.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string password = txtPassword.Password;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    UPDATE admin SET 
                        admin_name=@name,
                        gender=@gender,
                        email=@email,
                        phone=@phone,
                        address=@address,
                        password=@pwd
                    WHERE admin_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@pwd", password);
                cmd.Parameters.AddWithValue("@id", adminId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}

