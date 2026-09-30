using MudBlazor;

namespace Cinema.Web;

public static class AppTheme
{
    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#323776",
            Secondary = "#646898",
            AppbarBackground = "#101554",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#101554",
            Background = "#F4F4F8",
            Surface = "#FFFFFF",
            TextPrimary = "#101554",
            TextSecondary = "#646898"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#4C56A8",
            Secondary = "#7B81B5",
            AppbarBackground = "#1A1D3D",
            AppbarText = "#E8E9F5",
            DrawerBackground = "#26272E",
            DrawerText = "#C7C9DA",
            Background = "#1E1E1E",
            Surface = "#333333",
            TextPrimary = "#E5E5F0",
            TextSecondary = "#9A9CB5"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px"
        }
    };
}