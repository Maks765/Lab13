using System;
using System.IO;
using System.Windows.Forms;

public class FileExplorerForm : Form
{
    private TreeView driveTreeView;
    private ListView fileListView;
    private TextBox propertyTextBox;

    public FileExplorerForm()
    {
        this.Text = "File Explorer";
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Створюємо елементи форми
        this.driveTreeView = new TreeView();
        this.fileListView = new ListView();
        this.propertyTextBox = new TextBox();

        // Налаштовуємо елементи форми
        this.driveTreeView.Location = new System.Drawing.Point(20, 20);
        this.driveTreeView.Size = new System.Drawing.Size(200, 500);
        this.driveTreeView.AfterSelect += DriveTreeView_AfterSelect;

        this.fileListView.Location = new System.Drawing.Point(240, 20);
        this.fileListView.Size = new System.Drawing.Size(400, 500);
        this.fileListView.View = View.Details;
        this.fileListView.Columns.Add("Ім'я");
        this.fileListView.Columns.Add("Розмір");
        this.fileListView.Columns.Add("Дата");
        this.fileListView.DoubleClick += FileListView_DoubleClick;

        this.propertyTextBox.Location = new System.Drawing.Point(20, 530);
        this.propertyTextBox.Size = new System.Drawing.Size(780, 100);
        this.propertyTextBox.Multiline = true;
        this.propertyTextBox.ScrollBars = ScrollBars.Vertical;

        // Додаємо елементи форми до контейнера
        this.Controls.Add(this.driveTreeView);
        this.Controls.Add(this.fileListView);
        this.Controls.Add(this.propertyTextBox);

        // Завантажуємо список дисків при старті програми
        LoadDrives();
    }

    private void LoadDrives()
    {
        this.driveTreeView.Nodes.Clear();
        foreach (var drive in DriveInfo.GetDrives())
        {
            var driveNode = this.driveTreeView.Nodes.Add(drive.Name);
            driveNode.Tag = drive;
        }
    }

    private void DriveTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        var selectedDrive = e.Node.Tag as DriveInfo;
        if (selectedDrive != null)
        {
            DisplayDirectoryContents(selectedDrive.RootDirectory.FullName);
            DisplayDriveProperties(selectedDrive);
        }
    }

    private void FileListView_DoubleClick(object sender, EventArgs e)
    {
        var selectedItem = this.fileListView.SelectedItems[0];
        if (selectedItem != null)
        {
            var path = Path.Combine(this.driveTreeView.SelectedNode.FullPath, selectedItem.Text);
            if (Directory.Exists(path))
            {
                DisplayDirectoryContents(path);
                DisplayDirectoryProperties(path);
            }
            else if (File.Exists(path))
            {
                DisplayFileProperties(path);
            }
        }
    }

    private void DisplayDirectoryContents(string path)
    {
        this.fileListView.Items.Clear();
        foreach (var file in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(file);
            this.fileListView.Items.Add(new ListViewItem(new string[] { fileInfo.Name, fileInfo.Length.ToString(), fileInfo.LastWriteTime.ToString() }));
        }
        foreach (var dir in Directory.GetDirectories(path))
        {
            var dirInfo = new DirectoryInfo(dir);
            this.fileListView.Items.Add(new ListViewItem(new string[] { dirInfo.Name, "<DIR>", dirInfo.LastWriteTime.ToString() }));
        }
    }

    private void DisplayDriveProperties(DriveInfo drive)
    {
        this.propertyTextBox.Text = $"Диск: {drive.Name}\nТип: {drive.DriveType}\nФайлова система: {drive.DriveFormat}\nЗагальний обсяг: {drive.TotalSize / (1024.0 * 1024.0 * 1024.0):F2} ГБ\nВільний обсяг: {drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0):F2} ГБ";
    }

    private void DisplayDirectoryProperties(string path)
    {
        var dirInfo = new DirectoryInfo(path);
        this.propertyTextBox.Text = $"Каталог: {dirInfo.FullName}\nАтрибути: {dirInfo.Attributes}\nДата створення: {dirInfo.CreationTime}\nДата останньої зміни: {dirInfo.LastWriteTime}";
    }

    private void DisplayFileProperties(string path)
    {
        var fileInfo = new FileInfo(path);
        this.propertyTextBox.Text = $"Файл: {fileInfo.FullName}\nРозмір: {fileInfo.Length} байт\nАтрибути: {fileInfo.Attributes}\nДата створення: {fileInfo.CreationTime}\nДата останньої зміни: {fileInfo.LastWriteTime}";
    }
}

public static class Program
{
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (var form = new FileExplorerForm())
        {
            form.ShowDialog();
        }
    }
}