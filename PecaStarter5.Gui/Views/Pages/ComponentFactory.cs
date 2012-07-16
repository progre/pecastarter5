using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Progressive.PecaStarter5.Gui.Views.Pages
{
    static class ComponentFactory
    {
        public static void AddComponent(
            UIElementCollection labelCollection, UIElementCollection inputCollection,
            UIElementCollection checkBoxCollection,
            string key, object dataContext, int index)
        {
            switch (key)
            {
                case "namespace":
                    AddTextBox(labelCollection, inputCollection, checkBoxCollection, "名前空間:", dataContext, key, index);
                    break;
                case "password":
                    AddPasswordBox(labelCollection, inputCollection, checkBoxCollection, "パスワード:", dataContext, key, index);
                    break;
                case "no_log":
                    AddCheckBox(labelCollection, inputCollection, checkBoxCollection, "statsを記録しない", dataContext, key, index);
                    break;
                case "listeners_visibility":
                    AddCheckBox(labelCollection, inputCollection, checkBoxCollection, "リスナー数を表示しない", dataContext, key, index);
                    break;
                case "listeners_invisibility":
                    AddCheckBox(labelCollection, inputCollection, checkBoxCollection, "リスナー数を表示しない", dataContext, key, index);
                    break;
                case "time_visibility":
                    AddCheckBox(labelCollection, inputCollection, checkBoxCollection, "配信時間を表示する", dataContext, key, index);
                    break;
                case "time_invisibility":
                    AddCheckBox(labelCollection, inputCollection, checkBoxCollection, "配信時間を表示しない", dataContext, key, index);
                    break;
                case "port_bandwidth_check":
                    AddComboBox(labelCollection, inputCollection, checkBoxCollection, "ポート/帯域チェック:", dataContext, key, new Tuple<string, string>[]
                        {
                            Tuple.Create("無効", "0"),
                            Tuple.Create("ポートチェック", "1"),
                            Tuple.Create("ポートチェック+帯域チェック", "2"),
                            Tuple.Create("ポートチェック+帯域チェック(高速回線)", "3")
                        }, "Item1", "Item2", index);
                    break;
                case "outside_display":
                    AddComboBox(labelCollection, inputCollection, checkBoxCollection, "チャンネルの外部掲載:", dataContext, key, new Tuple<string, string>[]
                        {
                            Tuple.Create("全てに掲載", "0"),
                            Tuple.Create("RSSに掲載しない", "1"),
                            Tuple.Create("HTML及び、RSSに掲載しない", "2"),
                        }, "Item1", "Item2", index);
                    break;
            }
        }

        private static void AddTextBox(
            UIElementCollection labelCollection, UIElementCollection inputCollection,
            UIElementCollection checkBoxCollection,
            string labelText, object dataContext, string key, int tabIndex)
        {
            labelCollection.Add(new Label() { Content = labelText });
            var control = new TextBox() { DataContext = dataContext, TabIndex = tabIndex };
            control.SetBinding(TextBox.TextProperty, new Binding(key));
            inputCollection.Add(new UserControl() { Content = control });
            checkBoxCollection.Add(new FrameworkElement());
        }

        private static void AddPasswordBox(
            UIElementCollection labelCollection, UIElementCollection inputCollection,
            UIElementCollection checkBoxCollection,
            string content, object dataContext, string key, int index)
        {
            var viewModel = (INotifyPropertyChanged)dataContext;
            labelCollection.Add(new Label() { Content = content });
            var control = new PasswordBox() { DataContext = dataContext, TabIndex = index };
            dynamic indexAccessible = dataContext;
            control.Password = indexAccessible.Parameters[key];
            PropertyChangedEventHandler onSourceChanged = (sender, e) =>
            {
                if (e.PropertyName != key)
                {
                    return;
                }
                if (control.Password == indexAccessible[key])
                {
                    return;
                }
                control.Password = indexAccessible[key];
            };
            viewModel.PropertyChanged += onSourceChanged;
            control.PasswordChanged += (sender, e) =>
            {
                if (indexAccessible[key] != control.Password)
                {
                    indexAccessible[key] = control.Password;
                }
            };
            control.Unloaded += (sender, e) => viewModel.PropertyChanged -= onSourceChanged;
            inputCollection.Add(new UserControl() { Content = control });
            checkBoxCollection.Add(new FrameworkElement());
        }

        private static void AddCheckBox(
            UIElementCollection labelCollection, UIElementCollection inputCollection,
            UIElementCollection checkBoxCollection,
            string labelText, object dataContext, string key, int tabIndex)
        {
            labelCollection.Add(new FrameworkElement());
            inputCollection.Add(new FrameworkElement());
            var control = new CheckBox() { Content = labelText, TabIndex = tabIndex };
            control.SetBinding(CheckBox.IsCheckedProperty, new Binding(key));
            checkBoxCollection.Add(new UserControl() { Content = control });
        }

        private static void AddComboBox(
            UIElementCollection labelCollection, UIElementCollection inputCollection,
            UIElementCollection checkBoxCollection,
            string labelText, object dataContext, string key, IEnumerable items,
            string displayMemberPath, string selectedValuePath, int tabIndex)
        {
            labelCollection.Add(new Label() { Content = labelText });
            var control = new ComboBox()
            {
                DataContext = dataContext,
                ItemsSource = items,
                DisplayMemberPath = displayMemberPath,
                SelectedValuePath = selectedValuePath,
                TabIndex = tabIndex
            };
            control.SetBinding(ComboBox.SelectedValueProperty, new Binding(key));
            inputCollection.Add(new UserControl() { Content = control });
            checkBoxCollection.Add(new FrameworkElement());
        }
    }
}
