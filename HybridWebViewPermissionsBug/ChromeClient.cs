using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//===COPY AND PASTE FROM: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/webview?view=net-maui-8.0&pivots=devices-android
#if ANDROID
using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace MauiAppWebViewHandlers.Platforms.Android;

internal class MyWebChromeClient : MauiWebChromeClient {
    public MyWebChromeClient(IWebViewHandler handler) : base(handler) {

    }

    public override void OnPermissionRequest(PermissionRequest request) {
        // Process each request
        foreach (var resource in request.GetResources()) {
            // Check if the web page is requesting permission to the camera
            if (resource.Equals(PermissionRequest.ResourceVideoCapture, StringComparison.OrdinalIgnoreCase)) {
                // Get the status of the .NET MAUI app's access to the camera
                PermissionStatus status = Permissions.CheckStatusAsync<Permissions.Camera>().Result;

                // Deny the web page's request if the app's access to the camera is not "Granted"
                if (status != PermissionStatus.Granted)
                    request.Deny();
                else
                    request.Grant(request.GetResources());

                return;
            }
        }

        base.OnPermissionRequest(request);
    }
}
#endif
//===================