using ClosedXML.Excel;
using StudyProcessManagement.Business.Teacher;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace StudyProcessManagement.Views.Teacher.Controls
{
    public partial class GradingControl : UserControl
    {
        // ============================================
        // PRIVATE FIELDS
        // ============================================
        private StudentListFormService studentListService;
        private Panel headerPanel;
        private Label lblTitle;
        private Button btnExportExcel;
        private Label lblBreadcrumb;
        private Panel filterPanel;
        private ComboBox cboCourse;
        private ComboBox cboStatus;
        private Panel summaryPanel;
        private Label lblSummaryTitle;
        private Label lblSummaryCount;
        private Panel tablePanel;
        private DataGridView dgvSubmissions;
        private DataGridViewTextBoxColumn colStudent;
        private DataGridViewTextBoxColumn colAssignment;
        private DataGridViewTextBoxColumn colCourse;
        private DataGridViewTextBoxColumn colSubmitDate;
        private DataGridViewTextBoxColumn colDueDate;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colAction;
        private DataGridViewTextBoxColumn colSubmissionID;
        private DataGridViewTextBoxColumn colScore;
        private int hoverRow = -1;

        // Database connection
        private GradingService gradingService = new GradingService();
        private readonly StudentService studentService;
        private string currentTeacherID = "USR002";

        public GradingControl()
        {
            InitializeComponent();

            gradingService = new GradingService();
            studentService = new StudentService();
            studentListService = new StudentListFormService();

            if (!DesignMode)
            {
                LoadCourseFilter();
                LoadSubmissions();
            }
        }
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBreadcrumb = new System.Windows.Forms.Label();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.cboCourse = new System.Windows.Forms.ComboBox();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            // btnExportExcel - NÚT XUẤT EXCEL
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportExcel.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnExportExcel.FlatAppearance.BorderSize = 0;
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportExcel.Location = new System.Drawing.Point(830, 15);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(140, 35);
            this.btnExportExcel.TabIndex = 2;
            this.btnExportExcel.Text = "📊 Xuất Excel";
            this.btnExportExcel.UseVisualStyleBackColor = false;
            this.btnExportExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            this.summaryPanel = new System.Windows.Forms.Panel();
            this.lblSummaryTitle = new System.Windows.Forms.Label();
            this.lblSummaryCount = new System.Windows.Forms.Label();
            this.tablePanel = new System.Windows.Forms.Panel();
            this.dgvSubmissions = new System.Windows.Forms.DataGridView();
            this.colStudent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAssignment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCourse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubmitDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDueDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubmissionID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.headerPanel.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.summaryPanel.SuspendLayout();
            this.tablePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblBreadcrumb);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(30, 20, 30, 20);
            this.headerPanel.Size = new System.Drawing.Size(1000, 100);
            this.headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(256, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Chấm điểm & Phản hồi";
            // 
            // lblBreadcrumb
            // 
            this.lblBreadcrumb.AutoSize = true;
            this.lblBreadcrumb.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBreadcrumb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblBreadcrumb.Location = new System.Drawing.Point(33, 60);
            this.lblBreadcrumb.Name = "lblBreadcrumb";
            this.lblBreadcrumb.Size = new System.Drawing.Size(260, 15);
            this.lblBreadcrumb.TabIndex = 1;
            this.lblBreadcrumb.Text = "Trang chủ  >  Chấm điểm  >  Danh sách bài nộp";
            // 
            // filterPanel
            // 
            this.filterPanel.BackColor = System.Drawing.Color.White;
            this.filterPanel.Controls.Add(this.cboCourse);
            this.filterPanel.Controls.Add(this.btnExportExcel);
            this.filterPanel.Controls.Add(this.cboStatus);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 100);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(30, 15, 30, 15);
            this.filterPanel.Size = new System.Drawing.Size(1000, 70);
            this.filterPanel.TabIndex = 1;
            // 
            // cboCourse
            // 
            this.cboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourse.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboCourse.FormattingEnabled = true;
            this.cboCourse.Location = new System.Drawing.Point(30, 20);
            this.cboCourse.Name = "cboCourse";
            this.cboCourse.Size = new System.Drawing.Size(300, 25);
            this.cboCourse.TabIndex = 0;
            this.cboCourse.SelectedIndexChanged += new System.EventHandler(this.cboCourse_SelectedIndexChanged);
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(350, 20);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(200, 25);
            this.cboStatus.TabIndex = 1;
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            // 
            // summaryPanel
            // 
            this.summaryPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.summaryPanel.Controls.Add(this.lblSummaryTitle);
            this.summaryPanel.Controls.Add(this.lblSummaryCount);
            this.summaryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.summaryPanel.Location = new System.Drawing.Point(0, 170);
            this.summaryPanel.Name = "summaryPanel";
            this.summaryPanel.Padding = new System.Windows.Forms.Padding(30, 15, 30, 15);
            this.summaryPanel.Size = new System.Drawing.Size(1000, 60);
            this.summaryPanel.TabIndex = 2;
            // 
            // lblSummaryTitle
            // 
            this.lblSummaryTitle.AutoSize = true;
            this.lblSummaryTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSummaryTitle.ForeColor = System.Drawing.Color.White;
            this.lblSummaryTitle.Location = new System.Drawing.Point(30, 20);
            this.lblSummaryTitle.Name = "lblSummaryTitle";
            this.lblSummaryTitle.Size = new System.Drawing.Size(146, 20);
            this.lblSummaryTitle.TabIndex = 0;
            this.lblSummaryTitle.Text = "Bài nộp chưa chấm:";
            // 
            // lblSummaryCount
            // 
            this.lblSummaryCount.AutoSize = true;
            this.lblSummaryCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSummaryCount.ForeColor = System.Drawing.Color.White;
            this.lblSummaryCount.Location = new System.Drawing.Point(185, 20);
            this.lblSummaryCount.Name = "lblSummaryCount";
            this.lblSummaryCount.Size = new System.Drawing.Size(18, 20);
            this.lblSummaryCount.TabIndex = 1;
            this.lblSummaryCount.Text = "0";
            // 
            // tablePanel
            // 
            this.tablePanel.BackColor = System.Drawing.Color.White;
            this.tablePanel.Controls.Add(this.dgvSubmissions);
            this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel.Location = new System.Drawing.Point(0, 230);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.Padding = new System.Windows.Forms.Padding(30);
            this.tablePanel.Size = new System.Drawing.Size(1000, 470);
            this.tablePanel.TabIndex = 3;
            // 
            // dgvSubmissions
            // 
            this.dgvSubmissions.AllowUserToAddRows = false;
            this.dgvSubmissions.AllowUserToDeleteRows = false;
            this.dgvSubmissions.AllowUserToResizeRows = false;
            this.dgvSubmissions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSubmissions.BackgroundColor = System.Drawing.Color.White;
            this.dgvSubmissions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSubmissions.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvSubmissions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSubmissions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSubmissions.ColumnHeadersHeight = 40;
            this.dgvSubmissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSubmissions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStudent,
            this.colAssignment,
            this.colCourse,
            this.colSubmitDate,
            this.colDueDate,
            this.colStatus,
            this.colScore,
            this.colAction,
            this.colSubmissionID});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSubmissions.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSubmissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSubmissions.EnableHeadersVisualStyles = false;
            this.dgvSubmissions.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvSubmissions.Location = new System.Drawing.Point(30, 30);
            this.dgvSubmissions.MultiSelect = false;
            this.dgvSubmissions.Name = "dgvSubmissions";
            this.dgvSubmissions.ReadOnly = true;
            this.dgvSubmissions.RowHeadersVisible = false;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.dgvSubmissions.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSubmissions.RowTemplate.Height = 50;
            this.dgvSubmissions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubmissions.Size = new System.Drawing.Size(940, 410);
            this.dgvSubmissions.TabIndex = 0;
            this.dgvSubmissions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSubmissions_CellClick);
            this.dgvSubmissions.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSubmissions_CellMouseLeave);
            this.dgvSubmissions.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSubmissions_CellMouseMove);
            this.dgvSubmissions.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSubmissions_CellPainting);
            // 
            // colStudent
            // 
            this.colStudent.HeaderText = "Học viên";
            this.colStudent.Name = "colStudent";
            this.colStudent.ReadOnly = true;
            // 
            // colAssignment
            // 
            this.colAssignment.HeaderText = "Bài tập";
            this.colAssignment.Name = "colAssignment";
            this.colAssignment.ReadOnly = true;
            // 
            // colCourse
            // 
            this.colCourse.HeaderText = "Khóa học";
            this.colCourse.Name = "colCourse";
            this.colCourse.ReadOnly = true;
            // 
            // colSubmitDate
            // 
            this.colSubmitDate.HeaderText = "Ngày nộp";
            this.colSubmitDate.Name = "colSubmitDate";
            this.colSubmitDate.ReadOnly = true;
            // 
            // colDueDate
            // 
            this.colDueDate.HeaderText = "Hạn nộp";
            this.colDueDate.Name = "colDueDate";
            this.colDueDate.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Trạng thái";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colScore
            // 
            this.colScore.HeaderText = "Điểm";
            this.colScore.Name = "colScore";
            this.colScore.ReadOnly = true;
            // 
            // colAction
            // 
            this.colAction.HeaderText = "Thao tác";
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            // 
            // colSubmissionID
            // 
            this.colSubmissionID.HeaderText = "SubmissionID";
            this.colSubmissionID.Name = "colSubmissionID";
            this.colSubmissionID.ReadOnly = true;
            this.colSubmissionID.Visible = false;
            // 
            // GradingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.tablePanel);
            this.Controls.Add(this.summaryPanel);
            this.Controls.Add(this.filterPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "GradingControl";
            this.Size = new System.Drawing.Size(1000, 700);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.filterPanel.ResumeLayout(false);
            this.summaryPanel.ResumeLayout(false);
            this.summaryPanel.PerformLayout();
            this.tablePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).EndInit();
            this.ResumeLayout(false);

        }

        // ============================================
        // DATABASE METHODS
        // ============================================

        void LoadCourseFilter()
        {
            try
            {
                cboCourse.Items.Clear();
                cboCourse.Items.Add(new CourseItem { CourseID = "", CourseName = "-- Tất cả khóa học --" });

                DataTable dt = studentService.GetTeacherCourses(currentTeacherID);

                foreach (DataRow row in dt.Rows)
                {
                    cboCourse.Items.Add(new CourseItem
                    {
                        CourseID = row["CourseID"].ToString(),
                        CourseName = row["CourseName"].ToString()
                    });
                }

                cboCourse.DisplayMember = "CourseName";
                cboCourse.ValueMember = "CourseID";
                cboCourse.SelectedIndex = 0;

                // Status filter
                cboStatus.Items.Clear();
                cboStatus.Items.Add("Tất cả");
                cboStatus.Items.Add("Chưa chấm");
                cboStatus.Items.Add("Đã chấm");
                cboStatus.Items.Add("Nộp trễ");
                cboStatus.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải bộ lọc: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void LoadSubmissions()
        {
            try
            {
                string courseFilter = null;

                if (cboCourse.SelectedItem is CourseItem item && !string.IsNullOrEmpty(item.CourseID))
                {
                    // GradingService filter theo CourseName
                    courseFilter = item.CourseName;
                }

                string selectedStatus = cboStatus.SelectedItem?.ToString();
                string statusFilter = selectedStatus == "Tất cả" ? null : selectedStatus;

                DataTable dt = gradingService.GetAllSubmissions(
                    currentTeacherID,
                    courseFilter,
                    statusFilter
                );

                dgvSubmissions.Rows.Clear();

                int pendingCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string status = row["Status"].ToString();
                    if (status == "Chưa chấm") pendingCount++;

                    dgvSubmissions.Rows.Add(
                        row["StudentName"],
                        row["AssignmentTitle"],
                        row["CourseName"],
                        Convert.ToDateTime(row["SubmissionDate"]).ToString("dd/MM/yyyy"),
                        Convert.ToDateTime(row["DueDate"]).ToString("dd/MM/yyyy"),
                        status,
                        row["Score"] != DBNull.Value ? row["Score"].ToString() : "-",
                        "", // Thao tác
                        row["SubmissionID"]
                    );
                }

                lblSummaryCount.Text = pendingCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài nộp: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn khóa học chưa
                if (!(cboCourse.SelectedItem is CourseItem selectedCourse) || string.IsNullOrEmpty(selectedCourse.CourseID))
                {
                    MessageBox.Show("Vui lòng chọn một khóa học cụ thể để xuất Excel!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCourse.Focus();
                    return;
                }

                string courseName = selectedCourse.CourseName;

                // Lấy toàn bộ bài nộp của khóa học (không lọc theo trạng thái)
                DataTable dt = gradingService.GetAllSubmissions(currentTeacherID, courseName, null);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu bài nộp trong khóa học này!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.Title = "Xuất danh sách điểm";
                saveDialog.FileName = $"DiemLop_{courseName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Danh sách điểm");

                        // TIÊU ĐỀ: LỚP + TÊN KHÓA HỌC
                        worksheet.Cell(1, 1).Value = $"LỚP: {courseName.ToUpper()}";
                        worksheet.Range("A1:F1").Merge();
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Cell(1, 1).Style.Font.FontSize = 18;
                        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1F4E79");

                        // Ngày xuất
                        worksheet.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        worksheet.Range("A2:F2").Merge();
                        worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(2, 1).Style.Font.Italic = true;

                        // HEADER: Mã SV, Họ và tên, Email, Ngày nộp, Trạng thái, Điểm
                        int headerRow = 4;
                        string[] headers = { "Mã SV", "Họ và tên", "Email", "Ngày nộp", "Trạng thái", "Điểm" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            var cell = worksheet.Cell(headerRow, i + 1);
                            cell.Value = headers[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                            cell.Style.Font.FontColor = XLColor.White;
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        // DỮ LIỆU
                        int dataRow = headerRow + 1;
                        int gradedCount = 0, pendingCount = 0, lateCount = 0;

                        foreach (DataRow row in dt.Rows)
                        {
                            string status = row["Status"].ToString();

                            // Đếm thống kê
                            if (status == "Đã chấm") gradedCount++;
                            else if (status == "Chưa chấm") pendingCount++;
                            else if (status == "Nộp trễ") lateCount++;

                            // Mã SV (SubmissionID tạm thời, hoặc bạn có thể thêm cột StudentID vào query)
                            worksheet.Cell(dataRow, 1).Value = row["SubmissionID"].ToString();
                            worksheet.Cell(dataRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Họ và tên
                            worksheet.Cell(dataRow, 2).Value = row["StudentName"].ToString();
                            worksheet.Cell(dataRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Email (cần thêm vào query nếu muốn, tạm thời để trống)
                            worksheet.Cell(dataRow, 3).Value = row["Email"].ToString();
                            worksheet.Cell(dataRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            // Ngày nộp
                            string submitDate = "";
                            if (row["SubmissionDate"] != DBNull.Value)
                            {
                                submitDate = Convert.ToDateTime(row["SubmissionDate"]).ToString("dd/MM/yyyy");
                            }
                            worksheet.Cell(dataRow, 4).Value = submitDate;
                            worksheet.Cell(dataRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(dataRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Trạng thái (Đã chấm, Chưa chấm, Nộp trễ)
                            var cellStatus = worksheet.Cell(dataRow, 5);
                            cellStatus.Value = status;
                            cellStatus.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cellStatus.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            // Tô màu trạng thái
                            switch (status)
                            {
                                case "Chưa chấm":
                                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#FF9800"); // Cam
                                    break;
                                case "Đã chấm":
                                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#4CAF50"); // Xanh lá
                                    break;
                                case "Nộp trễ":
                                    cellStatus.Style.Font.FontColor = XLColor.FromHtml("#F44336"); // Đỏ
                                    break;
                            }

                            // Điểm - để trống nếu chưa chấm
                            var cellScore = worksheet.Cell(dataRow, 6);
                            if (status == "Đã chấm" && row["Score"] != DBNull.Value)
                            {
                                cellScore.Value = row["Score"].ToString();
                                cellScore.Style.Font.Bold = true;
                            }
                            else
                            {
                                cellScore.Value = ""; // Để trống nếu chưa chấm
                            }
                            cellScore.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            cellScore.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            dataRow++;
                        }

                        // THỐNG KÊ
                        int statsRow = dataRow + 2;
                        worksheet.Cell(statsRow, 1).Value = $"Tổng số bài nộp: {dt.Rows.Count}";
                        worksheet.Cell(statsRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(statsRow + 1, 1).Value = $"Đã chấm: {gradedCount} | Chưa chấm: {pendingCount} | Nộp trễ: {lateCount}";
                        worksheet.Cell(statsRow + 1, 1).Style.Font.Italic = true;

                        // Auto-fit
                        worksheet.Columns().AdjustToContents();
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 25;
                        worksheet.Column(4).Width = 12;
                        worksheet.Column(5).Width = 12;
                        worksheet.Column(6).Width = 10;

                        workbook.SaveAs(saveDialog.FileName);
                    }

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (MessageBox.Show("Bạn có muốn mở file?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ============================================
        // EVENT HANDLERS
        // ============================================

        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubmissions();
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubmissions();
        }

        private void dgvSubmissions_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Paint Status column with badges
            if (e.ColumnIndex == dgvSubmissions.Columns["colStatus"].Index)
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string status = dgvSubmissions.Rows[e.RowIndex].Cells["colStatus"].Value?.ToString();
                if (!string.IsNullOrEmpty(status))
                {
                    Color badgeColor;
                    switch (status)
                    {
                        case "Chưa chấm":
                            badgeColor = Color.FromArgb(255, 152, 0);
                            break;
                        case "Đã chấm":
                            badgeColor = Color.FromArgb(76, 175, 80);
                            break;
                        case "Nộp trễ":
                            badgeColor = Color.FromArgb(244, 67, 54);
                            break;
                        default:
                            badgeColor = Color.Gray;
                            break;
                    }

                    Rectangle badgeRect = new Rectangle(
                        e.CellBounds.X + 10,
                        e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                        e.CellBounds.Width - 20,
                        24
                    );

                    using (GraphicsPath path = GetRoundedRectanglePath(badgeRect, 12))
                    {
                        using (SolidBrush brush = new SolidBrush(badgeColor))
                        {
                            e.Graphics.FillPath(brush, path);
                        }
                    }

                    using (Font font = new Font("Segoe UI", 8F, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        e.Graphics.DrawString(status, font, textBrush, badgeRect, sf);
                    }
                }
            }

            // Paint Action column with buttons
            if (e.ColumnIndex == dgvSubmissions.Columns["colAction"].Index)
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string status = dgvSubmissions.Rows[e.RowIndex].Cells["colStatus"].Value?.ToString();
                string buttonText = status == "Đã chấm" ? "Xem" : "Chấm điểm";
                Color buttonColor = status == "Đã chấm"
                    ? Color.FromArgb(33, 150, 243)
                    : Color.FromArgb(76, 175, 80);

                Rectangle buttonRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + (e.CellBounds.Height - 32) / 2,
                    e.CellBounds.Width - 20,
                    32
                );

                bool isHovered = e.RowIndex == hoverRow;
                Color finalColor = isHovered ? ControlPaint.Dark(buttonColor, 0.1f) : buttonColor;

                using (GraphicsPath path = GetRoundedRectanglePath(buttonRect, 4))
                {
                    using (SolidBrush brush = new SolidBrush(finalColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(buttonText, font, textBrush, buttonRect, sf);
                }
            }
        }

        private void dgvSubmissions_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvSubmissions.Columns["colAction"].Index)
            {
                if (hoverRow != e.RowIndex)
                {
                    hoverRow = e.RowIndex;
                    dgvSubmissions.InvalidateRow(e.RowIndex);
                }
                dgvSubmissions.Cursor = Cursors.Hand;
            }
            else
            {
                if (hoverRow != -1)
                {
                    int oldHoverRow = hoverRow;
                    hoverRow = -1;
                    dgvSubmissions.InvalidateRow(oldHoverRow);
                }
                dgvSubmissions.Cursor = Cursors.Default;
            }
        }

        private void dgvSubmissions_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (hoverRow != -1)
            {
                int oldHoverRow = hoverRow;
                hoverRow = -1;
                dgvSubmissions.InvalidateRow(oldHoverRow);
            }
            dgvSubmissions.Cursor = Cursors.Default;
        }

        private void dgvSubmissions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvSubmissions.Columns["colAction"].Index)
            {
                string submissionID = dgvSubmissions.Rows[e.RowIndex].Cells["colSubmissionID"].Value.ToString();
                string status = dgvSubmissions.Rows[e.RowIndex].Cells["colStatus"].Value.ToString();

                OpenGradingForm(submissionID, status == "Đã chấm");
            }
        }

        // ============================================
        // HELPER METHODS
        // ============================================

        private void OpenGradingForm(string submissionID, bool isViewOnly)
        {
            // Tạo form chấm điểm (sẽ tạo ở phần tiếp theo)
            using (var form = new Forms.GradeSubmissionForm(submissionID, isViewOnly))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadSubmissions(); // Reload danh sách sau khi chấm
                }
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ============================================
        // NESTED CLASSES
        // ============================================

        private class CourseItem
        {
            public string CourseID { get; set; }
            public string CourseName { get; set; }
        }
    }
}
