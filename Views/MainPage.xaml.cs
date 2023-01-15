using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Controls;
using ToggleHypervisor.ViewModels;

namespace ToggleHypervisor.Views
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainPageViewModel>();
        }
    }
}
