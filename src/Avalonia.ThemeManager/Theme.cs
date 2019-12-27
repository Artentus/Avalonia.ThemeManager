// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using ReactiveUI;

namespace Avalonia.ThemeManager
{
    public class Theme : ReactiveObject, ITheme
    {
        IThemeSelector? _selector;

        public string Name { get; }

        public IStyle? Style { get; }

        public IThemeSelector? Selector
        {
            get => _selector;
            set => this.RaiseAndSetIfChanged(ref _selector, value);
        }

        protected Theme(string name, IStyle style)
        {
            Name = name;
            Style = style;
        }


        static readonly IStyle _light;
        static readonly IStyle _dark;

        public static ITheme DefaultLight => new Theme("Light", _light);
        public static ITheme DefaultDark => new Theme("Dark", _dark);

        static Theme()
        {
            _light = new StyleInclude(new Uri("resm:Styles?assembly=Avalonia.ThemeManager"))
            {
                Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseLight.xaml?assembly=Avalonia.Themes.Default")
            };
            _dark = new StyleInclude(new Uri("resm:Styles?assembly=Avalonia.ThemeManager"))
            {
                Source = new Uri("resm:Avalonia.Themes.Default.Accents.BaseDark.xaml?assembly=Avalonia.Themes.Default")
            };
        }

        public static bool TryLoad(FileInfo file, out ITheme? theme)
        {
            theme = default;
            if ((file is null) || !file.Exists) return false;

            var name = Path.GetFileNameWithoutExtension(file.Name);
            var xaml = File.ReadAllText(file.FullName);

            IStyle style;
            try
            {
                style = AvaloniaXamlLoader.Parse<IStyle>(xaml);
            }
            catch
            {
                return false;
            }
            
            theme = new Theme(name, style);
            return true;
        }

        public static bool TryLoad(string filePath, out ITheme? theme)
            => TryLoad(new FileInfo(filePath), out theme);

        public static ITheme Load(FileInfo file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            var name = Path.GetFileNameWithoutExtension(file.Name);
            var xaml = File.ReadAllText(file.FullName);
            var style = AvaloniaXamlLoader.Parse<IStyle>(xaml);
            return new Theme(name, style);
        }

        public static ITheme Load(string filePath)
            => Load(new FileInfo(filePath));
    }
}
