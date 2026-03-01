using MudBlazor;

namespace WA.Theme;

public static class AppTheme
{
    public static MudTheme Light = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2B2B2B",              // Anthracite
            PrimaryContrastText = "#FFFFFF", // White

            Secondary = "#D4AF37",            // Muted gold
            SecondaryContrastText = "#1A1A1A",// Near black

            Tertiary = "#5A4A7A",             // Muted royal purple
            TertiaryContrastText = "#FFFFFF", // White

            Background = "#F4F4F4",           // Light gray
            Surface = "#FFFFFF",              // White

            AppbarBackground = "#2B2B2B",     // Anthracite
            AppbarText = "#FFFFFF",           // White

            DrawerBackground = "#1F1F1F",     // Dark charcoal
            DrawerText = "#E0E0E0",           // Light gray
            DrawerIcon = "#D4AF37",           // Muted gold

            TextPrimary = "#1A1A1A",          // Near black
            TextSecondary = "#555555",        // Medium gray

            Divider = "#D6D6D6",              // Soft gray

            Success = "#2E7D32",              // Dark green
            Error = "#C62828",                // Dark red
            Warning = "#ED6C02",              // Dark orange
            Info = "#0288D1"                  // Strong blue
        }
    };

    public static MudTheme Dark = new MudTheme
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#D4AF37",              // Muted gold
            PrimaryContrastText = "#1A1A1A",  // Near black

            Secondary = "#5A4A7A",            // Muted royal purple
            SecondaryContrastText = "#FFFFFF",// White

            Tertiary = "#2B2B2B",             // Anthracite
            TertiaryContrastText = "#FFFFFF", // White

            Background = "#121212",           // Very dark gray
            Surface = "#1E1E1E",              // Dark gray

            AppbarBackground = "#1A1A1A",     // Near black
            AppbarText = "#D4AF37",           // Muted gold

            DrawerBackground = "#181818",     // Deep charcoal
            DrawerText = "#E0E0E0",           // Light gray
            DrawerIcon = "#D4AF37",           // Muted gold

            TextPrimary = "#FFFFFF",          // White
            TextSecondary = "#B0B0B0",        // Soft gray

            Divider = "#2C2C2C",              // Dark divider gray

            Success = "#66BB6A",              // Soft green
            Error = "#EF5350",                // Soft red
            Warning = "#FFA726",              // Soft orange
            Info = "#29B6F6"                  // Light blue
        }
    };
}