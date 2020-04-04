using System;
using System.Collections.Generic;
using System.IO;
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

namespace pt_lab_01_wpf_app.Scripts
{
    /// <summary>
    /// Interaction logic for CreateFileDialog.xaml
    /// </summary>
    public partial class CreateFileDialog : Window
    {
        private TreeViewItem fileParent;
        private string filePath;
        private System.Action<TreeViewItem, string> successCallback;

        public CreateFileDialog(TreeViewItem parent, string path, Action<TreeViewItem, string> successCallback = null)
        {
            fileParent = parent;
            filePath = path;
            this.successCallback = successCallback;
            InitializeComponent();
        }

        private void OnButtonOkClick(object sender, RoutedEventArgs e)
        {
            if ((bool)rbDir.IsChecked)
            {
                try
                {
                    var tempPath = filePath + "\\" + txtName.Text;
                    CheckPathAvailability(ref tempPath, isDirectory: true);

                    var dirInfo = Directory.CreateDirectory(tempPath);
                    System.Diagnostics.Debug.WriteLine("Trying to create directory: " + tempPath);
                    dirInfo.Attributes = GetDialogAttributes();
                    successCallback?.Invoke(fileParent, tempPath);
                }
                catch { HandleCreationFailure(); }
            }
            else if ((bool)rbFile.IsChecked)
            {
                try
                {
                    var tempPath = filePath + "\\" + txtName.Text;
                    CheckPathAvailability(ref tempPath, isDirectory: false);

                    using (var tempFile = File.Create(tempPath)){}
                    File.SetAttributes(tempPath, GetDialogAttributes());
                    System.Diagnostics.Debug.WriteLine("Trying to create file: " + tempPath);
                    successCallback?.Invoke(fileParent, tempPath);
                }
                catch { HandleCreationFailure(); }
            }
            else
                HandleCreationFailure();
            this.DialogResult = true;
        }

        private void CheckPathAvailability(ref string tempPath, bool isDirectory)
        {
            int counter = 1;
            string pathToCheck = tempPath;
            var tempSplit = pathToCheck.Split('.');
            while (Directory.Exists(pathToCheck) || File.Exists(pathToCheck))
            {
                if (tempSplit.Length > 1)
                {
                    for (int i = 0; i < tempSplit.Length - 3; i++)
                        pathToCheck += tempSplit[i];
                    pathToCheck += tempSplit[tempSplit.Length - 2] + "(" + counter.ToString() + ")";
                    pathToCheck += tempSplit[tempSplit.Length - 1];
                }
                else
                    pathToCheck = tempPath + "(" + counter.ToString() + ")";
                counter++;
            }
            tempPath = pathToCheck;
        }

        private void HandleCreationFailure()
        {
            MessageBox.Show("Couldn't create file", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            txtName.SelectAll();
            txtName.Focus();
        }

        private FileAttributes GetDialogAttributes()
        {
            FileAttributes tempAttr = 0;
            if ((bool)cbReadOnly.IsChecked) tempAttr |= FileAttributes.ReadOnly;
            if ((bool)cbArchive.IsChecked) tempAttr |= FileAttributes.Archive;
            if ((bool)cbHidden.IsChecked) tempAttr |= FileAttributes.Hidden;
            if ((bool)cbSystem.IsChecked) tempAttr |= FileAttributes.System;
            if ((bool)rbDir.IsChecked)
            {
                tempAttr = tempAttr | FileAttributes.Directory;
            }
            else if (tempAttr == 0) tempAttr = FileAttributes.Normal;

            System.Diagnostics.Debug.WriteLine("GetDialogAttributes: " + tempAttr.ToString());
            return tempAttr;
        }
    }
}
