using System;
using Android;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Webkit;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace Xam.Plugin.WebView.Droid
{
    public class FormsWebViewChromeClient : WebChromeClient
    {
        public static int FILECHOOSER_RESULTCODE = 1;

        readonly IFilePathCallback _mainActivity;

        public FormsWebViewChromeClient(IFilePathCallback mainActivity)
        {
            _mainActivity = mainActivity;
        }

        private File CreateImageFile()
        {
            // Create unique image file name
            string imageFileName = "Playbook_" + DateTime.Now.Ticks;

            File storageDir = Environment.GetExternalStoragePublicDirectory(
                Environment.DirectoryPictures);

            return File.CreateTempFile(imageFileName, ".jpg", storageDir);
        }

        public override bool OnShowFileChooser(Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            try
            {
                _mainActivity.FilePathCallback = filePathCallback;
                Intent contentSelectionIntent = new Intent(Intent.ActionGetContent);
                contentSelectionIntent.AddCategory(Intent.CategoryOpenable);

                Intent mediaCaptureIntent;

                // Currently only image and video types are supported
                if (fileChooserParams.GetAcceptTypes().Length > 0 &&
                    string.CompareOrdinal(fileChooserParams.GetAcceptTypes()[0], "image/*") == 0)
                {
                    contentSelectionIntent.SetType("image/*");
                    mediaCaptureIntent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
                    _mainActivity.CapturedImageUri = Uri.FromFile(CreateImageFile());
                    mediaCaptureIntent.PutExtra(Android.Provider.MediaStore.ExtraOutput, _mainActivity.CapturedImageUri);
                }
                else
                {
                    contentSelectionIntent.SetType("video/*");
                    mediaCaptureIntent = new Intent(Android.Provider.MediaStore.ActionVideoCapture);
                }

                mediaCaptureIntent.AddFlags(ActivityFlags.GrantReadUriPermission);

                Intent chooserIntent = new Intent(Intent.ActionChooser);
                chooserIntent.PutExtra(Intent.ExtraIntent, contentSelectionIntent);
                Intent[] intentArray = { mediaCaptureIntent };
                chooserIntent.PutExtra(Intent.ExtraInitialIntents, intentArray);

                _mainActivity.StartActivityForResult(chooserIntent, FILECHOOSER_RESULTCODE);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}