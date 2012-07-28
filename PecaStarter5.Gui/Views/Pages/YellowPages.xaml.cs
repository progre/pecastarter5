using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Progressive.PecaStarter5.Commons.Models;
using Progressive.PecaStarter5.Gui.Views.Pages;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;

namespace Progressive.PecaStarter5.Views.Pages
{
    /// <summary>
    /// Parameters.xaml の相互作用ロジック
    /// </summary>
    public partial class YellowPages : UserControl
    {
        public YellowPages()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((((Hyperlink)sender).DataContext as string));
        }

        /// <summary>
        /// (DataContextの変更通知を受けて)Viewを更新
        /// disabled?
        /// </summary>
        public void UpdateTarget()
        {
            foreach (var input in InputsStackPanel.Children)
            {
                if (!(input is UserControl))
                {
                    return;
                }
                var content = ((UserControl)input).Content;
                if (content is ComboBox)
                {
                    ((ComboBox)content).GetBindingExpression(ComboBox.SelectedValueProperty).UpdateTarget();
                    return;
                }
                if (content is TextBox)
                {
                    ((TextBox)content).GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    return;
                }
            }
            foreach (var input in CheckboxesStackPanel.Children)
            {
                if (!(input is UserControl))
                {
                    return;
                }
                ((CheckBox)((UserControl)input).Content).GetBindingExpression(CheckBox.IsCheckedProperty).UpdateTarget();
                return;
            }
        }

        /// <summary>
        /// コントロールを生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Clear();
            DynamicStringDictionary parameters;
            try
            {
                parameters = ((dynamic)e.NewValue).Parameters;
            }
            catch (RuntimeBinderException)
            {
                return;
            }
            int i = 11;
            foreach (string key in parameters.Dictionary.Keys)
            {
                ComponentFactory.AddComponent(
                    LabelsStackPanel.Children, InputsStackPanel.Children, CheckboxesStackPanel.Children,
                    key, parameters, i);
                i++;
            }
        }

        private void Clear()
        {
            LabelsStackPanel.Children.Clear();
            InputsStackPanel.Children.Clear();
            CheckboxesStackPanel.Children.Clear();
        }
    }
}
