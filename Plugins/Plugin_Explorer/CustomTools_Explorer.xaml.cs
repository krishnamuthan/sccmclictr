﻿using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AgentActionTools
{
    /// <summary>
    /// Interaction logic for CustomTools_Explorer.xaml
    /// </summary>
    public partial class CustomTools_Explorer : System.Windows.Controls.UserControl
    {
        public CustomTools_Explorer()
        {
            InitializeComponent();

            foreach (string sPath in xmlFolders)
            {
                try
                {
                    RibbonButton bR = new RibbonButton();
                    bR.Label = sPath;
                    bR.Tag = sPath;
                    bR.SmallImageSource = new BitmapImage(new Uri(@"/Plugin_Explorer;component/Images/shell32.dll_I010b_0409.ico", UriKind.Relative));
                    bR.ToolTip = sPath;
                    bR.Click += btC_Click;
                    btExplore.Items.Add(bR);
                }
                catch { }
            }
        }

        private void btC_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (((RibbonButton)sender).Tag != null)
                {
                    string sTag = ((RibbonButton)sender).Tag.ToString();
                    string sShare = "";
                    switch (sTag)
                    {
                        case "C":
                            sShare = "C$";
                            break;
                        case "Admin":
                            sShare = "Admin$";
                            break;
                        case "WBEM":
                            sShare = @"Admin$\System32\wbem";
                            break;
                        case "ccmsetup":
                            sShare = @"Admin$\ccmsetup\logs";
                            break;
                        case "CCMLOGS":
                            sShare = @"Admin$\ccm\logs";
                            break;
                        default:
                            sShare = sTag;
                            break;
                    }
                    Type t = System.Reflection.Assembly.GetEntryAssembly().GetType("ClientCenter.Common", false, true);
                    //Get the Hostname
                    System.Reflection.PropertyInfo pInfo = t.GetProperty("Hostname");
                    string sHost = (string)pInfo.GetValue(null, null);

                    Process Explorer = new Process();
                    Explorer.StartInfo.FileName = "Explorer.exe";
                    Explorer.StartInfo.Arguments = @"\\" + sHost + @"\" + sShare;
                    Explorer.Start();
                }
            }
            catch { }
        }

        public static string ConfigPath
        {
            get
            {
                //Get XML Settings
                return Assembly.GetExecutingAssembly().Location + ".config";
            }
        }

        internal static StringCollection xmlFolders
        {
            get
            {
                try
                {
                    StringCollection sResults = new StringCollection();
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(ConfigPath);
                    var xNodes = xDoc.SelectNodes("//configuration/applicationSettings/AgentActionTools.Properties.Settings/setting[@name='Folders']/value/ArrayOfString/string");
                    foreach(XmlNode xNode in xNodes)
                    {
                        sResults.Add(xNode.InnerText.ToString());
                    }

                    return sResults;
                }
                catch { }

                return Properties.Settings.Default.Folders;
            }
        }
    }
}
