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

namespace Management_system
{
    public partial class ViewReportPage : Page
    {
        public ViewReportPage()
        {
            InitializeComponent();
            LoadReports();
        }

        // LOAD REPORT
        private void LoadReports()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT id, content, request_date FROM report ORDER BY request_date DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dtgReports.ItemsSource = dt.DefaultView;
            }
        }

        // NÚT XEM CHI TIẾT
        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var row = dtgReports.SelectedItem as DataRowView;
            if (row == null) return;

            MessageBox.Show(
                $"Nội dung phản ánh:\n\n{row["content"]}\n\n" +
                $"Ngày gửi: {row["request_date"]}",
                "Chi tiết phản ánh",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
