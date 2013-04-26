﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using sccmclictr.automation;
using System.Management.Automation;

namespace ClientCenter
{
    /// <summary>
    /// Interaction logic for AgentComponents.xaml
    /// </summary>
    public partial class CollectionVariables : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;

        public CollectionVariables()
        {
            InitializeComponent();
        }

        public SCCMAgent SCCMAgentConnection
        {
            get
            {
                return oAgent;
            }
            set
            {
                if (value.isConnected)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        oAgent = value;
                        oAgent.PSCode.Listeners.Clear();
                        try
                        {
                            
                            //Store Collection Variables for 30s
                            List<PSObject> sRes = oAgent.Client.GetObjectsFromPS(Properties.Resources.PSCollDecode, false, new TimeSpan(0,0,30));
                            
                            //Need to reconnect otherwise following calls will fail because of some evil code in the script...
                            oAgent.connect();

                            //Get reuslts...
                            foreach (PSObject po in sRes)
                            {
                                if (po.ImmediateBaseObject != null)
                                {
                                    if (po.ImmediateBaseObject.GetType() == typeof(System.Collections.Hashtable))
                                    {
                                        //returning Object is a hashtable
                                        System.Collections.Hashtable hTable = (System.Collections.Hashtable)po.ImmediateBaseObject;
                                        
                                        //cleanup returning data
                                        System.Collections.Hashtable hRes = new System.Collections.Hashtable();
                                        foreach (System.Collections.DictionaryEntry entry in hTable)
                                        {
                                            hRes.Add(entry.Key, entry.Value.ToString().Replace("\0", ""));
                                        }

                                        dataGrid1.BeginInit();
                                        dataGrid1.ItemsSource = hRes;
                                        dataGrid1.EndInit();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    catch { }
                    oAgent.PSCode.Listeners.Add(Listener);
                    Listener.WriteLine("Get-Wmiobject -Namespace \"root\\ccm\\Policy\\Machine\\ActualConfig\" -Class \"CCM_CollectionVariable\"");
                    Listener.WriteLine("#... and some decoding stuff ;-) ...");
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }
    }
}
