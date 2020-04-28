using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

[assembly: AssemblyTitle("Xam.Plugin.WebView.iOS")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("${AuthorCopyright}")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.*")]

// Add permissions to support new WebView renderer
[assembly: ExportRenderer(typeof(WebView), typeof(WkWebViewRenderer))]