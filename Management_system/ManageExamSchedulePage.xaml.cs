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
    /// <summary>
    /// Interaction logic for ManageExamSchedulePage.xaml
    /// </summary>
    public partial class ManageExamSchedulePage : Page
    {
        public ManageExamSchedulePage()
        {
            InitializeComponent();
            LoadExamList();
        }

        // Load danh sách lịch thi
        private void LoadExamList()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT exam_id, courseClass_id, schedule FROM examschedule";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgExam.ItemsSource = dt.DefaultView;
            }
        }

        private DataRowView GetSelected()
        {
            return dtgExam.SelectedItem as DataRowView;
        }

        // NÚT THÊM
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddExamScheduleWindow add = new AddExamScheduleWindow();
            add.ShowDialog();
            LoadExamList();
        }

        // NÚT SỬA
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            EditExamScheduleWindow edit = new EditExamScheduleWindow(row["exam_id"].ToString());
            edit.ShowDialog();

            LoadExamList();
        }

        // NÚT XÓA
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            if (MessageBox.Show("Xóa lịch thi này?", "Xác nhận",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM examschedule WHERE exam_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", row["exam_id"]);
                cmd.ExecuteNonQuery();
            }

            LoadExamList();
        }

        // TÌM KIẾM
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "")
            {
                LoadExamList();
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT * FROM examschedule WHERE exam_id LIKE @kw OR courseClass_id LIKE @kw";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgExam.ItemsSource = dt.DefaultView;
            }
        }
    }
}
