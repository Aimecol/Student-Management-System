using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SQLite;
using System.Drawing.Drawing2D;

namespace StudentManagementSystem
{
    public partial class MainForm : Form
    {
        private SQLiteConnection connection; 
        private DataGridView studentDataGridView;
        private MaterialButton btnAdd, btnEdit, btnDelete;
        private MaterialTextBox txtName, txtAge, txtGrade;
        private Panel sidePanel, headerPanel, mainPanel;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadStudents();
        }

        private void InitializeComponent()
        {
            // Form Setup
            this.Icon = new Icon("group.ico");
            this.Text = "Student Management System";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(33, 150, 243) 
            };

            var lblTitle = new Label
            {
                Text = "Student Management System",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // Main Panel
            mainPanel = new Panel
            {
                Dock = DockStyle.None,
                Location = new Point(250, 50),
                Size = new Size(this.ClientSize.Width - 250, this.ClientSize.Height - 50),
                BackColor = Color.WhiteSmoke
            };

            // Side Panel for Inputs and Buttons
            sidePanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Input Fields
            txtName = new MaterialTextBox { PlaceholderText = "Student Name", Location = new Point(20, 40), Width = 200 };
            txtAge = new MaterialTextBox { PlaceholderText = "Age", Location = new Point(20, 100), Width = 200 };
            txtGrade = new MaterialTextBox { PlaceholderText = "Grade", Location = new Point(20, 160), Width = 200 };

            // Buttons
            btnAdd = new MaterialButton { Text = "Add", Location = new Point(20, 240), Width = 200, BackColor = Color.FromArgb(76, 175, 80) };
            btnEdit = new MaterialButton { Text = "Edit", Location = new Point(20, 300), Width = 200, BackColor = Color.FromArgb(33, 150, 243) };
            btnDelete = new MaterialButton { Text = "Delete", Location = new Point(20, 360), Width = 200, BackColor = Color.FromArgb(244, 67, 54) };

            sidePanel.Controls.Add(txtName);
            sidePanel.Controls.Add(txtAge);
            sidePanel.Controls.Add(txtGrade);
            sidePanel.Controls.Add(btnAdd);
            sidePanel.Controls.Add(btnEdit);
            sidePanel.Controls.Add(btnDelete);

            this.Controls.Add(sidePanel);

            // DataGridView for displaying students
            studentDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Location = new Point(250, 100),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 245)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(33, 150, 243),
                    SelectionForeColor = Color.White,
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                },
                GridColor = Color.FromArgb(224, 224, 224)
            };

            mainPanel.Controls.Add(studentDataGridView);
            this.Controls.Add(mainPanel);

            // Event Handlers
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        // Material-like Button
        public class MaterialButton : Button
        {
            public MaterialButton()
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Font = new Font("Segoe UI", 10, FontStyle.Bold);
                ForeColor = Color.White;
                Cursor = Cursors.Hand;
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                this.BackColor = ControlPaint.Light(this.BackColor, 0.2f);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                this.BackColor = ControlPaint.Dark(this.BackColor, 0.1f);
            }
        }

        // Material-like TextBox
        public class MaterialTextBox : TextBox
        {
            private string placeholderText = "";
            private Color placeholderColor = Color.Gray;

            public MaterialTextBox()
            {
                BorderStyle = BorderStyle.None;
                Font = new Font("Segoe UI", 10);
                BackColor = Color.White;
                ForeColor = Color.Black;
                
                // Add custom bottom border
                Paint += (s, e) => {
                    using (Pen pen = new Pen(Color.Gray, 1))
                    {
                        e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
                    }
                };
            }

            private void UpdatePlaceholder()
            {
                if (string.IsNullOrEmpty(Text))
                {
                    Text = placeholderText;
                    ForeColor = placeholderColor;
                }
            }

            private void RemovePlaceholder()
            {
                if (Text == placeholderText && ForeColor == placeholderColor)
                {
                    Text = string.Empty;
                    ForeColor = Color.Black;
                }
            }
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=students.db";
            connection = new SQLiteConnection(connectionString);
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Students (
                    StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Age INTEGER,
                    Grade TEXT
                )";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void LoadStudents()
        {
            string query = "SELECT * FROM Students";
            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);
                    studentDataGridView.DataSource = dataTable;

                    studentDataGridView.Columns["StudentID"].Visible = false;
                    studentDataGridView.Columns["Name"].HeaderText = "Student Name";
                    studentDataGridView.Columns["Age"].HeaderText = "Age";
                    studentDataGridView.Columns["Grade"].HeaderText = "Grade";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Comprehensive input validation
            if (!ValidateStudentInput(txtName.Text, txtAge.Text, out int age))
            {
                return;
            }

            try
            {
                string insertQuery = @"INSERT INTO Students (Name, Age, Grade) VALUES (@Name, @Age, @Grade)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Grade", txtGrade.Text.Trim());
                    command.ExecuteNonQuery();
                }

                LoadStudents();
                ClearInputs();
                MessageBox.Show("Student added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Error adding student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Validate row selection
            if (studentDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a student to edit.", 
                    "Selection Required", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information
                );
                return;
            }

            // Extract selected student data
            DataGridViewRow selectedRow = studentDataGridView.SelectedRows[0];
            int studentId = Convert.ToInt32(selectedRow.Cells["StudentID"].Value);

            // Create edit dialog
            using (var editForm = new Form())
            {
                editForm.Text = "Edit Student";
                editForm.Size = new Size(300, 250);
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.MaximizeBox = false;

                // Name input
                var lblName = new Label { Text = "Name:", Location = new Point(20, 20), Width = 100 };
                var txtEditName = new TextBox 
                { 
                    Text = selectedRow.Cells["Name"].Value.ToString(), 
                    Location = new Point(130, 20), 
                    Width = 150 
                };

                // Age input
                var lblAge = new Label { Text = "Age:", Location = new Point(20, 60), Width = 100 };
                var txtEditAge = new TextBox 
                { 
                    Text = selectedRow.Cells["Age"].Value.ToString(), 
                    Location = new Point(130, 60), 
                    Width = 150 
                };

                // Grade input
                var lblGrade = new Label { Text = "Grade:", Location = new Point(20, 100), Width = 100 };
                var txtEditGrade = new TextBox 
                { 
                    Text = selectedRow.Cells["Grade"].Value.ToString(), 
                    Location = new Point(130, 100), 
                    Width = 150 
                };

                // Save button
                var btnSave = new Button 
                { 
                    Text = "Save",
                    Location = new Point(100, 150),
                    DialogResult = DialogResult.OK,
                    Width = 100,
                    BackColor = Color.FromArgb(76, 175, 80),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White 
                };

                // Cancel button
                var btnCancel = new Button 
                { 
                    Text = "Cancel", 
                    Location = new Point(180, 150), 
                    DialogResult = DialogResult.Cancel 
                };

                editForm.Controls.AddRange(new Control[] 
                { 
                    lblName, txtEditName, 
                    lblAge, txtEditAge, 
                    lblGrade, txtEditGrade, 
                    btnSave, btnCancel 
                });

                // Validate inputs before closing
                btnSave.Click += (s, ev) => 
                {
                    if (string.IsNullOrWhiteSpace(txtEditName.Text))
                    {
                        MessageBox.Show("Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        editForm.DialogResult = DialogResult.None;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtEditGrade.Text))
                    {
                        MessageBox.Show("Grade cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        editForm.DialogResult = DialogResult.None;
                        return;
                    }

                    if (!int.TryParse(txtEditAge.Text, out int age) || age < 0 || age > 120)
                    {
                        MessageBox.Show("Please enter a valid age.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        editForm.DialogResult = DialogResult.None;
                        return;
                    }
                };

                // Show dialog and process result
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    string updateQuery = @"
                        UPDATE Students 
                        SET Name = @Name, Age = @Age, Grade = @Grade 
                        WHERE StudentID = @StudentID";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", txtEditName.Text);
                        command.Parameters.AddWithValue("@Age", txtEditAge.Text);
                        command.Parameters.AddWithValue("@Grade", txtEditGrade.Text);
                        command.Parameters.AddWithValue("@StudentID", studentId);

                        command.ExecuteNonQuery();
                    }

                    LoadStudents();
                }
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (studentDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected student details
            int studentId = Convert.ToInt32(studentDataGridView.SelectedRows[0].Cells["StudentID"].Value);
            string studentName = studentDataGridView.SelectedRows[0].Cells["Name"].Value.ToString();

            // Confirm deletion with detailed dialog
            var result = MessageBox.Show(
                $"Are you absolutely sure you want to delete the student record for '{studentName}'?\n\nThis action cannot be undone.", 
                "Confirm Permanent Deletion", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning, 
                MessageBoxDefaultButton.Button2
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string deleteQuery = "DELETE FROM Students WHERE StudentID = @StudentID";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@StudentID", studentId);
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            LoadStudents();
                            ClearInputs();
                            MessageBox.Show($"Student '{studentName}' deleted successfully.", "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No student was deleted. The record may have already been removed.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Centralized input validation method
        private bool ValidateStudentInput(string name, string ageText, out int age)
        {
            age = 0;

            // Check name
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Student name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Check age
            if (!int.TryParse(ageText, out age) || age <= 0)
            {
                MessageBox.Show("Age must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtAge.Clear();
            txtGrade.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            connection?.Close();
            base.OnFormClosing(e);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
