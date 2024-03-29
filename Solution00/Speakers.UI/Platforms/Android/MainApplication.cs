﻿using Android.App;
using Android.Runtime;
using System.Diagnostics;

namespace Speakers.UI
{
//#if DEBUG
//    [Application(UsesCleartextTraffic = true, NetworkSecurityConfig = "@xml/network_security_config")]
//#else
//    [Application()]
//#endif

    [Application()]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}