using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using PetryNet.ViewModels;
using PetryNet.ViewModels.Interfaces;
using System.Windows.Controls;
using PetryNet.Views;

namespace PetryNet.AttachedProperties
{
    public static class SelectionProps
    {
        #region EnabledForSelection

        public static readonly DependencyProperty EnabledForSelectionProperty =
            DependencyProperty.RegisterAttached("EnabledForSelection", typeof(bool), typeof(SelectionProps),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnEnabledForSelectionChanged)));

        public static bool GetEnabledForSelection(DependencyObject d)
        {
            return (bool)d.GetValue(EnabledForSelectionProperty);
        }

        public static void SetEnabledForSelection(DependencyObject d, bool value)
        {
            d.SetValue(EnabledForSelectionProperty, value);
        }

        private static void OnEnabledForSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                fe.PreviewMouseLeftButtonDown += Fe_PreviewMouseDown;
            }
            else
            {
                fe.PreviewMouseLeftButtonDown -= Fe_PreviewMouseDown;
            }
        }
        #endregion

        private static bool isSelectionMode = false;

        static SelectionProps()
        {
            // Subscribe to the event when SelectionProps is first used
            Application.Current.MainWindow.DataContextChanged += MainWindow_DataContextChanged;
        }

        // Handler to subscribe to the event when MainViewModel is set
        private static void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MainViewModel mainViewModel)
            {
                mainViewModel.IsSelectionModeChanged += MainViewModel_ArcCreationModeChanged;
            }

            if (e.OldValue is MainViewModel oldViewModel)
            {
                oldViewModel.IsSelectionModeChanged -= MainViewModel_ArcCreationModeChanged;
            }
        }

        private static void MainViewModel_ArcCreationModeChanged(object sender, bool e)
        {
            // Update isCreatingArc flag based on mode
            isSelectionMode = e;
        }

        static void Fe_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (!isSelectionMode)
                return;

            var element = (FrameworkElement)sender;
            var selectableVm = element.DataContext as SelectableElementViewModelBase;

            if (selectableVm != null)
            {
                HandleSelectionLogic(selectableVm); // Extract selection logic into a method
            }

            //if (!isSelectionMode)
            //{
            //    return;
            //}

            //SelectableElementViewModelBase selectableDesignerItemViewModelBase =
            //    (SelectableElementViewModelBase)((FrameworkElement)sender).DataContext;

            //if (selectableDesignerItemViewModelBase != null)
            //{
            //    if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            //    {
            //        if ((Keyboard.Modifiers & (ModifierKeys.Shift)) != ModifierKeys.None)
            //        {
            //            selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
            //        }

            //        if ((Keyboard.Modifiers & (ModifierKeys.Control)) != ModifierKeys.None)
            //        {
            //            selectableDesignerItemViewModelBase.IsSelected = !selectableDesignerItemViewModelBase.IsSelected;
            //        }
            //    }
            //    else if (!selectableDesignerItemViewModelBase.IsSelected)
            //    {
            //        foreach (SelectableElementViewModelBase item in selectableDesignerItemViewModelBase.Parent.SelectedItems)
            //        {

            //            if (item is IPetryNetViewModel)
            //            {
            //                IPetryNetViewModel tmp = (IPetryNetViewModel)item;
            //                foreach (SelectableElementViewModelBase gItem in tmp.Items)
            //                {
            //                    gItem.IsSelected = false;
            //                }

            //            }
            //            if (selectableDesignerItemViewModelBase.Parent is SelectableElementViewModelBase)
            //            {
            //                SelectableElementViewModelBase tmp = (SelectableElementViewModelBase)selectableDesignerItemViewModelBase.Parent;
            //                tmp.IsSelected = false;
            //            }
            //            item.IsSelected = false;
            //        }
            //        //if (selectableDesignerItemViewModelBase is IPetryNetViewModel)
            //        //{
            //        //    IPetryNetViewModel tmp = (IPetryNetViewModel)selectableDesignerItemViewModelBase;
            //        //    foreach (SelectableElementViewModelBase gItem in tmp.Items)
            //        //    {
            //        //        gItem.IsSelected = false;
            //        //    }

            //        //}
            //        selectableDesignerItemViewModelBase.Parent.SelectedItems.Clear();
            //        selectableDesignerItemViewModelBase.IsSelected = true;
            //    }
            //}
            //e.Handled = true;
        }

        private static void HandleSelectionLogic(SelectableElementViewModelBase vm)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
            {
                vm.IsSelected = !vm.IsSelected; // Toggle selection with Shift/Ctrl
            }
            else if (!vm.IsSelected)
            {
                // Clear previous selection
                foreach (var item in vm.Parent.SelectedItems)
                {
                    if (item is IPetryNetViewModel petriNet)
                    {
                        foreach (var gItem in petriNet.Items)
                            gItem.IsSelected = false;
                    }
                    item.IsSelected = false;
                }
                vm.Parent.SelectedItems.Clear();
                vm.IsSelected = true; // Select the clicked item
            }
        }

    }

}
