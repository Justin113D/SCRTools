﻿using SCR.Tools.DialogEditor.Viewmodeling.Simulator;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCR.Tools.DialogEditor.WPF.UserControls.Simulator
{
    /// <summary>
    /// Interaction logic for UcSimulatorTable.xaml
    /// </summary>
    public partial class UcSimulatorTable : UserControl
    {
        public VmSimulator ViewModel
            => (VmSimulator)DataContext;

        public UcSimulatorTable()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ListBox)sender).SelectedValue is not VmSimulatorNode node)
            {
                return;
            }

            ViewModel.Jump(node);
        }
    }
}
