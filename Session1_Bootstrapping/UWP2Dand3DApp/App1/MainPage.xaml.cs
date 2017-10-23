using HolographicApp1;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    async void OnButtonClick(object sender, RoutedEventArgs e)
    {
      // We only go 2D->3D right now, we could change that later on if
      // we wanted to.
      var newView = CoreApplication.CreateNewView(new AppViewSource());

      ApplicationView appView = null;

      await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        () =>
        {
          appView = ApplicationView.GetForCurrentView();
          CoreWindow.GetForCurrentThread().Activate();
        }
      );
      await ApplicationViewSwitcher.TryShowAsStandaloneAsync(appView.Id);
    }
  }
}
