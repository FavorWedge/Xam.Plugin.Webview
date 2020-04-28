using Android.Content;
using Android.Net;
using Android.Webkit;

namespace Xam.Plugin.WebView.Droid
{
    public interface IFilePathCallback
    {
        IValueCallback FilePathCallback { get; set; }
        Uri CapturedImageUri { get; set; }
        void StartActivityForResult(Intent intent, int requestCode);
    }
}
