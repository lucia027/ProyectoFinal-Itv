using System.Windows;
using Itv.Wpf.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace Itv.Wpf.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}