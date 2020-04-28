using System;
using System.Threading.Tasks;
using Xam.Plugin.WebView.Abstractions.Enumerations;

namespace Xam.Plugin.WebView.Abstractions
{
    public interface IFormsWebView
    {

        event EventHandler<NavigationResponseArgs> OnNavigationStarted;

        event EventHandler<NavigationResponseArgs> OnNavigationCompleted;

        event EventHandler<NavigationResponseArgs> OnNavigationError;

        event EventHandler OnContentLoaded;

        WebViewContentType ContentType { get; set; }

        string Source { get; set; }

        string BaseUrl { get; set; }

        bool EnableGlobalCallbacks { get; set; }

        bool EnableGlobalHeaders { get; set; }

        bool Navigating { get; }

        bool CanGoBack { get; }

        bool CanGoForward { get; }

        void GoBack();

        void GoForward();

        void Refresh();

        Task<string> InjectJavascriptAsync(string js);

        void AddLocalCallback(string functionName, Action<string> action);

        void RemoveLocalCallback(string functionName);

        void RemoveAllLocalCallbacks();
        Task ClearCookiesAsync();
    }
}
