using PetryNet.Commands;
using PetryNet.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PetryNet.ViewModels
{
    public interface ISelectItems
    {
        RelayCommand SelectItemCommand { get; }
    }

    public abstract class SelectableElementViewModelBase : BaseViewModel, ISelectItems
    {
        private bool isSelected;
        public IPetryNetViewModel Parent { get; set; }
        public RelayCommand SelectItemCommand { get; private set; }
        public int Id { get; set; }

        public SelectableElementViewModelBase(int id, IPetryNetViewModel parent)
        {
            Id = id;
            Parent = parent;
            Init();
        }

        public SelectableElementViewModelBase()
        {
            Init();
        }

        public List<SelectableElementViewModelBase> SelectedItems
        {
            get { return Parent.SelectedItems; }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {

                    SetProperty(ref isSelected, value);
                }
            }
        }

        private void ExecuteSelectItemCommand(object param)
        {
            bool newSelect = param != null && (bool)param;
            SelectItem(newSelect, !IsSelected);
        }

        private void SelectItem(bool newselect, bool select)
        {
            if (newselect)
            {
                foreach (var designerItemViewModelBase in Parent.SelectedItems.ToList())
                {
                    designerItemViewModelBase.isSelected = false;
                }
            }

            IsSelected = select;
        }

        private void Init()
        {
            SelectItemCommand = new RelayCommand(ExecuteSelectItemCommand);
        }

        
    }
}
