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
        private System.Windows.Controls.ContextMenu textFileContextMenu;
        private System.Windows.Controls.ContextMenu directoryContextMenu;
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeContent();
        }

        private void InitializeContent()
        {
            //System.Diagnostics.Debug.WriteLine("Initializing content.");

            var c1 = new System.Windows.Controls.MenuItem();
            c1.Header = "Open";
            c1.Click += (object sender, RoutedEventArgs e) => OnOpenFile(treeView.SelectedItem as TreeViewItem);
            var c2 = new System.Windows.Controls.MenuItem();
            c2.Header = "Create";
            c2.Click += (object sender, RoutedEventArgs e) => OnCreateInFile(treeView.SelectedItem as TreeViewItem);

            var c3 = new System.Windows.Controls.MenuItem();
            c3.Header = "Delete";
            c3.Click += (object sender, RoutedEventArgs e) => OnDeleteFile(treeView.SelectedItem as TreeViewItem);
            var c4 = new System.Windows.Controls.MenuItem();
            c4.Header = "Delete";
            c4.Click += (object sender, RoutedEventArgs e) => OnDeleteFile(treeView.SelectedItem as TreeViewItem);

            textFileContextMenu = new System.Windows.Controls.ContextMenu();
            textFileContextMenu.Items.Add(c1);
            textFileContextMenu.Items.Add(c3);

            directoryContextMenu = new System.Windows.Controls.ContextMenu();
            directoryContextMenu.Items.Add(c2);
            directoryContextMenu.Items.Add(c4);

            selectedPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            SetTreeRoot(selectedPath);
        }

        #region Interactions

        //external
        // Respond to the right mouse button down event by setting up a hit test results callback.
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //// Retrieve the coordinate of the mouse position.
            //Point pt = e.GetPosition((UIElement)sender);

            //// Clear the contents of the list used for hit test results.
            //hitResultsList.Clear();

            //// Set up a callback to receive the hit test result enumeration.
            //VisualTreeHelper.HitTest(treeView, null,
            //    new HitTestResultCallback(MyHitTestResult),
            //    new PointHitTestParameters(pt));

            //// Perform actions on the hit test results list.
            //if (hitResultsList.Count > 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("---HitRecount---");
            //    hitResultsList.ForEach(item =>
            //    {
            //        if (item.GetType() == typeof(TextBlock))
            //            System.Diagnostics.Debug.WriteLine("ItemHit: " + (item as TreeViewItem).Parent);
            //    });
            //}
        }

        // Return the result of the hit test to the callback.
        //public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        //{
        //    // Add the hit test result to the list that will be processed after the enumeration.
        //    hitResultsList.Add(result.VisualHit);

        //    // Set the behavior to return visuals at all z-order levels.
        //    return HitTestResultBehavior.Continue;
        //}

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                SetContextMenu(treeViewItem);
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

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

        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem extendedParent = e.Source as TreeViewItem;

            if (!IsDirectory(extendedParent)) return;

            extendedParent.Items.Clear();

            string[] fileEntries = Directory.GetFileSystemEntries(extendedParent.Tag.ToString());
            foreach (string fileEntryPath in fileEntries)
            {
                AddTreeItem(extendedParent, fileEntryPath);
            }
        }

        private void OnDeleteFile(TreeViewItem item)
        {
            System.Diagnostics.Debug.WriteLine("Deleting file: " + item.Tag);
            if (IsFile(item))
            {
                File.Delete(item.Tag.ToString());
                if (item.Parent != null) (item.Parent as TreeViewItem).Items.Remove(item);
            }
            else
            {

                Directory.Delete(item.Tag.ToString(), recursive: true);
                if (item.Parent != null) (item.Parent as TreeViewItem).Items.Remove(item);
            }
        }

        private void OnCreateInFile(TreeViewItem file)
        {
            System.Diagnostics.Debug.WriteLine("Creating file: " + file.Tag);
        }

        private void OnOpenFile(TreeViewItem file)
        {
            System.Diagnostics.Debug.WriteLine("Opening file: " + file.Tag);
            using (var textReader = System.IO.File.OpenText(file.Tag.ToString()))
            {
                string text = textReader.ReadToEnd();
                textBlock.Text = text;
            }
        }
        #endregion Interactions

        #region Inner Mechanics
        private bool IsDirectory(TreeViewItem item)
        {
            if (Directory.Exists(item.Tag.ToString())) return true;
            return false;
        }

        private bool IsDirectory(string path)
        {
            if (Directory.Exists(path)) return true;
            return false;
        }

        private bool IsFile(TreeViewItem item)
        {
            if (File.Exists(item.Tag.ToString())) return true;
            return false;
        }

        private bool IsFile(string path)
        {
            if (File.Exists(path)) return true;
            return false;
        }

        private void SetTreeRoot(string path)
        {
            if (!IsDirectory(path))
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
                if (IsDirectory(item as TreeViewItem))
                {
                    (item as TreeViewItem).Items.Add(new TreeViewItem());
                    (item as TreeViewItem).IsExpanded = false;
                }

            root.IsSelected = true;
            root.IsExpanded = true;
        }

        private void AddTreeItem(TreeViewItem parent, string path)
        {
            if (IsFile(path))
            {
                var temp = GetTreeItem(path);
                parent.Items.Add(temp);
            }
            else if (IsDirectory(path))
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

        private TreeViewItem GetTreeItem(string path, bool isDirectory = false)
        {
            return new TreeViewItem
            {
                Header = isDirectory ? "[D] " + path.Split('\\').Last((e) => e != "") : path.Split('\\').Last((e) => e != ""),
                Tag = path
            };
        }

        private void SetContextMenu(TreeViewItem file)
        {
            if (IsFile(file) && file.Tag.ToString().Split('.').Last() == "txt")
                treeView.ContextMenu = textFileContextMenu;
            else if (IsDirectory(file))
                treeView.ContextMenu = directoryContextMenu;
        }
        #endregion Inner Mechanics
    }
}
