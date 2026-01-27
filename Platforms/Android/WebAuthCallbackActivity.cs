using Android.App;
using Android.Content.PM;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui;

[Activity(
    Exported = true,
    NoHistory = true,
    LaunchMode = LaunchMode.SingleTop)]
[IntentFilter(
    new[] { Android.Content.Intent.ActionView },
    Categories = new[]
    {
        Android.Content.Intent.CategoryDefault,
        Android.Content.Intent.CategoryBrowsable
    },
    DataScheme = "com.companyname.clientmaui",
    DataHost = "oauth2redirect"
)]
public class WebAuthCallbackActivity : WebAuthenticatorCallbackActivity
{
}
