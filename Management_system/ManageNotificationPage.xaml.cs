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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_system.Pages
{
    /// <summary>
    /// Interaction logic for NotificationPage.xaml
    /// </summary>
    public partial class ManageNotificationPage : Page
    {
        public ManageNotificationPage()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM notification ORDER BY date DESC";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));

                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgNotification.ItemsSource = dt.DefaultView;
            }
        }

        private DataRowView GetSelected()
        {
            return dtgNotification.SelectedItem as DataRowView;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNotificationWindow win = new AddNotificationWindow();
            win.ShowDialog();

            LoadNotifications();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM notification WHERE title LIKE @kw OR content LIKE @kw";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgNotification.ItemsSource = dt.DefaultView;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            if (MessageBox.Show("Xóa thông báo này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM notification WHERE noti_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", row["noti_id"]);

                cmd.ExecuteNonQuery();
            }

            LoadNotifications();
        }
        
    }
}
