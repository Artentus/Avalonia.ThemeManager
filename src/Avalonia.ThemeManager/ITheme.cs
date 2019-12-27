// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Styling;

namespace Avalonia.ThemeManager
{
    public interface ITheme
    {
        string Name { get; }
        IStyle? Style { get; }
    }
}
