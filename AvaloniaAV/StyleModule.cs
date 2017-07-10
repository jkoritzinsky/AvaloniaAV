using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Data;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaAV;

[assembly:ExportAvaloniaModule("AvaloniaAVStyles", typeof(StyleModule))]

namespace Avalonia
{
    public static partial class AppBuilderExtensions
    {
        public static TAppBuilder UseAvaloniaAVStyles<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup(_ => new StyleModule());
            return builder;
        }
    }
}


namespace AvaloniaAV
{
    public class StyleModule
    {
        public StyleModule()
        {
            AvaloniaLocator.Current.GetService<IGlobalStyles>().Styles.AddRange(new Generic());
            AvaloniaLocator.Current.GetService<IGlobalDataTemplates>()
                .DataTemplates.Add(new FuncDataTemplate<SystemCamera>(
                    camera => new TextBlock
                    {
                        [TextBox.TextProperty] = new Binding(nameof(SystemCamera.FriendlyName))
                    }));
        }
    }
}
