using MudBlazor;

namespace WA.Theme;

public static class AppTheme
{
    public static MudTheme Light = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary = Colors.BlueGray.Default,
            Secondary = Colors.Teal.Accent4,
            Background = Colors.Gray.Lighten5
        }
    };

    public static MudTheme Dark = new MudTheme
    {
        PaletteDark = new PaletteDark
        {
            Primary = Colors.BlueGray.Lighten2,
            Secondary = Colors.Teal.Accent2,
            Background = Colors.Gray.Darken4,
            Surface = Colors.Gray.Darken3
        }
    };
}