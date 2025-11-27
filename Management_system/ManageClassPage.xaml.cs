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
    /// Interaction logic for ManageClassPage.xaml
    /// </summary>
    public partial class ManageClassPage : Page
    {
        public ManageClassPage()
        {
            InitializeComponent();
            LoadClasses();
        }

        private void LoadClasses()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT cc.courseClass_id, cc.course_id, cc.room, cc.learnSchedule,
                                        cc.duration, cc.semester, cc.year, cc.status, cc.capacity, es.schedule
                                        FROM courseclass cc
                                        LEFT JOIN examschedule es
                                        ON cc.courseClass_id = es.courseClass_id;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgClass.ItemsSource = dt.DefaultView;
            }
        }

        private DataRowView GetSelected()
        {
            return dtgClass.SelectedItem as DataRowView;
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            MessageBox.Show(
                $"Mã lớp: {row["courseClass_id"]}\n" +
                $"Mã môn: {row["course_id"]}\n" +
                $"Phòng: {row["room"]}\n" +
                $"Lịch học: {row["learnSchedule"]}\n" +
                $"Thời lượng: {row["duration"]}\n" +
                $"Kỳ: {row["semester"]}\n" +
                $"Năm: {row["year"]}\n" +
                $"Trạng thái: {row["status"]}\n" +
                $"Sức chứa: {row["capacity"]}\n" +
                $"Lịch thi: {row["schedule"]}",
                "Chi tiết lớp học phần",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            if (MessageBox.Show("Xóa lớp học phần này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM courseclass WHERE courseClass_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", row["courseClass_id"]);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Đã xóa lớp học phần thành công");
            LoadClasses();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            EditClassWindow edit = new EditClassWindow(row["courseClass_id"].ToString());
            edit.ShowDialog();

            LoadClasses();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddClassWindow add = new AddClassWindow();
            add.ShowDialog();
            LoadClasses();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "")
            {
                LoadClasses();
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT * FROM courseclass 
                            WHERE courseClass_id LIKE @kw 
                               OR course_id LIKE @kw 
                               OR room LIKE @kw";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgClass.ItemsSource = dt.DefaultView;
            }
        }

       
    }
}
