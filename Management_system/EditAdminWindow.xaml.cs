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
        private string newAvatarFile = null;

        public EditAdminWindow(string id)
        {
            InitializeComponent();
            adminId = id;
            LoadAdminInfo();
        }

        // Convert link Google Drive
        private string ConvertGoogleDrive(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            string id = null;

            if (url.Contains("/file/d/"))
            {
                int s = url.IndexOf("/file/d/") + 8;
                int e = url.IndexOf("/", s);
                id = url.Substring(s, e - s);
            }

            if (id == null) return url;

            return $"https://drive.google.com/uc?export=view&id={id}";
        }

        // LOAD ADMIN
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

                    string gender = rd["gender"].ToString();
                    cbGender.SelectedIndex = (gender == "Nữ") ? 1 : 0;

                    currentAvatar = rd["avatar"]?.ToString();

                    LoadAvatar(currentAvatar);
                }
            }
        }

        private void LoadAvatar(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string imgUrl = ConvertGoogleDrive(url);

                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(imgUrl, UriKind.Absolute);
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

        // CHỌN ẢNH
        private void BtnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ảnh (.png; .jpg)|*.png;*.jpg";

            if (dlg.ShowDialog() == true)
            {
                newAvatarFile = dlg.FileName;

                BitmapImage bmp = new BitmapImage(new Uri(newAvatarFile));
                imgAvatar.Fill = new ImageBrush(bmp);
            }
        }

        // XÓA ẢNH
        private void BtnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            newAvatarFile = null;
            currentAvatar = null;

            imgAvatar.Fill = new ImageBrush(
                new BitmapImage(new Uri("images/admin_avatar.png", UriKind.Relative)));
        }

        // NÚT LƯU THAY ĐỔI
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string gender = ((ComboBoxItem)cbGender.SelectedItem).Content.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string password = txtPassword.Password;

            string finalAvatar = currentAvatar;

            // Nếu chọn ảnh mới → upload lên thư mục images
            if (newAvatarFile != null)
            {
                string fileName = System.IO.Path.GetFileName(newAvatarFile);
                string destPath = "images/" + fileName;

                File.Copy(newAvatarFile, destPath, true);

                finalAvatar = destPath;
            }

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
                        password=@pwd,
                        avatar=@avatar
                    WHERE admin_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@pwd", password);
                cmd.Parameters.AddWithValue("@avatar", finalAvatar);
                cmd.Parameters.AddWithValue("@id", adminId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}
