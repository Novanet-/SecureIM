using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SecureIM.ChatGUI.ViewModel.interfaces;

namespace SecureIM.ChatGUI.ViewModel.alternativeViews
{
    public class ViewModelCustomStyleExampleWindow : ViewModelExampleBase, IViewModelCustomStyleExampleWindow
    {
        public ViewModelCustomStyleExampleWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            var view = CollectionViewSource.GetDefaultView(ItemCollection) as ICollectionView;
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
        }
    }
}