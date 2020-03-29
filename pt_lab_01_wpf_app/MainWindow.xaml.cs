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
            System.Diagnostics.Debug.Write("Initializing content.");
            selectedPath = "c:\\";
            var root = new TreeViewItem
            {
                Header = selectedPath,
                Tag = selectedPath
            };
            treeView.Items.Add(root);
        }

        public void OnMenuButtonOpenClick(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.SelectedPath = selectedPath;
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                treeView.Items.Clear();
                selectedPath = dlg.SelectedPath;
            }
            else if (result == System.Windows.Forms.DialogResult.Cancel)
                return;

            //System.Diagnostics.Debug.WriteLine("selectedPath: " + selectedPath);

            var root = new TreeViewItem
            {
                Header = dlg.SelectedPath,
                Tag = dlg.SelectedPath
            };
            treeView.Items.Add(root);
            //root.Items.Add(item);
        }

        public void OnMenuButtonExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
