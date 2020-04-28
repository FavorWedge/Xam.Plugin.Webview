using System;
using System.Diagnostics;
using Xam.Plugin.WebView.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SampleApp.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentPlayerSample : ContentPage
    {
        private readonly string _courseBaseUrl;
        private readonly string _courseDataUrl;
        private readonly string _courseLocale;
        private readonly string _frameworkUrl;

        public ContentPlayerSample()
        {
            InitializeComponent();

            _frameworkUrl = "https://elearning-stage.sistemlms.com/dev_content_player_versioned/2.1.25/";
            _courseLocale = "en-US";
            _courseBaseUrl = "https://elearning.alchemysystems.com/content/courses/UDM/UDM29/en-US/1.0/";
            _courseDataUrl = "https://elearning.alchemysystems.com/content/courses/UDM/UDM29/en-US/1.0/locale/en-US/xml/config.json";

            WebContent.OnContentLoaded += HybridWebViewSampleOnOnContentLoaded;
        }

        private void HybridWebViewSampleOnOnContentLoaded(object sender, EventArgs e)
        {
            if (!(sender is FormsWebView obj)) return;
            obj.InjectJavascriptAsync("var controller = new HybridController({ courseBaseUrl:'" + _courseBaseUrl +
                                      "',courseDataUrl:'" + _courseDataUrl + "',manifestUrl:'" + _frameworkUrl +
                                      "manifest.json',frameworkUrl: '" + _frameworkUrl + "',locale: '" +
                                      _courseLocale + "',uiLocale: '" + _courseLocale + "',});")
                .ConfigureAwait(false);
            obj.InjectJavascriptAsync(
                    "this.controller.addUser('9855','Thon Becker');")
                .ConfigureAwait(false);
            obj.InjectJavascriptAsync("this.controller.startCourse();").ConfigureAwait(false);
        }

        private void HandleExitCourse(string obj)
        {
            Debug.WriteLine($"Got callback: {obj}");
        }

        private void AddCallback(object sender, EventArgs e)
        {
            WebContent.AddLocalCallback("exitCourseHandler", HandleExitCourse);
        }

        private void CallCallback(object sender, EventArgs e)
        {
            WebContent.InjectJavascriptAsync("this.controller.windowClose();").ConfigureAwait(false);
        }
    }
}