using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace WA.Services;

public class LayoutStateService
{
    public bool ShowTopBar { get; private set; } = true;
    public bool ShowFooter { get; private set; } = true;
    public bool ShowFab { get; private set; } = false;
    public bool EnableSnackbar { get; private set; } = true;

    public CultureInfo Language { get; set; } = CultureInfo.CurrentCulture;

    public event Action? OnChange;

    public void SetTopBar(bool value) { ShowTopBar = value; Notify(); }
    public void SetFooter(bool value) { ShowFooter = value; Notify(); }
    public void SetFab(bool value) { ShowFab = value; Notify(); }
    public void SetSnackbar(bool value) { EnableSnackbar = value; Notify(); }

    private void Notify() => OnChange?.Invoke();

    public static CultureInfo ChangeLanguage(ChangeEventArgs language)
    {
        var culture = new CultureInfo(language.Value!.ToString()!);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        return culture;
    }
}