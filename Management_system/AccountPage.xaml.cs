using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
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

        public static string ConvertGoogleDriveToDirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            var match1 = System.Text.RegularExpressions.Regex.Match(url,
                @"\/file\/d\/([a-zA-Z0-9_-]+)");

            if (match1.Success)
                return $"https://drive.google.com/uc?export=view&id={match1.Groups[1].Value}";

            var match2 = System.Text.RegularExpressions.Regex.Match(url,
                @"id=([a-zA-Z0-9_-]+)");

            if (match2.Success)
                return $"https://drive.google.com/uc?export=view&id={match2.Groups[1].Value}";

            return url;
        }

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

                    string avatar = reader["avatar"]?.ToString();
                    LoadAvatar(avatar);
                }
            }
        }

        private void LoadAvatar(string avatar)
        {
            try
            {
                if (string.IsNullOrEmpty(avatar))
                {
                    SetDefaultAvatar();
                    return;
                }

                // Avatar cục bộ (images/...)
                if (File.Exists("images/" + avatar))
                {
                    imgAvatar.Fill = new ImageBrush(
                        new BitmapImage(new Uri("images/" + avatar, UriKind.Relative))
                    );
                    return;
                }

                // Google Drive
                if (avatar.Contains("drive.google.com"))
                {
                    string url = ConvertGoogleDriveToDirect(avatar);

                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(url, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();

                    imgAvatar.Fill = new ImageBrush(bmp);
                    return;
                }

                // Link http bình thường
                imgAvatar.Fill = new ImageBrush(new BitmapImage(new Uri(avatar)));
            }
            catch
            {
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {
            imgAvatar.Fill = new ImageBrush(
                new BitmapImage(new Uri("images/admin_avatar.png", UriKind.Relative))
            );
        }

        private void EditInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditAdminWindow edit = new EditAdminWindow(currentAdminId);
            edit.ShowDialog();
            LoadAdminInfo();  // load lại sau khi sửa
        }
    }
}
