using MudBlazor;

namespace WA.Theme;

public static class AppTheme
{
    public static MudTheme Light = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1976D2",
            Secondary = "#9C27B0",
            Background = "#FFFFFF",
            Surface = "#FFFFFF",
            AppbarBackground = "#1976D2",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#000000",
            TextPrimary = "#000000",
            TextSecondary = "#5F6368",
            Divider = "#E0E0E0",
            Success = "#4CAF50",
            Error = "#F44336",
            Warning = "#FF9800",
            Info = "#2196F3"
        }
    };

    public static MudTheme Dark = new MudTheme
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#1976D2",
            Secondary = "#9C27B0",
            Background = "#FFFFFF",
            Surface = "#FFFFFF",
            AppbarBackground = "#1976D2",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#000000",
            TextPrimary = "#000000",
            TextSecondary = "#5F6368",
            Divider = "#E0E0E0",
            Success = "#4CAF50",
            Error = "#F44336",
            Warning = "#FF9800",
            Info = "#2196F3"
        }
    };
}