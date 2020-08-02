#pragma checksum "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d501cb933678947705b8043e692348be6b61fbfd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views__ViewStart), @"mvc.1.0.view", @"/Views/_ViewStart.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/_ViewStart.cshtml", typeof(AspNetCore.Views__ViewStart))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewImports.cshtml"
using TMS;

#line default
#line hidden
#line 1 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml"
using TMS.Models;

#line default
#line hidden
#line 2 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml"
using System.Security.Claims;

#line default
#line hidden
#line 3 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d501cb933678947705b8043e692348be6b61fbfd", @"/Views/_ViewStart.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4b920ee378507dd0789e3303ed664c79361ef06d", @"/Views/_ViewImports.cshtml")]
    public class Views__ViewStart : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 4 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml"
  var identity = Context.User.Identity as ClaimsIdentity; 

#line default
#line hidden
#line 5 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewStart.cshtml"
  
    if (this.User.IsInRole("INSTRUCTOR") || !this.User.Identity.IsAuthenticated)
    {
        Layout = "~/Views/Shared/_LayoutInstructor.cshtml";
    }
    if (this.User.IsInRole("ADMIN") || !this.User.Identity.IsAuthenticated)
    {
        Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    }
    if (this.User.IsInRole("PENDINGUSER") || !this.User.Identity.IsAuthenticated)
    {
        Layout = "~/Views/Shared/_LayoutPendingUser.cshtml";
    }
    if (!this.User.Identity.IsAuthenticated)
    {
        Layout = "~/Views/Shared/_Layout.cshtml";
    }


#line default
#line hidden
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
