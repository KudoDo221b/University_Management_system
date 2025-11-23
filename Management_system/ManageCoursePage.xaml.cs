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
    /// Interaction logic for ManageCoursePage.xaml
    /// </summary>
    public partial class ManageCoursePage : Page
    {
        public ManageCoursePage()
        {
            InitializeComponent();
            LoadCourses();
        }

        // Lấy dòng đang chọn
        private DataRowView GetSelected()
        {
            return dtgCourse.SelectedItem as DataRowView;
        }

        // ========= LOAD DANH SÁCH ==========
        private void LoadCourses()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM course";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgCourse.ItemsSource = dt.DefaultView;
            }
        }

        // ========= CHI TIẾT ===========
        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            MessageBox.Show(
                $"Mã môn: {row["course_id"]}\n" +
                $"Tên môn: {row["name"]}\n" +
                $"Số tín chỉ: {row["credit"]}\n" +
                $"Mô tả: {row["description"]}\n" +
                $"Tiên quyết: {row["pre_requisite"]}"
                , "Thông tin môn học");
        }

        // ========= XÓA ===========
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            if (MessageBox.Show("Xóa môn học này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM course WHERE course_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", row["course_id"]);

                cmd.ExecuteNonQuery();
            }

            LoadCourses();
        }

        // ========= TÌM KIẾM ===========
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "")
            {
                LoadCourses();
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql =
                    "SELECT * FROM course WHERE course_id LIKE @kw OR name LIKE @kw OR description LIKE @kw";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgCourse.ItemsSource = dt.DefaultView;
            }
        }

        // ========= THÊM ===========
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCourseWindow add = new AddCourseWindow();
            add.ShowDialog();
            LoadCourses();
        }

        // ========= SỬA ===========
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            EditCourseWindow edit = new EditCourseWindow(row["course_id"].ToString());
            edit.ShowDialog();
            LoadCourses();
        }
    }
}
