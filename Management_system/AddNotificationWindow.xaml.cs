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
    /// Interaction logic for AddNotificationWindow.xaml
    /// </summary>
    public partial class AddNotificationWindow : Window
    {
        public AddNotificationWindow()
        {
            InitializeComponent();
            cbTarget.SelectedIndex = 0;
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string content = txtContent.Text.Trim();
            string target = (cbTarget.SelectedItem as ComboBoxItem).Content.ToString();

            if (title == "" || content == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                
            string sql = @"
INSERT INTO notification(`title`, `content`, `date`, `target`)
VALUES(@title, @content, NOW(), @target)";



                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@content", content);
                cmd.Parameters.AddWithValue("@target", target);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Gửi thông báo thành công!");
            this.Close();
        }

    }
}
