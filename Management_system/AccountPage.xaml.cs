using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Management_system.Pages
{
    public partial class AccountPage : Page
    {
        string currentAdminId;

        public AccountPage()
        {
            InitializeComponent();
            LoadAdminInfo();
        }

        // =============================================
        // 1. CHUYỂN LINK GOOGLE DRIVE → DIRECT LINK
        // =============================================
        public static string ConvertGoogleDriveToDirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            string fileId = null;

            // Dạng /file/d/<ID>
            var match1 = System.Text.RegularExpressions.Regex.Match(url,
                @"\/file\/d\/([a-zA-Z0-9_-]+)");

            if (match1.Success)
                fileId = match1.Groups[1].Value;

            // Dạng id=...
            if (fileId == null)
            {
                var match2 = System.Text.RegularExpressions.Regex.Match(url,
                    @"id=([a-zA-Z0-9_-]+)");

                if (match2.Success)
                    fileId = match2.Groups[1].Value;
            }

            if (fileId == null)
                return url;

            return $"https://drive.google.com/uc?export=view&id={fileId}";
        }

        // =============================================
        // 2. LOAD THÔNG TIN ADMIN + AVATAR
        // =============================================
        private void LoadAdminInfo()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM admin LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    currentAdminId = reader["admin_id"].ToString();

                    DataContext = new
                    {
                        AdminId = reader["admin_id"].ToString(),
                        AdminName = reader["admin_name"].ToString(),
                        Gender = reader["gender"].ToString(),
                        Email = reader["email"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Address = reader["address"].ToString(),
                    };

                    // =============================
                    // LOAD AVATAR
                    // =============================
                    string avatar = reader["avatar"]?.ToString();

                    if (string.IsNullOrEmpty(avatar))
                    {
                        SetDefaultAvatar();
                        return;
                    }

                    // Nếu là file cục bộ
                    if (File.Exists("images/" + avatar))
                    {
                        imgAvatar.Fill = new ImageBrush(
                            new BitmapImage(new Uri($"images/{avatar}", UriKind.Relative))
                        );
                        return;
                    }

                    // Nếu là link Google Drive
                    if (avatar.Contains("drive.google.com"))
                    {
                        string directUrl = ConvertGoogleDriveToDirect(avatar);

                        try
                        {
                            BitmapImage bmp = new BitmapImage();
                            bmp.BeginInit();
                            bmp.UriSource = new Uri(directUrl, UriKind.Absolute);
                            bmp.CacheOption = BitmapCacheOption.OnLoad;
                            bmp.EndInit();

                            imgAvatar.Fill = new ImageBrush(bmp);
                            return;
                        }
                        catch
                        {
                            SetDefaultAvatar();
                            return;
                        }
                    }

                    // Nếu avatar là link thường (http...)
                    try
                    {
                        imgAvatar.Fill = new ImageBrush(new BitmapImage(new Uri(avatar)));
                    }
                    catch
                    {
                        SetDefaultAvatar();
                    }
                }
            }
        }

        // =============================================
        // 3. ĐẶT AVATAR MẶC ĐỊNH
        // =============================================
        private void SetDefaultAvatar()
        {
            imgAvatar.Fill = new ImageBrush(
                new BitmapImage(new Uri("images/admin_avatar.png", UriKind.Relative))
            );
        }

        // =============================================
        // 4. CẬP NHẬT ẢNH TẢI TỪ MÁY
        // =============================================
        private void UpdateImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ảnh (*.png;*.jpg)|*.png;*.jpg";

            if (dlg.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(dlg.FileName);
                string destPath = "images/" + fileName;

                File.Copy(dlg.FileName, destPath, true);

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE admin SET avatar=@avt WHERE admin_id=@id";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@avt", fileName);
                    cmd.Parameters.AddWithValue("@id", currentAdminId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật ảnh đại diện thành công!");
                LoadAdminInfo();
            }
        }

        // =============================================
        // 5. XÓA ẢNH
        // =============================================
        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE admin SET avatar=NULL WHERE admin_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", currentAdminId);
                cmd.ExecuteNonQuery();
            }

            SetDefaultAvatar();
            MessageBox.Show("Đã xóa ảnh đại diện!");
        }

        // =============================================
        // 6. SỬA THÔNG TIN
        // =============================================
        private void EditInfo_Click(object sender, RoutedEventArgs e)
        {
            EditAdminWindow edit = new EditAdminWindow(currentAdminId);
            edit.ShowDialog();
            LoadAdminInfo();
        }
    }
}
