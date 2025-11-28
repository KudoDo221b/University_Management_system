using System;
using MySql.Data.MySqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_system
{
    /// <summary>
    /// Interaction logic for ManageAdvisorClassPage.xaml
    /// </summary>
    public partial class ManageAdvisorClassPage : Page
    {
        public ManageAdvisorClassPage()
        {
            InitializeComponent();
            LoadAdvisorClass();
        }

        private void LoadAdvisorClass()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT cc.teacher_id, cc.khoa, cc.advisorClass_name, t.full_name
                                        FROM Advisor_Class cc
                                        LEFT JOIN teacher t
                                        ON cc.teacher_id = t.teacher_id;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgAdvisorClass.ItemsSource = dt.DefaultView;
            }
        }

        private DataRowView GetSelected()
        {
            return dtgAdvisorClass.SelectedItem as DataRowView;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddAdvisorClassWindow add = new AddAdvisorClassWindow();
            add.ShowDialog();
            LoadAdvisorClass();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (keyword == "")
            {
                LoadAdvisorClass();
                return;
            }

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT cc.teacher_id, cc.khoa, cc.advisorClass_name, t.full_name
                                        FROM Advisor_Class cc
                                        LEFT JOIN teacher t
                                        ON cc.teacher_id = t.teacher_id;
                            WHERE teacher_id LIKE @kw 
                               OR advisorClass_name LIKE @kw 
                               OR full_name LIKE @kw";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["STT"] = i + 1;

                dtgAdvisorClass.ItemsSource = dt.DefaultView;
            }
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            MessageBox.Show(
                $"Mã giảng viên: {row["teacher_id"]}\n" +
                $"Họ tên giảng viên: {row["full_name"]}\n" +
                $"Khóa: {row["khoa"]}\n" +
                $"Lớp cố vấn: {row["advisorClass_name"]}\n",
                "Chi tiết lớp học phần",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            if (MessageBox.Show("Xóa lớp cố vấn này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM Advisor_Class WHERE teacher_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", row["teacher_id"]);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Đã xóa lớp có vấn thành công");
            LoadAdvisorClass();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var row = GetSelected();
            if (row == null) return;

            EditAdvisorClassWindow edit = new EditAdvisorClassWindow(row["teacher_id"].ToString());
            edit.ShowDialog();

            LoadAdvisorClass();
        }
    }
}
