using Java.Lang;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Diagnostics;


namespace HybridWebViewPermissionsBug
{
    public partial class App : Application
    {
        protected override Window CreateWindow(IActivationState? activationState)
        {
            ContentPage mainPage = new();
            HybridWebView webView = new();
            mainPage.Content = webView;
#if ANDROID

            webView.HandlerChanged += delegate {
                Debug.Assert(webView.Handler != null, "Value must not be null.");
                var platformView = webView.ToPlatform(webView.Handler.MauiContext);
                Debug.WriteLine("PLATFORM VIEW " + platformView.GetType());//Microsoft.Maui.Platform.MauiHybridWebView
                Debug.WriteLine("HANDLER PLATFORM VIEW " + webView.Handler.PlatformView.GetType()); //Microsoft.Maui.Platform.MauiHybridWebView
                Debug.WriteLine("HANDLER " + webView.Handler.GetType());//Microsoft.Maui.Handlers.HybridWebViewHandler
                var webViewHandler = (IWebViewHandler)webView.Handler;
                Debug.Assert(webViewHandler != null, "Value must not be null.");
                var chromeClient = new MauiAppWebViewHandlers.Platforms.Android.MyWebChromeClient(webViewHandler);
                webViewHandler.PlatformView.SetWebChromeClient(chromeClient);
            };


#endif
            return new Window(mainPage);
        }
    }
}
