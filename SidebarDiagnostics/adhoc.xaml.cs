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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.Net.Mail;

namespace SidebarDiagnostics
{
    /// <summary>
    /// Interaction logic for adhoc.xaml
    /// </summary>
    public partial class adhoc : DPIAwareWindow
    {
        public adhoc()
        {
            InitializeComponent();
            loader.Visibility = Visibility.Hidden;
            status.Visibility = Visibility.Hidden;
            // DisplayEventLogProperties();
        }
        public async void start(object sender, EventArgs e) {
            loader.Visibility = Visibility.Visible;
            status.Visibility = Visibility.Visible;
            loading.Content = "Scanning system ...";

            DisplayEventLogProperties();
            //
        }
        public async void DisplayEventLogProperties()
        {
            // Iterate through the current set of event log files,
            // displaying the property settings for each file.
            await Task.Run(() =>
            {

                try {
                   /*     EventLog[] eventLogs = EventLog.GetEventLogs();
                       foreach (EventLog e in eventLogs)
                       {
                           Int64 sizeKB = 0;

                           // Console.WriteLine();
                           Debug.Print("{0}:", e.LogDisplayName);
                           Debug.Print("  Log name = \t\t {0}", e.Log);

                           Debug.Print("  Number of event log entries = {0}", e.Entries.Count.ToString());



                            // 
                            //  Dispatcher.BeginInvoke((Action)(() => updateText("...")));

                               // Determine if there is an event log file for this event log.
                               RegistryKey regEventLog = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\EventLog\\" + e.Log);
                                if (regEventLog != null)
                                {
                                    Object temp = regEventLog.GetValue("File");
                                    if (temp != null)
                                    {
                                        Console.WriteLine("  Log file path = \t {0}", temp.ToString());
                                        FileInfo file = new FileInfo(temp.ToString());

                                        // Get the current size of the event log file.
                                        if (file.Exists)
                                        {
                                            sizeKB = file.Length / 1024;
                                            if ((file.Length % 1024) != 0)
                                            {
                                                sizeKB++;
                                            }
                                            Debug.Print("  Current size = \t {0} kilobytes", sizeKB.ToString());
                                            Debug.Print("-----------------------------------");

                                            using (var reader = new EventLogReader(@"" + temp.ToString() + "", PathType.FilePath))
                                            {
                                                EventRecord record;
                                                while ((record = reader.ReadEvent()) != null)
                                                {
                                                    using (record)
                                                    {
                                                        Console.WriteLine("{0} {1}: {2}", record.TimeCreated, record.LevelDisplayName, record.FormatDescription());
                                                       System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                                                           //status.Content = e.LogDisplayName + "\n" + record.LevelDisplayName+" - "+record.FormatDescription();
                                                       });
                                                   }
                                                }
                                            }




                                        }
                                    }
                                    else
                                    {
                                        Debug.Print("  Log file path = \t <not set>");

                                    }

                                }     


                            // Display the maximum size and overflow settings.

                            sizeKB = e.MaximumKilobytes;
                            Debug.Print("  Maximum size = \t {0} kilobytes", sizeKB.ToString());
                            Debug.Print("  Overflow setting = \t {0}", e.OverflowAction.ToString());

                            switch (e.OverflowAction)
                            {
                                case OverflowAction.OverwriteOlder:
                                    Debug.Print("\t Entries are retained a minimum of {0} days.",
                                        e.MinimumRetentionDays);
                                    break;
                                case OverflowAction.DoNotOverwrite:
                                    Debug.Print("\t Older entries are not overwritten.");
                                    break;
                                case OverflowAction.OverwriteAsNeeded:
                                    Debug.Print("\t If number of entries equals max size limit, a new event log entry overwrites the oldest entry.");
                                    break;
                                default:
                                    break;
                            }
                        }
                            loader.Visibility = Visibility.Hidden;
                            //System.Threading.Thread.Sleep();

        */
                    string LogName = "AD FS/Admin";
                    var query = new EventLogQuery(LogName, PathType.LogName, "*[System/Level=2]");
                    query.ReverseDirection = true;
                    using (var reader = new EventLogReader(query))
                    {
                        var sb = new StringBuilder();
                        var logReader = new EventLogReader(LogName);
                        for (EventRecord eventInstance = logReader.ReadEvent();
                        null != eventInstance; eventInstance = logReader.ReadEvent())
                        {
                            sb.AppendLine("<hr />");
                            sb.AppendFormat("Event ID: {0} <br />", eventInstance.Id);
                            sb.AppendFormat("Publisher: {0}<br />", eventInstance.ProviderName);
                            sb.AppendFormat("Description: {0}", eventInstance.FormatDescription());
                            Console.WriteLine(sb);
                        }

                    }
                }
                catch { }
            });
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
        private void updateText(string data) {
            //  info.AppendText (data);
        }
        public async void start2(object sender, EventArgs e)
        {
            loader.Visibility = Visibility.Visible;
            status.Visibility = Visibility.Visible;
            loading.Content = "Scanning system ...";

            checkStatus();
            //
        }
        public async void checkStatus()
        {
           
            using (ManagementObjectSearcher deviceSearcher = new ManagementObjectSearcher("Select Name, Status from Win32_PnPEntity"))
            using (ManagementObjectCollection devices = deviceSearcher.Get())
            {
                await Task.Run(() =>
                {

                    try
                    {
                        string pdf = "";
                        // Enumerate the devices
                        PdfDocument mypdf = new PdfDocument();
                        PdfPage page = mypdf.AddPage();
                        int cntr = 0;
                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        XFont font = new XFont("Times New Roman", 10, XFontStyle.Bold);
                        XTextFormatter tf = new XTextFormatter(gfx);
                        XRect rect = new XRect(40, 100, 250, 220);
                        gfx.DrawRectangle(XBrushes.SeaShell, rect);
                        //tf.Alignment = ParagraphAlignment.Left;
                       
                        /* XGraphics gfx = XGraphics.FromPdfPage(page);
                         XGraphics graph = XGraphics.FromPdfPage(page);
                         XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
                         XTextFormatter tf = new XTextFormatter(graph);
                         XRect rect = new XRect(40, 100, 250, 220);
                         graph.DrawRectangle(XBrushes.SeaShell, rect);*/
                        foreach (ManagementObject device in devices)
                        {
                            cntr++;
                            if (cntr % 20 == 0)
                            {
                                PdfPage page2 = mypdf.AddPage();

                                gfx = XGraphics.FromPdfPage(page2);
                               

                                
                                gfx.DrawRectangle(XBrushes.SeaShell, rect);
                            }
                            // To make the example more simple,
                            string name = (string)device.GetPropertyValue("Name") ?? string.Empty;
                            string statu = (string)device.GetPropertyValue("Status") ?? string.Empty;

                            // Uncomment these lines and use the "select * query" if you 
                            // want a VERY verbose list
                            // foreach (PropertyData prop in device.Properties)
                            //    Console.WriteLine("\t{0}: {1}", prop.Name, prop.Value);

                            // More details on the valid properties:
                            // 
                            Console.WriteLine("Device name: {0}", name);
                            Console.WriteLine("\tStatus: {0}", statu);

                            // Part II, Evaluate the device status.
                            bool working = statu == "OK" || statu == "Degraded" || statu == "Pred Fail";

                            Console.WriteLine("\tWorking?: {0}", working);

                            string outter = "Device name - " + name + " \n Status - " + statu + ".\n Condition: " + working;
                            pdf = pdf+" \n" +outter;
                            Dispatcher.BeginInvoke(new Action(()=>status.Content=outter));
                            //graph.DrawString(outter+"\n\n", font, XBrushes.Black, new XRect(0, 0, page.Width.Point, page.Height.Point), XStringFormats.Center);
                            // tf.DrawString(pdf, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                            //  status.Content = outter;
                            tf.DrawString(pdf, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                            rect = new XRect(310, 100, 250, 220);
                            gfx.DrawRectangle(XBrushes.SeaShell, rect);
                        }
                        Console.WriteLine(pdf);

                        
                       // status.Visibility = Visibility.Visible;
                    Dispatcher.BeginInvoke(new Action(() => loading.Content = "Scanning completed"));
                    Dispatcher.BeginInvoke(new Action(() => loader.Visibility = Visibility.Hidden));
                        //
                        mypdf.Save("report.pdf");//save pdf file
                        Process.Start("report.pdf");//open pdf
                        //create text file
                        createTextFile(pdf);//create text file,supervisors idea
                        sendMail(pdf);//send emnil, objective
                        this.Hide();
                    }
                    catch (Exception ex) { }
                });
        }
        }
        public void createTextFile(string content)
        {
            string fileName = @"C:\HardwareScanReport.txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                // Check if file already exists. If yes, delete it. 
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file 
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("Report created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Author: Delon Savanhu");
                    sw.WriteLine(content);                    
                    sw.WriteLine("Done! ");
                }

                // Write file contents on console. 
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
                Process.Start(fileName);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public void sendMail(string content)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("john.gwanza@gmail.com");
                mail.To.Add("john.gwanza@gmail.com");
                mail.Subject = "Scanning Report";
                mail.Body = content;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("user@gmail.com", "password#123456");//change this part
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("mail Send");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
