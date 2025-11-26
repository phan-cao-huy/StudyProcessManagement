namespace StudyProcessManagement.Views.Teacher
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.flowLayoutPanelMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnCourses = new System.Windows.Forms.Button();
            this.btnContent = new System.Windows.Forms.Button();
            this.btnAssignments = new System.Windows.Forms.Button();
            this.btnGrading = new System.Windows.Forms.Button();
            this.btnStudents = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblSidebarHeader = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelSidebar.SuspendLayout();
            this.flowLayoutPanelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.panelSidebar.Controls.Add(this.flowLayoutPanelMenu);
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Controls.Add(this.lblSidebarHeader);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(272, 626);
            this.panelSidebar.TabIndex = 0;
            // 
            // flowLayoutPanelMenu
            // 
            this.flowLayoutPanelMenu.Controls.Add(this.btnDashboard);
            this.flowLayoutPanelMenu.Controls.Add(this.btnCourses);
            this.flowLayoutPanelMenu.Controls.Add(this.btnContent);
            this.flowLayoutPanelMenu.Controls.Add(this.btnAssignments);
            this.flowLayoutPanelMenu.Controls.Add(this.btnGrading);
            this.flowLayoutPanelMenu.Controls.Add(this.btnStudents);
            this.flowLayoutPanelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMenu.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelMenu.Location = new System.Drawing.Point(0, 60);
            this.flowLayoutPanelMenu.Name = "flowLayoutPanelMenu";
            this.flowLayoutPanelMenu.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanelMenu.Size = new System.Drawing.Size(272, 516);
            this.flowLayoutPanelMenu.TabIndex = 1;
            // 
            // btnDashboard
            // 
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnDashboard.Location = new System.Drawing.Point(13, 13);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(3);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnDashboard.Size = new System.Drawing.Size(240, 40);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnCourses
            // 
            this.btnCourses.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCourses.FlatAppearance.BorderSize = 0;
            this.btnCourses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCourses.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCourses.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnCourses.Location = new System.Drawing.Point(13, 59);
            this.btnCourses.Margin = new System.Windows.Forms.Padding(3);
            this.btnCourses.Name = "btnCourses";
            this.btnCourses.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCourses.Size = new System.Drawing.Size(240, 40);
            this.btnCourses.TabIndex = 1;
            this.btnCourses.Text = "📚 Quản lý khóa học";
            this.btnCourses.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCourses.UseVisualStyleBackColor = true;
            this.btnCourses.Click += new System.EventHandler(this.btnCourses_Click);
            // 
            // btnContent
            // 
            this.btnContent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContent.FlatAppearance.BorderSize = 0;
            this.btnContent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnContent.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnContent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnContent.Location = new System.Drawing.Point(13, 105);
            this.btnContent.Margin = new System.Windows.Forms.Padding(3);
            this.btnContent.Name = "btnContent";
            this.btnContent.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnContent.Size = new System.Drawing.Size(240, 40);
            this.btnContent.TabIndex = 2;
            this.btnContent.Text = "📂 Quản lý nội dung";
            this.btnContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContent.UseVisualStyleBackColor = true;
            this.btnContent.Click += new System.EventHandler(this.btnContent_Click);
            // 
            // btnAssignments
            // 
            this.btnAssignments.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAssignments.FlatAppearance.BorderSize = 0;
            this.btnAssignments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAssignments.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnAssignments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnAssignments.Location = new System.Drawing.Point(13, 151);
            this.btnAssignments.Margin = new System.Windows.Forms.Padding(3);
            this.btnAssignments.Name = "btnAssignments";
            this.btnAssignments.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnAssignments.Size = new System.Drawing.Size(240, 40);
            this.btnAssignments.TabIndex = 3;
            this.btnAssignments.Text = "📝 Quản lý bài tập";
            this.btnAssignments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAssignments.UseVisualStyleBackColor = true;
            this.btnAssignments.Click += new System.EventHandler(this.btnAssignments_Click);
            // 
            // btnGrading
            // 
            this.btnGrading.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrading.FlatAppearance.BorderSize = 0;
            this.btnGrading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGrading.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGrading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnGrading.Location = new System.Drawing.Point(13, 197);
            this.btnGrading.Margin = new System.Windows.Forms.Padding(3);
            this.btnGrading.Name = "btnGrading";
            this.btnGrading.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnGrading.Size = new System.Drawing.Size(240, 40);
            this.btnGrading.TabIndex = 4;
            this.btnGrading.Text = "✅ Chấm điểm & Phản hồi";
            this.btnGrading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrading.UseVisualStyleBackColor = true;
            this.btnGrading.Click += new System.EventHandler(this.btnGrading_Click);
            // 
            // btnStudents
            // 
            this.btnStudents.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStudents.FlatAppearance.BorderSize = 0;
            this.btnStudents.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStudents.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnStudents.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.btnStudents.Location = new System.Drawing.Point(13, 243);
            this.btnStudents.Margin = new System.Windows.Forms.Padding(3);
            this.btnStudents.Name = "btnStudents";
            this.btnStudents.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnStudents.Size = new System.Drawing.Size(240, 40);
            this.btnStudents.TabIndex = 5;
            this.btnStudents.Text = "🎓 Quản lý học viên";
            this.btnStudents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStudents.UseVisualStyleBackColor = true;
            this.btnStudents.Click += new System.EventHandler(this.btnStudents_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(0, 576);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(272, 50);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "🚪 Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            this.btnLogout.MouseEnter += new System.EventHandler(this.btnLogout_MouseEnter);
            this.btnLogout.MouseLeave += new System.EventHandler(this.btnLogout_MouseLeave);
            // 
            // lblSidebarHeader
            // 
            this.lblSidebarHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSidebarHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSidebarHeader.ForeColor = System.Drawing.Color.White;
            this.lblSidebarHeader.Location = new System.Drawing.Point(0, 0);
            this.lblSidebarHeader.Name = "lblSidebarHeader";
            this.lblSidebarHeader.Padding = new System.Windows.Forms.Padding(10);
            this.lblSidebarHeader.Size = new System.Drawing.Size(272, 60);
            this.lblSidebarHeader.TabIndex = 3;
            this.lblSidebarHeader.Text = "📚 LMS Teacher";
            this.lblSidebarHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(272, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1034, 626);
            this.panelContent.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1306, 626);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelSidebar);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản Lý Học Tập - LMS";
            this.panelSidebar.ResumeLayout(false);
            this.flowLayoutPanelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMenu;
        private System.Windows.Forms.Label lblSidebarHeader;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnCourses;
        private System.Windows.Forms.Button btnContent;
        private System.Windows.Forms.Button btnAssignments;
        private System.Windows.Forms.Button btnGrading;
        private System.Windows.Forms.Button btnStudents;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelContent;
    }
}
