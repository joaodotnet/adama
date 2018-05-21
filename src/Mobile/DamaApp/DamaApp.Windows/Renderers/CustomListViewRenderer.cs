using DamaApp.Windows.Renderers;
using System;
using Windows.UI.Xaml.Controls;

[assembly: Xamarin.Forms.Platform.UWP.ExportRenderer(typeof(DLToolkit.Forms.Controls.FlowListView), typeof(CustomListViewRenderer))]
namespace DamaApp.Windows.Renderers
{
    using Xamarin.Forms.Platform.UWP;
    
    public class CustomListViewRenderer : ListViewRenderer
    {
        
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);



            if (List != null)

                List.SelectionMode = ListViewSelectionMode.None;

        }

    }
}
