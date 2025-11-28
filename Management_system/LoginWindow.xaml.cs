using AdminPortal;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Password.Trim();

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Admin WHERE admin_id=@user AND password=@pass";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    MySqlDataReader rd = cmd.ExecuteReader();

                    if (rd.Read())
                    {
                        Admin.adminId = rd["admin_id"].ToString();
                        Admin.adminName = rd["admin_name"].ToString();
                        Admin.gender = rd["gender"].ToString();
                        Admin.email = rd["email"].ToString();
                        Admin.phone = rd["phone"].ToString();
                        Admin.address = rd["address"].ToString();

                        //Lưu vào biến cục bộ App
                        App.AdminID = Admin.adminId;
                        App.AdminName = Admin.adminName;
                        // Đăng nhập thành công
                        MainWindow home = new MainWindow();
                        home.Show();
                        MessageBox.Show("Đăng nhập: " + Admin.adminName);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!",
                                        "Thông báo",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối DB: " + ex.Message);
                }
            }

        }


        private void TxtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
