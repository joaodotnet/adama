using DamaNoJornal.Windows.Renderers;

[assembly: Xamarin.Forms.Platform.UWP.ExportRenderer(typeof(DLToolkit.Forms.Controls.FlowListView), typeof(CustomListViewRenderer))]
namespace DamaNoJornal.Windows.Renderers
{
    using Xamarin.Forms.Platform.UWP;

    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (List != null)
                List.SelectionMode = global::Windows.UI.Xaml.Controls.ListViewSelectionMode.None;
        }
    }
}
