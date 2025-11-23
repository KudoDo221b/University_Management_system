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
using System.Windows.Shapes;

namespace Management_system
{
    /// <summary>
    /// Interaction logic for EditCourseWindow.xaml
    /// </summary>
    public partial class EditCourseWindow : Window
    {
        private string courseId;

        public EditCourseWindow(string id)
        {
            InitializeComponent();
            courseId = id;
            LoadCourse();
        }

        private void LoadCourse()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM course WHERE course_id=@id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", courseId);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    var row = dt.Rows[0];

                    txtCourseId.Text = row["course_id"].ToString();
                    txtName.Text = row["name"].ToString();
                    txtCredit.Text = row["credit"].ToString();
                    txtDescription.Text = row["description"].ToString();
                    txtPrereq.Text = row["pre_requisite"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    UPDATE course 
                    SET name=@name, credit=@credit, description=@desc, pre_requisite=@prereq
                    WHERE course_id=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", courseId);
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@credit", txtCredit.Text.Trim());
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@prereq", txtPrereq.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thành công!");
                this.Close();
            }
        }
    }
}
