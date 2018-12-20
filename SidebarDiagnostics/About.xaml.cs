using System;
using System.Collections.Generic;
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

using System.Windows.Navigation;

using System.Diagnostics;
using System.Management;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics.Eventing.Reader;

using SidebarDiagnostics.Models;
using SidebarDiagnostics.Windows;
using System.ComponentModel;


namespace SidebarDiagnostics
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class about : DPIAwareWindow
    {
        public about()
        {
            InitializeComponent(); 
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            /*  DataContext = null;

              if (Model != null)
              {
                  Model.Dispose();
                  Model = null;
              }*/
        }
    }
}
