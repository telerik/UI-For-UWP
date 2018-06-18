# Progress Telerik UI for UWP

This is an open-source version of Telerik UI for Universal Windows Platform (UWP) by Progress. The project is community-supported on [Stack Overflow](https://stackoverflow.com/questions/tagged/telerik+uwp). Commercial support is available at [telerik.com/uwp](http://www.telerik.com/uwp) where you'll find a supported commercial trial and pricing options. Please visit [telerik.com](http://www.telerik.com/) for UI tools for ASP.NET, WPF, WinForms, or JavaScript.

![UI for UWP](http://d585tldpucybw.cloudfront.net/sfimages/default-source/default-album/oss/uwp-ui.png?sfvrsn=2)

## Build status
| Target | Branch | Status | Recommended Nuget packages version |
| ------ | ------ | ------ | ------ |
| Production | master | [![Build status](https://ci.appveyor.com/api/projects/status/gjb70e1valw4d6vn/branch/master?svg=true)](https://ci.appveyor.com/project/UIForUWP/ui-for-uwp/branch/master) | [![NuGet](https://img.shields.io/nuget/v/Telerik.UI.for.UniversalWindowsPlatform.svg)](https://www.nuget.org/packages/Telerik.UI.for.UniversalWindowsPlatform/) |
| Pre-release beta testing | dev | [![Build status](https://ci.appveyor.com/api/projects/status/in6mu8pxvufh0x3m/branch/development?svg=true)](https://ci.appveyor.com/project/UIForUWP/ui-for-uwp-35if0/branch/development)| - |


## Available UI Components and Documentation for UI for UWP

UI for UWP is built to target UWP Windows 10 application development and deliver a unique experience based on the [Microsoft UX guidelines for Windows Runtime apps](https://developer.microsoft.com/windows/apps/design). It consists of the following components:

* [RadAutoCompleteBox](http://www.telerik.com/universal-windows-platform-ui/autocompletebox)
* [RadBulletGraph](http://www.telerik.com/universal-windows-platform-ui/bulletgraph)
* [RadBusyIndicator](http://www.telerik.com/universal-windows-platform-ui/busyindicator)
* [RadCalendar](http://www.telerik.com/universal-windows-platform-ui/calendar)
* [RadChart](http://www.telerik.com/universal-windows-platform-ui/chart)
* [RadDataBoundListBox](http://www.telerik.com/universal-windows-platform-ui/databoundlistbox)
* [RadDataForm](http://www.telerik.com/universal-windows-platform-ui/dataform)
* [RadDataGrid](http://www.telerik.com/universal-windows-platform-ui/grid)
* [RadDatePicker and RadTimePicker](http://www.telerik.com/universal-windows-platform-ui/datepicker-and-timepicker)
* [RadExpander](http://www.telerik.com/universal-windows-platform-ui/expander)
* [RadGauge](http://www.telerik.com/universal-windows-platform-ui/gauge)
* [RadHexView](http://www.telerik.com/universal-windows-platform-ui/hexview)
* [RadHubTile](http://www.telerik.com/universal-windows-platform-ui/hubtile)
* [RadListView](http://www.telerik.com/universal-windows-platform-ui/listview)
* [RadLoopingList](http://www.telerik.com/universal-windows-platform-ui/loopinglist)
* [RadMap](http://www.telerik.com/universal-windows-platform-ui/map)
* [RadNumericBox](http://www.telerik.com/universal-windows-platform-ui/numericbox)
* [RadPagination](http://www.telerik.com/universal-windows-platform-ui/pagination)
* [RadRadialMenu](http://www.telerik.com/universal-windows-platform-ui/radialmenu)
* [RadRangeSlider](http://www.telerik.com/universal-windows-platform-ui/rangeslider)
* [RadRating](http://www.telerik.com/universal-windows-platform-ui/rating)
* [RadSideDrawer](http://www.telerik.com/universal-windows-platform-ui/sidedrawer)

## Documentation

The official documentation for UI for UWP is available [here](http://docs.telerik.com/devtools/universal-windows-platform/Introduction-uwp) and here is the git repo for it [Telerik UI for UWP documentaton repo](https://github.com/telerik/uwp-docs)

## Demos

The [UI for UWP Demos application](https://github.com/telerik/UI-For-UWP-Demos) demonstrates a great number of user case scenarios using Telerik UI for UWP.

Also, you can take a look at the [Customers Orders Database sample](https://github.com/Microsoft/Windows-appsample-customers-orders-database) developed by Microsoft, which showcases the usage of the RadDataGrid control.

**********************************************************************************************************************************

## Getting started

* Make sure you have the [required software to build UWP applications](https://docs.microsoft.com/windows/uwp/get-started/get-set-up)
* Clone a copy of the repository code
* Open UWPControls.sln located in Controls folder and build it
* Open UWPMap.sln located in Controls folder and build it in x86/x64 and ARM configs
* All binaries are now located in Binaries folder
* You can additionaly build a NuGet package by running BuildTools/BuildNuGet.bat. The generated package will be located in the NuGet subfolder.

## Downloads

If you want to skip building the project yourself, you can get [the prebuilt nuget package](https://www.nuget.org/packages/Telerik.UI.for.UniversalWindowsPlatform/).

## How to Contribute

UI for UWP is free and open-source. We encourage and support an active, healthy community that accepts contributions from the public. We'd like you to be a part of that community.

Before contributing to UI for UWP, please:

1. Read and sign the [Telerik UI for UWP Contribution License Agreement](https://docs.google.com/forms/d/e/1FAIpQLSfQAzVxnnfwRQmtJCVmB41_ig1gYow--Gr8qLvaDxJRNHPtUQ/viewform), to confirm you've read and acknowledged the legal aspects of your contributions, and
2. Read our [contribution guide](CONTRIBUTING.md), which houses all of the necessary info to:
  * submit bugs,
  * request new features, and,
  * walk you through the entire process of preparing your code for a Pull Request.
  
## Getting Help

* Use the [issues list](https://github.com/telerik/UI-For-UWP/issues) of this repo for bug reports.
* Get help on [Stack Overflow](https://stackoverflow.com/questions/tagged/telerik+uwp) or the using the [commercial Telerik UI for UWP support channel](http://www.telerik.com/account/support-tickets/my-support-tickets.aspx).

As a fully-open source project, UI for UWP is a primarily community-supported project, as such, you are encouraged to use forums like [Stack Overflow](https://stackoverflow.com/) to post questions, and the issues list of this repo to report bugs.

The UI for UWP team does not provide formal support, except to those customers who have purchased a [commercial license for UI for UWP](http://www.telerik.com/universal-windows-platform-ui). Please do not create support requests for this project in the issues list for this repo, as these will be immediately closed. You'll be directed to post your question on a community forum.

## License

Licensed under the Apache License, Version 2.0. Please refer to [LICENSE.md](LICENSE.md) for more information.

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).

## Recent news

* [Blog: Getting Started with Telerik UI for UWP](http://www.telerik.com/blogs/getting-started-with-telerik-ui-for-uwp)
* [Video on CH9: Getting Started with Telerik UI for UWP](https://channel9.msdn.com/Blogs/vsppstories/Getting-Started-with-Telerik-UI-for-UWP)
* [Blog: Telerik UI for UWP joins the .NET Foundation](http://www.telerik.com/blogs/progress-telerik-ui-for-uwp-joins-net-foundation)
* [Blog: Telerik UI for UWP in Windows Template Studio](https://developer.telerik.com/topics/net/announcing-windows-template-studio/)
* [Blog: Telerik UI for UWP toolbox support with NuGet package](http://www.telerik.com/blogs/telerik-ui-for-uwp-free-and-updated-xaml-controls) ([alternative link](https://blogs.msdn.microsoft.com/visualstudio/2017/05/30/telerik-ui-for-uwp-free-and-updated-xaml-controls/))
* [Video: Building Windows Apps with Adaptive UI om Channel9](http://www.telerik.com/blogs/ui-controls-for-uwp-building-windows-apps-with-adaptive-ui) ([alternative link](https://channel9.msdn.com/Blogs/DevRadio/DR1734))
* [Blog: Analyzing NEOs with Telerik UI for UWP](https://www.telerik.com/blogs/analyzing-neos-with-telerik-ui-for-uwp)

## Like what you see?

If you like what you see, [tweet us please](https://twitter.com/intent/tweet?text=Woop%20woop%21%20I%20just%20got%20%23Telerik%20UI%20controls%20for%20%23UWP%20for%20free%20here%20https%3A%2F%2Fgithub.com%2Ftelerik%2FUI-For-UWP%20%40Telerik%20%40windev)




