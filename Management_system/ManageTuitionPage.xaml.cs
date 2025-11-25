using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ManageTuitionPage.xaml
    /// </summary>
    public partial class ManageTuitionPage : Page
    {
        public class TuitionModel
        {
            public int STT { get; set; }
            public string student_id { get; set; }
            public string full_name { get; set; }
            public string tuitionStatus { get; set; }
            public string bill_url { get; set; }
        }

        private List<TuitionModel> tuitionList = new List<TuitionModel>();

        public ManageTuitionPage()
        {
            InitializeComponent();
            LoadTuition();
        }
        private void LoadTuition()
        {
            tuitionList.Clear();

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT tp.student_id, st.full_name, tp.tuitionStatus, tp.bill_url
                    FROM tuition_processing tp
                    JOIN student st ON tp.student_id = st.student_id
                    WHERE tp.tuitionStatus = 'Chờ xét duyệt'
                ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                var rd = cmd.ExecuteReader();

                int index = 1;
                while (rd.Read())
                {
                    tuitionList.Add(new TuitionModel
                    {
                        STT = index++,
                        student_id = rd["student_id"].ToString(),
                        full_name = rd["full_name"].ToString(),
                        tuitionStatus = rd["tuitionStatus"].ToString(),
                        bill_url = rd["bill_url"].ToString()
                    });
                }
            }

            dtgTuition.ItemsSource = null;
            dtgTuition.ItemsSource = tuitionList;
        }
        

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int stt = int.Parse(btn.Tag.ToString());

            TuitionModel selected = tuitionList.Find(x => x.STT == stt);
            if (selected == null) return;

            string studentId = selected.student_id;
            string adminId = App.AdminID; //Lấy id admin đã login

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                MySqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // Cập nhật tuition_processing
                    string q1 = @"
                        UPDATE tuition_processing 
                        SET tuitionStatus='Đã nộp', admin_id=@admin, approve_date = NOW()
                        WHERE student_id=@sid
                    ";
                    MySqlCommand cmd1 = new MySqlCommand(q1, conn, trans);
                    cmd1.Parameters.AddWithValue("@admin", adminId);
                    cmd1.Parameters.AddWithValue("@sid", studentId);
                    cmd1.ExecuteNonQuery();

                    // Cập nhật student
                    string q2 = @"
                        UPDATE student 
                        SET tuitionStatus='Đã nộp' 
                        WHERE student_id=@sid
                    ";
                    MySqlCommand cmd2 = new MySqlCommand(q2, conn, trans);
                    cmd2.Parameters.AddWithValue("@sid", studentId);
                    cmd2.ExecuteNonQuery();

                    trans.Commit();

                    MessageBox.Show("Đã xác nhận học phí!");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Lỗi cập nhật: " + ex.Message);
                }
            }
            LoadTuition();
        }

        private void BtnRefuse_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int stt = int.Parse(btn.Tag.ToString());

            TuitionModel selected = tuitionList.Find(x => x.STT == stt);
            if (selected == null) return;

            string studentId = selected.student_id;
            string adminId = App.AdminID;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();

                MySqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // Update tuition_processing
                    string q1 = @"
                        UPDATE tuition_processing 
                        SET tuitionStatus='Không hợp lệ', admin_id=@admin, approve_date = NOW()
                        WHERE student_id=@sid
                    ";
                    MySqlCommand cmd1 = new MySqlCommand(q1, conn, trans);
                    cmd1.Parameters.AddWithValue("@sid", studentId);
                    cmd1.Parameters.AddWithValue("@admin", adminId);
                    cmd1.ExecuteNonQuery();

                    // Update student
                    string q2 = @"
                        UPDATE student 
                        SET tuitionStatus='Không hợp lệ'
                        WHERE student_id=@sid
                    ";
                    MySqlCommand cmd2 = new MySqlCommand(q2, conn, trans);
                    cmd2.Parameters.AddWithValue("@sid", studentId);
                    cmd2.ExecuteNonQuery();

                    trans.Commit();
                    MessageBox.Show("Đã từ chối!");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Lỗi cập nhật: " + ex.Message);
                }
            }
            LoadTuition();
        }

        

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            var filter = tuitionList.FindAll(x =>
                x.student_id.ToLower().Contains(keyword) ||
                x.full_name.ToLower().Contains(keyword)
            );

            dtgTuition.ItemsSource = null;
            dtgTuition.ItemsSource = filter;
        }
        public static string ConvertGoogleDriveToDirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            string fileId = null;

            // 1. Dạng /file/d/<ID>
            var regex1 = new Regex(@"\/file\/d\/([a-zA-Z0-9_-]+)");
            var match1 = regex1.Match(url);
            if (match1.Success)
                fileId = match1.Groups[1].Value;

            // 2. Dạng ?id=<ID>
            if (fileId == null)
            {
                var regex2 = new Regex(@"id=([a-zA-Z0-9_-]+)");
                var match2 = regex2.Match(url);
                if (match2.Success)
                    fileId = match2.Groups[1].Value;
            }

            // Nếu vẫn không tìm được fileId
            if (fileId == null)
            {
                if (url.Contains("drive.google.com"))
                    throw new Exception("Link Google Drive không đúng định dạng hoặc chưa chia sẻ công khai.");
                else
                    return url; // link thường
            }

            // Tạo direct link
            return $"https://drive.google.com/uc?export=view&id={fileId}";
        }
        private void Bill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is TuitionModel model)
                {
                    string rawUrl = model.bill_url;
                    string imageUrl = ConvertGoogleDriveToDirect(rawUrl);

                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        MessageBox.Show("Đường dẫn hóa đơn bị rỗng hoặc không hợp lệ.");
                        return;
                    }

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    // Kiểm tra ảnh hợp lệ
                    if (bitmap.PixelWidth == 0 || bitmap.PixelHeight == 0)
                    {
                        MessageBox.Show("Không thể hiển thị hóa đơn: Ảnh bị lỗi hoặc không tồn tại.");
                        return;
                    }

                    // Hiện popup ảnh
                    Window popup = new Window
                    {
                        Title = "Xem ảnh hóa đơn",
                        Width = 600,
                        Height = 600,
                        Content = new Image
                        {
                            Source = bitmap,
                            Stretch = System.Windows.Media.Stretch.Uniform
                        },
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = Window.GetWindow(this),
                        ResizeMode = ResizeMode.CanResize
                    };
                    popup.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không thể mở hóa đơn do thông tin bị thiếu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ảnh hóa đơn:\n" + ex.Message);
            }
        }

    }

    
}
