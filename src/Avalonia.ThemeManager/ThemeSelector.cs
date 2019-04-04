﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using ReactiveUI;

namespace Avalonia.ThemeManager
{
    public class ThemeSelector : ReactiveObject
    {
        public static ThemeSelector Instance;

        private Theme _selectedTheme;
        private ObservableCollection<Theme> _themes;
        private ObservableCollection<Window> _windows;

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        }

        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            set => this.RaiseAndSetIfChanged(ref _themes, value);
        }

        public ObservableCollection<Window> Windows
        {
            get => _windows;
            set => this.RaiseAndSetIfChanged(ref _windows, value);
        }

        public ThemeSelector(string path)
        {
            _themes = new ObservableCollection<Theme>();

            try
            {
                foreach (string file in System.IO.Directory.EnumerateFiles(path, "*.xaml"))
                {
                    _themes.Add(LoadTheme(file));
                }
            }
            catch (Exception)
            {
            }

            if (_themes.Count == 0)
            {
                var light = AvaloniaXamlLoader.Parse<StyleInclude>(@"<StyleInclude xmlns='https://github.com/avaloniaui' Source='avares://Avalonia.Themes.Default/Accents/BaseLight.xaml'/>");
                var dark = AvaloniaXamlLoader.Parse<StyleInclude>(@"<StyleInclude xmlns='https://github.com/avaloniaui' Source='avares://Avalonia.Themes.Default/Accents/BaseDark.xaml'/>");
                _themes.Add(new Theme() { Name = "Light", Style = light, Selector = this });
                _themes.Add(new Theme() { Name = "Dark", Style = dark, Selector = this });
            }

            _selectedTheme = _themes.FirstOrDefault();

            _windows = new ObservableCollection<Window>();
        }

        public Theme LoadTheme(string file)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(file);
            var xaml = System.IO.File.ReadAllText(file);
            var style = AvaloniaXamlLoader.Parse<IStyle>(xaml);
            return new Theme() { Name = name, Style = style, Selector = this };
        }

        public void EnableThemes(Window window)
        {
            IDisposable disposable = null;

            window.Styles.Add(Instance.SelectedTheme.Style);

            window.Opened += (sender, e) =>
            {
                _windows.Add(window);
                disposable = this.WhenAnyValue(x => x.SelectedTheme).Where(x => x != null).Subscribe(x =>
                {
                    window.Styles[0] = x.Style;
                });
            };

            window.Closing += (sender, e) =>
            {
                disposable?.Dispose();
                _windows.Remove(window);
            };
        }

        public void ApplyTheme(Theme theme)
        {
            if (theme != null)
            {
                SelectedTheme = theme;
            }
        }

        public void LoadSelectedTheme(string file)
        {
            try
            {
                if (System.IO.File.Exists(file) == true)
                {
                    var name = System.IO.File.ReadAllText(file);
                    if (name != null)
                    {
                        var theme = _themes.FirstOrDefault(x => x.Name == name);
                        if (theme != null)
                        {
                            SelectedTheme = theme;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SaveSelectedTheme(string file)
        {
            try
            {
                System.IO.File.WriteAllText(file, _selectedTheme?.Name);
            }
            catch (Exception)
            {
            }
        }
    }
}
