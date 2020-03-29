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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pt_lab_01_wpf_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedPath;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContent();
        }

        private void InitializeContent()
        {
            //System.Diagnostics.Debug.WriteLine("Initializing content.");
            selectedPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            SetTreeRoot(selectedPath);
        }


        #region Interactions
        public void OnMenuButtonOpenClick(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.SelectedPath = selectedPath;
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //System.Diagnostics.Debug.WriteLine("FolderBrowserDialog DialogResult.OK.");
                treeView.Items.Clear();
                selectedPath = dlg.SelectedPath;    //saves last path for next dialog open
                SetTreeRoot(dlg.SelectedPath);
            }
            else if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                //System.Diagnostics.Debug.WriteLine("FolderBrowserDialog DialogResult.Cancel.");
                return;
            }
        }

        public void OnMenuButtonExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion Interactions

        #region Inner Mechanics
        private void SetTreeRoot(string path)
        {
            if (!Directory.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine("UpdateTreeViewRoot - given path is not a directory.");
                return;
            }

            var root = GetTreeItem(path);
            treeView.Items.Add(root);

            string[] fileEntries = Directory.GetFileSystemEntries(path);
            foreach (string fileEntryPath in fileEntries)
            {
                AddTreeItem(root, fileEntryPath);
            }

            foreach (var item in root.Items)
                if (Directory.Exists((item as TreeViewItem).Tag.ToString()))
                {
                    (item as TreeViewItem).Items.Add(new TreeViewItem());
                    (item as TreeViewItem).IsExpanded = false;
                }

            root.IsSelected = true;
            root.IsExpanded = true;
        }

        private void AddTreeItem(TreeViewItem parent, string path)
        {
            if (File.Exists(path))
            {
                parent.Items.Add(GetTreeItem(path));
            }
            else if (Directory.Exists(path))
            {
                var temp = GetTreeItem(path, isDirectory: true);
                parent.Items.Add(temp);
                temp.IsExpanded = false;
                temp.Items.Add(new TreeViewItem());
                temp.Expanded += OnTreeViewItemExpanded;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("{0} is not a valid file or directory.", path);
            }
        }

        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem extendedParent = e.Source as TreeViewItem;

            extendedParent.Items.Clear();

            string[] fileEntries = Directory.GetFileSystemEntries(extendedParent.Tag.ToString());
            foreach (string fileEntryPath in fileEntries)
            {
                AddTreeItem(extendedParent, fileEntryPath);
            }
        }

        private TreeViewItem GetTreeItem(string path, bool isDirectory = false)
        {
            return new TreeViewItem
            {
                Header = isDirectory ? "[D] " + path.Split('\\').Last((e) => e != "") : path.Split('\\').Last((e) => e != ""),
                Tag = path
            };
        }
        #endregion Inner Mechanics
    }
}
