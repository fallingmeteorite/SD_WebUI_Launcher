using System.Diagnostics;
using Wpf.Ui.Common.Interfaces;

namespace Awake.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }
    }
}