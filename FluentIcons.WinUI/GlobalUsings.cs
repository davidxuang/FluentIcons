global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Linq;
global using FluentIcons.Common;
global using FluentIcons.Common.Internals;
global using Symbol = FluentIcons.Common.Symbol;
#if WINDOWS_WINAPPSDK || HAS_UNO_WINUI
global using Microsoft.Extensions.Hosting;
global using Microsoft.UI.Text;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Markup;
global using Microsoft.UI.Xaml.Media;
global using FluentIcons.WinUI.Internals;
#else
global using Windows.UI.Text;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Data;
global using Windows.UI.Xaml.Markup;
global using Windows.UI.Xaml.Media;
global using FluentIcons.Uwp.Internals;
#endif
