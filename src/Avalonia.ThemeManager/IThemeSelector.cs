// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Avalonia.Controls;
using ReactiveUI;

namespace Avalonia.ThemeManager
{
    public interface IThemeSelector : ICollection<ITheme>
    {
        ITheme? SelectedTheme { get; set; }
        IReadOnlyList<Window> Windows { get; }
        void EnableThemes(Window window);
        bool DisableThemes(Window window);
        bool SelectTheme(string name);
        ReactiveCommand<string, bool> SelectThemeCommand { get; }
    }
}
