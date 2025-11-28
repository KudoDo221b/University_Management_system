using Management_system;
using Management_system.Pages;
using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace AdminPortal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TestDB_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Kết nối OK!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("kết nối thất bại");
                }
            }
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new HomePage();
        }

        private void BtnForum_Click(object sender, RoutedEventArgs e)
        {          
                 MainFrame.Navigate(new WebBrowserPage("https://af.duytan.edu.vn/sites/index.aspx"));                  
        }

        private void BtnEmail_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WebBrowserPage("https://mail.google.com"));
        }

        private void BtnELib_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Account_Info(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AccountPage();
        }

        private void BtnQLSinhVien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageStudentPage();
        }

        private void BtnQLGiangVien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageTeacherPage();
        }

        private void BtnQLMonHoc_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageCoursePage();
        }

        private void BtnQLLop_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageClassPage();
        }
            
        private void BtnQLLich_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageExamSchedulePage();
        }

        private void BtnQLThongBao_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageNotificationPage();
        }

        private void BtnHDSD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnQLHocPhi_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageTuitionPage();
        }

        private void BtnQLLopCoVan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManageAdvisorClassPage();
        }
        private void BtnXBaoCao_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ViewReportPage();
        }
    }
}
