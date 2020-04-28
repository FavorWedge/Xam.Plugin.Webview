using System;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Webkit;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace Xam.Plugin.WebView.Droid
{
    public class FormsWebViewClient : WebViewClient
    {
        private readonly Context _context;
        private readonly WeakReference<FormsWebViewRenderer> _reference;

        public FormsWebViewClient(FormsWebViewRenderer renderer, Context context)
        {
            _reference = new WeakReference<FormsWebViewRenderer>(renderer);
            _context = context;
        }

        public override void OnReceivedHttpError(Android.Webkit.WebView view, IWebResourceRequest request,
            WebResourceResponse errorResponse)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.HandleNavigationError(errorResponse.StatusCode, false);
            renderer.Element.HandleNavigationCompleted(request.Url.ToString());
            renderer.Element.Navigating = false;
        }

        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request,
            WebResourceError error)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.HandleNavigationError((int) error.ErrorCode, true);
            renderer.Element.HandleNavigationCompleted(request.Url.ToString());
            renderer.Element.Navigating = false;
            view.StopLoading();
        }

        //For Android < 5.0
        [Obsolete]
        public override void OnReceivedError(Android.Webkit.WebView view, [GeneratedEnum] ClientError errorCode,
            string description, string failingUrl)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) return;

            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.HandleNavigationError((int) errorCode, true);
            renderer.Element.HandleNavigationCompleted(failingUrl);
            renderer.Element.Navigating = false;
        }

        public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) goto EndShouldOverrideUrlLoading;
            if (renderer.Element == null) goto EndShouldOverrideUrlLoading;

            if (renderer.Element.HandleUrlLinkClicked(request.Url.ToString()).OffloadOntoDevice)
            {
                if (AttemptToHandleCustomUrlScheme(view, request.Url.ToString()))
                    return true;
            }

            EndShouldOverrideUrlLoading:
            return base.ShouldOverrideUrlLoading(view, request);
        }

        //For Android < 5.0
        [Obsolete]
        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, string url)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) goto EndShouldInterceptRequest;

            if (_reference == null || !_reference.TryGetTarget(out var renderer)) goto EndShouldInterceptRequest;
            if (renderer.Element == null) goto EndShouldInterceptRequest;

            var response = renderer.Element.HandleNavigationStartRequest(url);

            if (response.Cancel || response.OffloadOntoDevice)
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (response.OffloadOntoDevice)
                        AttemptToHandleCustomUrlScheme(view, url);

                    view.StopLoading();
                });

            EndShouldInterceptRequest:
            return base.ShouldInterceptRequest(view, url);
        }

        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view,
            IWebResourceRequest request)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) goto EndShouldInterceptRequest;
            if (renderer.Element == null) goto EndShouldInterceptRequest;

            var url = request.Url.ToString();
            var response = renderer.Element.HandleNavigationStartRequest(url);

            if (response.Cancel || response.OffloadOntoDevice)
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (response.OffloadOntoDevice)
                        AttemptToHandleCustomUrlScheme(view, url);

                    view.StopLoading();
                });

            EndShouldInterceptRequest:
            return base.ShouldInterceptRequest(view, request);
        }

        private void CheckResponseValidity(Android.Webkit.WebView view, string url)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            var response = renderer.Element.HandleNavigationStartRequest(url);

            if (response.Cancel || response.OffloadOntoDevice)
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (response.OffloadOntoDevice)
                        AttemptToHandleCustomUrlScheme(view, url);

                    view.StopLoading();
                });
        }

        public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            renderer.Element.Navigating = true;
        }

        private bool AttemptToHandleCustomUrlScheme(Android.Webkit.WebView view, string url)
        {
            if (url.StartsWith("mailto", StringComparison.Ordinal))
            {
                var emailData = MailTo.Parse(url);

                var email = new Intent(Intent.ActionSendto);

                email.SetData(Uri.Parse("mailto:"));
                email.PutExtra(Intent.ExtraEmail, new[] {emailData.To});
                email.PutExtra(Intent.ExtraSubject, emailData.Subject);
                email.PutExtra(Intent.ExtraCc, emailData.Cc);
                email.PutExtra(Intent.ExtraText, emailData.Body);

                if (email.ResolveActivity(_context.PackageManager) != null)
                    _context.StartActivity(email);

                return true;
            }

            if (url.StartsWith("http", StringComparison.Ordinal))
            {
                var webPage = new Intent(Intent.ActionView, Uri.Parse(url));
                if (webPage.ResolveActivity(_context.PackageManager) != null)
                    _context.StartActivity(webPage);

                return true;
            }

            return false;
        }

        public override void OnReceivedSslError(Android.Webkit.WebView view, SslErrorHandler handler, SslError error)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            if (FormsWebViewRenderer.IgnoreSSLGlobally)
            {
                handler.Proceed();
            }

            else
            {
                handler.Cancel();
                renderer.Element.Navigating = false;
            }
        }

        public override async void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            if (_reference == null || !_reference.TryGetTarget(out var renderer)) return;
            if (renderer.Element == null) return;

            // Add Injection Function
            await renderer.OnJavascriptInjectionRequest(FormsWebView.InjectedFunction);

            // Add Global Callbacks
            if (renderer.Element.EnableGlobalCallbacks)
                foreach (var callback in FormsWebView.GlobalRegisteredCallbacks)
                    await renderer.OnJavascriptInjectionRequest(FormsWebView.GenerateFunctionScript(callback.Key));

            // Add Local Callbacks
            foreach (var callback in renderer.Element.LocalRegisteredCallbacks)
                await renderer.OnJavascriptInjectionRequest(FormsWebView.GenerateFunctionScript(callback.Key));

            renderer.Element.CanGoBack = view.CanGoBack();
            renderer.Element.CanGoForward = view.CanGoForward();
            renderer.Element.Navigating = false;

            renderer.Element.HandleNavigationCompleted(url);
            renderer.Element.HandleContentLoaded();
        }
    }
}