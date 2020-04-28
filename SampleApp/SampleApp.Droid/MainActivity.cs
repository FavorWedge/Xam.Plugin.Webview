using Android.App;
using Android.Net;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using Xam.Plugin.WebView.Droid;
using Android.Runtime;
using Android.Support.V4.App;
using Android;

namespace SampleApp.Droid
{
    [Activity(Label = "SampleApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IFilePathCallback
    {
        public IValueCallback FilePathCallback { get; set; }
        public Uri CapturedImageUri { get; set; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);

            FormsWebViewRenderer.Initialize();

            global::Xamarin.Forms.Forms.Init(this, bundle);

            RequestPermissions();

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (data is null)
                FilePathCallback.OnReceiveValue(null);
            else
            {
                if (requestCode == FormsWebViewChromeClient.FILECHOOSER_RESULTCODE)
                {
                    if (FilePathCallback is null)
                    {
                        return;
                    }
                    var result = WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data);
                    if (result is null)
                    {
                        if (!(CapturedImageUri is null))
                            result = new Uri[] { CapturedImageUri };
                    }
                    FilePathCallback.OnReceiveValue(result);
                    FilePathCallback = null;
                    CapturedImageUri = null;
                }
            }
        }

        private void RequestPermissions()
        {
            var permissions = new string[]
            {
                Manifest.Permission.Camera,
                Manifest.Permission.CaptureAudioOutput,
                Manifest.Permission.CaptureVideoOutput,
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ReadExternalStorage
            };

            ActivityCompat.RequestPermissions(this, permissions, 5);
        }
    }
}

