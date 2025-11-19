using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;
using System;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using StudyProcessManagement.Views.Login;
namespace StudyProcessManagement.Views.SinhVien.assignments
{
    public partial class assignments : Form
    {
        private WebView2 webView;

        public assignments()
        {
            InitializeComponent();
            // Gắn trình xử lý sự kiện Load
            this.Load += assignments_Load;
        }

        private async void assignments_Load(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo WebView2 control VÀ thêm vào Form TRƯỚC khi gọi EnsureCoreWebView2Async
                webView = new WebView2
                {
                    Dock = DockStyle.Fill
                };

                // Thêm vào Controls và đưa lên trước
                this.Controls.Add(webView);

                webView.BringToFront();

                // Đảm bảo runtime CoreWebView2 được khởi tạo
                await webView.EnsureCoreWebView2Async(null);

                
                var htmlRelative = Path.Combine("Views", "SinhVien", "assignments", "student-assignments.html");
                var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, htmlRelative);

                if (!File.Exists(htmlPath))
                {
                    MessageBox.Show($"Lỗi: Không tìm thấy file HTML: {htmlPath}\n\nĐảm bảo thuộc tính 'Copy to Output Directory' của file được đặt là 'Copy if newer' hoặc 'Copy always'.", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

                    // Gắn sự kiện để gửi dữ liệu sau khi trang tải xong
                    webView.NavigationCompleted += WebView_NavigationCompleted;
                }

                // Điều hướng tới file cục bộ
                webView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Khởi tạo WebView2 thất bại: {ex.Message}", "WebView2 error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            
                if (e.IsSuccess && webView.CoreWebView2 != null)
                {
                    // 1. Chuẩn bị dữ liệu C#
                    var dataToSend = new AssignmentData();
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(dataToSend);

                    // 2. Escape chuỗi JSON cho JavaScript
                    string escapedJson = jsonString.Replace("\\", "\\\\").Replace("'", "\\'");

                    // 3. Gọi hàm JavaScript 
                    string script = $"receiveDataFromCSharp('{escapedJson}')";

                    try
                    {
                        // 4. Sử dụng await an toàn trong async method
                        await webView.CoreWebView2.ExecuteScriptAsync(script);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi thực thi JS: {ex.Message}", "WebView2 Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        private void CoreWebView2_WebMessageReceived(object sender,
        Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {

            string targetFileName = e.TryGetWebMessageAsString();
            if(targetFileName == "LOGOUT")
            {
                StudyProcessManagement.Views.Login.Login loginForm = new StudyProcessManagement.Views.Login.Login();
                loginForm.Show();
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(this.Close));
                }
                else
                {
                    this.Close();
                }
            }

            else if (targetFileName == "student-discover.html")
            {
               

                // 1. Xây dựng đường dẫn tương đối đến file đích (Views\SinhVien\discover\student-discover.html)
                string targetRelativePath = Path.Combine("Views", "SinhVien", "discover", targetFileName);

                // 2. Xây dựng đường dẫn tuyệt đối
                string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetRelativePath);

                if (File.Exists(targetPath))
                {
                    // 3. Thực hiện điều hướng
                    webView.CoreWebView2.Navigate(new Uri(targetPath).AbsoluteUri);
                }
                else
                {
                    MessageBox.Show($"Lỗi: Không tìm thấy file HTML đích: {targetPath}", "Lỗi Điều hướng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        
        public class AssignmentData
        {
            
            public string AssignmentTitle { get; set; } = "Website Phạm Văn Giáp";
            public string CourseName { get; set; } = "Lập trình react với sql";
            public string DueStatus { get; set; } = "đến hạn";
            public string AssignedDate { get; set; } = "01/11/2025";
            public string DueDate { get; set; } = "18/11/2025";
            public string Description { get; set; } = "Xây dựng website bán hàng trực tuyến hoàn chỉnh...";
        }

        // Các phương thức được tạo bởi Designer
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
    }
}