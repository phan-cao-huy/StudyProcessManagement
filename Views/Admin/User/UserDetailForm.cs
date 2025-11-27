using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using StudyProcessManagement.Business.Admin;
using StudyProcessManagement.Models;

namespace StudyProcessManagement.Views.Admin.User
{
    public partial class UserDetailForm : Form
    {
        private UserService userService;
        private string _userId;
        private bool _isViewOnly;

        public UserDetailForm(string userId = null, bool isViewOnly = false)
        {
            InitializeComponent();

            if (this.DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            userService = new UserService();
            _userId = userId;
            _isViewOnly = isViewOnly;

            InitializeFormData();
            InitializeValidationEvents();

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void InitializeFormData()
        {
            cboRole.Items.Clear();
            cboRole.Items.AddRange(new string[] { "Admin", "Teacher", "Student" });
            cboRole.SelectedIndex = 2;

            dtpDob.Value = DateTime.Now.AddYears(-18);

            if (_userId == null)
            {
                lblTitle.Text = "THÊM NGƯỜI DÙNG MỚI";
                chkActive.Visible = false;
            }
            else
            {
                LoadUserData();

                if (_isViewOnly)
                {
                    lblTitle.Text = "CHI TIẾT NGƯỜI DÙNG";
                    SetViewOnlyMode();
                }
                else
                {
                    lblTitle.Text = "CẬP NHẬT THÔNG TIN";
                    txtEmail.Enabled = false;
                    lblPassword.Visible = true;
                    txtPassword.Visible = true;
                    txtPassword.Text = "";
                }
            }
        }

        private void InitializeValidationEvents()
        {
            // Validation khi rời khỏi textbox
            txtName.Leave += TxtName_Leave;
            txtEmail.Leave += TxtEmail_Leave;
            txtPhone.Leave += TxtPhone_Leave;
            txtPassword.Leave += TxtPassword_Leave;

            // Chỉ cho phép nhập số vào số điện thoại
            txtPhone.KeyPress += TxtPhone_KeyPress;
        }

        private void TxtName_Leave(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Họ và tên không được để trống!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (name.Length < 3)
            {
                MessageBox.Show("Họ và tên phải có ít nhất 3 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (name.Length > 100)
            {
                MessageBox.Show("Họ và tên không được vượt quá 100 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Kiểm tra chỉ chứa chữ cái, khoảng trắng và dấu tiếng Việt
            if (!Regex.IsMatch(name, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                MessageBox.Show("Họ và tên chỉ được chứa chữ cái và khoảng trắng!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
            }
        }

        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email không được để trống!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Regex kiểm tra định dạng email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Email không đúng định dạng! (Ví dụ: user@example.com)", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (email.Length > 100)
            {
                MessageBox.Show("Email không được vượt quá 100 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
            }
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số và phím Backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void TxtPhone_Leave(object sender, EventArgs e)
        {
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone))
            {
                return; // Số điện thoại không bắt buộc
            }

            // Kiểm tra độ dài số điện thoại (10-11 số)
            if (phone.Length < 10 || phone.Length > 11)
            {
                MessageBox.Show("Số điện thoại phải có 10-11 chữ số!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            // Kiểm tra số điện thoại Việt Nam bắt đầu bằng 0
            if (!phone.StartsWith("0"))
            {
                MessageBox.Show("Số điện thoại phải bắt đầu bằng số 0!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
            }
        }

        private void TxtPassword_Leave(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            // Nếu đang cập nhật và không nhập password thì bỏ qua
            if (_userId != null && string.IsNullOrEmpty(password))
            {
                return;
            }

            // Nếu đang thêm mới thì bắt buộc nhập password
            if (_userId == null && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Kiểm tra độ dài mật khẩu
            if (!string.IsNullOrEmpty(password) && password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(password) && password.Length > 50)
            {
                MessageBox.Show("Mật khẩu không được vượt quá 50 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
            }
        }

        private bool ValidateAllFields()
        {
            // Validate Họ tên
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Họ và tên không được để trống!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (name.Length < 3 || name.Length > 100)
            {
                MessageBox.Show("Họ và tên phải có từ 3-100 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (!Regex.IsMatch(name, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                MessageBox.Show("Họ và tên chỉ được chứa chữ cái và khoảng trắng!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Validate Email
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email không được để trống!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Email không đúng định dạng!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate Số điện thoại (không bắt buộc)
            string phone = txtPhone.Text.Trim();
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (phone.Length < 10 || phone.Length > 11 || !phone.StartsWith("0"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! (10-11 số, bắt đầu bằng 0)", "Lỗi nhập liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus();
                    return false;
                }
            }

            // Validate Password
            string password = txtPassword.Text;
            if (_userId == null && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Mật khẩu không được để trống khi thêm mới!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            if (!string.IsNullOrEmpty(password) && (password.Length < 6 || password.Length > 50))
            {
                MessageBox.Show("Mật khẩu phải có từ 6-50 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate Vai trò
            if (cboRole.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn vai trò!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboRole.Focus();
                return false;
            }

            // Validate Ngày sinh
            if (dtpDob.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không được lớn hơn ngày hiện tại!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDob.Focus();
                return false;
            }

            int age = DateTime.Now.Year - dtpDob.Value.Year;
            if (age < 5 || age > 100)
            {
                MessageBox.Show("Tuổi phải trong khoảng 5-100!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDob.Focus();
                return false;
            }

            // Validate Địa chỉ (không bắt buộc nhưng kiểm tra độ dài nếu có nhập)
            string address = txtAddress.Text.Trim();
            if (!string.IsNullOrWhiteSpace(address) && address.Length > 200)
            {
                MessageBox.Show("Địa chỉ không được vượt quá 200 ký tự!", "Lỗi nhập liệu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            return true;
        }

        private void LoadUserData()
        {
            var user = userService.GetUserById(_userId);
            if (user != null)
            {
                txtName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.PhoneNumber;
                cboRole.SelectedItem = user.Role;
                chkActive.Checked = user.IsActive;
                txtAddress.Text = user.Address;

                if (user.DateOfBirth.HasValue)
                {
                    dtpDob.Value = user.DateOfBirth.Value;
                }
            }
        }

        private void SetViewOnlyMode()
        {
            gbAccount.Enabled = false;
            gbProfile.Enabled = false;
            btnSave.Visible = false;
            btnCancel.Text = "Đóng";
            txtPassword.Visible = false;
            lblPassword.Visible = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate tất cả các trường trước khi lưu
            if (!ValidateAllFields())
            {
                return;
            }

            var userModel = new Users()
            {
                UserID = _userId,
                FullName = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                Role = cboRole.Text,
                IsActive = chkActive.Checked,
                Address = txtAddress.Text.Trim(),
                DateOfBirth = dtpDob.Value
            };

            bool success = false;
            try
            {
                if (_userId == null)
                {
                    success = userService.AddUser(userModel, txtPassword.Text);
                }
                else
                {
                    success = userService.UpdateUser(userModel, txtPassword.Text.Trim());
                }

                if (success)
                {
                    MessageBox.Show("Thao tác thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Thất bại! Có thể email đã tồn tại.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserDetailForm_Load(object sender, EventArgs e)
        {
            // Form load event
        }
    }
}