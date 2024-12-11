using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


//===COPY AND PASTE FROM: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/webview?view=net-maui-8.0&pivots=devices-android
#if ANDROID
using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

using Android.App;

namespace HybridWebViewPermissionsBug;

internal class MyWebChromeClient : MauiWebChromeClient {
    //=========================================================================================
    // CUSTOM FIX FOR PERMISSIONS ISSUE https://github.com/dotnet/maui/issues/25784
    //=========================================================================================
    public MyWebChromeClient(IWebViewHandler fakeHandler, HybridWebViewHandler realHandler) : base(fakeHandler) {
        //base constructor (base(fakeHandler)) is automatically called before customSetContext(realHandler) 
        customSetContext(realHandler);
    }
    public void customSetContext(HybridWebViewHandler handler) {
        //copied and pasted from https://github.com/dotnet/maui/blob/07f41ae8209b9f00e784dacb87d70860622ff4e9/src/Core/src/Platform/Android/MauiWebChromeClient.cs#L12
        var activity = handler?.MauiContext?.Context?.GetActivity();

        if (activity == null) {
            Debug.WriteLine("FAILED TO GET ACTIVITY");
            //handler?.MauiContext?.CreateLogger<WebViewHandler>()?.LogWarning($"Failed to set the activity of the WebChromeClient, can't show pickers on the Webview");
        }

        // UsE reflection to access the private _activityRef field in the base class
        var fieldInfo = typeof(MauiWebChromeClient).GetField("_activityRef", BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo != null) {
            // Create a new WeakReference<Activity> to wrap the provided activity
            var activityWeakRef = new WeakReference<Android.App.Activity>(activity);

            // Set the value of _activityRef using reflection
            fieldInfo.SetValue(this, activityWeakRef);
        }
        else {
            Debug.WriteLine("FAILED TO GET _ACTIVITYREF BY REFLECTION");
        }
        //_activityRef = new WeakReference<Activity>(activity); //can't set as not public

    }
    //=========================================================================================
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