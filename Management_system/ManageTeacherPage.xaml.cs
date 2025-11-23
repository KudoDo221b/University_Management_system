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
    /// Interaction logic for QuanLySinhVienPage.xaml
    /// </summary>
   
        public partial class ManageTeacherPage : Page
        {
            public ManageTeacherPage()
            {
                InitializeComponent();
                LoadTeachers();
            }

            // ============================
            // 1. LOAD DANH SÁCH GIẢNG VIÊN
            // ============================
            private void LoadTeachers()
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query =
                        "SELECT teacher_id, full_name, gender, email, phone, address, faculty FROM teacher";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Thêm cột STT
                    dt.Columns.Add("STT", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                        dt.Rows[i]["STT"] = i + 1;

                    dtgTeacher.ItemsSource = dt.DefaultView;
                }
            }

            private DataRowView GetSelected()
            {
                return dtgTeacher.SelectedItem as DataRowView;
            }

            // ============================
            // 2. NÚT CHI TIẾT
            // ============================
            private void BtnDetail_Click(object sender, RoutedEventArgs e)
            {
                var row = GetSelected();
                if (row == null) return;

                MessageBox.Show(
                    $"MGV: {row["teacher_id"]}\n" +
                    $"Họ tên: {row["full_name"]}\n" +
                    $"Giới tính: {row["gender"]}\n" +
                    $"Email: {row["email"]}\n" +
                    $"SDT: {row["phone"]}\n" +
                    $"Địa chỉ: {row["address"]}\n" +
                    $"Khoa: {row["faculty"]}",
                    "Chi tiết giảng viên",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }

            // ============================
            // 3. XÓA GIẢNG VIÊN
            // ============================
            private void BtnDelete_Click(object sender, RoutedEventArgs e)
            {
                var row = GetSelected();
                if (row == null) return;

                if (MessageBox.Show("Xóa giảng viên này?",
                    "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Xóa đúng bảng teacher
                    string sql = "DELETE FROM teacher WHERE teacher_id=@id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", row["teacher_id"]);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể xóa vì có dữ liệu liên quan!\n" + ex.Message);
                    }
                }

                LoadTeachers();
            }

            // ============================
            // 4. SỬA GIẢNG VIÊN
            // ============================
            private void BtnEdit_Click(object sender, RoutedEventArgs e)
            {
                var row = GetSelected();
                if (row == null) return;

                EditTeacherWindow edit = new EditTeacherWindow(row["teacher_id"].ToString());
                edit.ShowDialog();

                LoadTeachers();
            }

            // ============================
            // 5. THÊM GIẢNG VIÊN
            // ============================
            private void BtnAdd_Click(object sender, RoutedEventArgs e)
            {
                AddTeacherWindow add = new AddTeacherWindow();
                add.ShowDialog();
                LoadTeachers();
            }

            // ============================
            // 6. TÌM KIẾM GIẢNG VIÊN
            // ============================
            private void BtnSearch_Click(object sender, RoutedEventArgs e)
            {
                string keyword = txtSearch.Text.Trim();
                if (keyword == "")
                {
                    LoadTeachers();
                    return;
                }

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string sql =
                        "SELECT teacher_id, full_name, gender, email, phone, address, faculty " +
                        "FROM teacher " +
                        "WHERE teacher_id LIKE @kw OR full_name LIKE @kw OR faculty LIKE @kw";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("STT", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                        dt.Rows[i]["STT"] = i + 1;

                    dtgTeacher.ItemsSource = dt.DefaultView;
                }
            }
        }
    }
