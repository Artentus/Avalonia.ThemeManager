// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveUI;

namespace Avalonia.ThemeManager
{
    public sealed class ThemeSelector : ReactiveObject, IThemeSelector
    {
        ITheme? _selectedTheme;
        readonly IList<ITheme> _themes;
        readonly IList<Window> _windows;

        public ITheme? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value != _selectedTheme)
                {
                    if (!(_selectedTheme is null))
                    {
                        foreach (var window in _windows)
                            window.Styles.Remove(_selectedTheme.Style);
                    }

                    _selectedTheme = value;
                    this.RaisePropertyChanged(nameof(SelectedTheme));

                    if (!(_selectedTheme is null))
                    {
                        foreach (var window in _windows)
                            window.Styles.Add(_selectedTheme.Style);
                    }
                }
            }
        }

        public IReadOnlyList<Window> Windows { get; }

        public ReactiveCommand<string, bool> SelectThemeCommand { get; }

        public int Count => _themes.Count;

        public bool IsReadOnly => false;

        public ThemeSelector(IEnumerable<ITheme> themes)
        {
            _themes = new List<ITheme>(themes);
            _windows = new List<Window>();
            Windows = new ReadOnlyCollection<Window>(_windows);
            SelectThemeCommand = ReactiveCommand.Create<string, bool>(SelectTheme);
        }

        public ThemeSelector(params ITheme[] themes)
            : this((IEnumerable<ITheme>)themes)
        { }

        public void Add(ITheme theme) => _themes.Add(theme);

        public void AddRange(IEnumerable<ITheme> themes)
        {
            foreach (var theme in themes)
                Add(theme);
        }

        public void AddRange(params ITheme[] themes)
        {
            foreach (var theme in themes)
                Add(theme);
        }

        public bool Remove(ITheme theme)
        {
            if (theme == SelectedTheme)
                SelectedTheme = null;

            return _themes.Remove(theme);
        }

        public void Clear()
        {
            _themes.Clear();
            SelectedTheme = null;
        }

        public bool Contains(ITheme item) => _themes.Contains(item);
        void ICollection<ITheme>.CopyTo(ITheme[] array, int arrayIndex) => _themes.CopyTo(array, arrayIndex);
        public IEnumerator<ITheme> GetEnumerator() => _themes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _themes.GetEnumerator();

        public void EnableThemes(Window window)
        {
            if (window is null)
                throw new ArgumentNullException(nameof(window));

            if (_windows.Contains(window)) return;

            _windows.Add(window);
            if (!(SelectedTheme is null))
                window.Styles.Add(SelectedTheme.Style);
        }

        public bool DisableThemes(Window window)
        {
            if (window is null) return false;

            bool result = _windows.Remove(window);
            if (result && !(SelectedTheme is null))
                window.Styles.Remove(SelectedTheme.Style);
            return result;
        }

        public bool SelectTheme(string name)
        {
            var candidates = _themes.Where(t => t.Name == name);
            if (candidates.Any())
            {
                var theme = candidates.First();
                SelectedTheme = theme;
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool TryLoad(DirectoryInfo directory, out IThemeSelector? selector)
        {
            selector = default;
            if ((directory is null) || !directory.Exists) return false;

            var themes = new List<ITheme>();
            foreach (var file in directory.EnumerateFiles("*.xaml"))
            {
                if (Theme.TryLoad(file, out var theme) && !(theme is null))
                    themes.Add(theme);
            }

            selector = new ThemeSelector(themes);
            return true;
        }

        public static bool TryLoad(string directoryPath, out IThemeSelector? selector)
            => TryLoad(new DirectoryInfo(directoryPath), out selector);

        public static IThemeSelector Load(DirectoryInfo directory)
        {
            if (directory is null)
                throw new ArgumentNullException(nameof(directory));

            var themes = new List<ITheme>();
            foreach (var file in directory.EnumerateFiles("*.xaml"))
            {
                if (Theme.TryLoad(file, out var theme) && !(theme is null))
                    themes.Add(theme);
            }

            return new ThemeSelector(themes);
        }

        public static IThemeSelector Load(string directoryPath)
            => Load(new DirectoryInfo(directoryPath));

        public static IThemeSelector LoadSafe(DirectoryInfo directory)
        {
            if (!TryLoad(directory, out var selector) || (selector is null))
                selector = new ThemeSelector(Theme.DefaultLight, Theme.DefaultDark);
            return selector;
        }

        public static IThemeSelector LoadSafe(string directoryPath)
            => LoadSafe(new DirectoryInfo(directoryPath));
    }
}
