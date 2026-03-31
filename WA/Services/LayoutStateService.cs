using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace WA.Services;

/// <summary>
/// Service for managing layout-related UI state such as visibility of components
/// and application language settings.
/// </summary>
public class LayoutStateService
{
    /// <summary>
    /// Indicates whether the top bar is visible.
    /// </summary>
    public bool ShowTopBar { get; private set; } = true;

    /// <summary>
    /// Indicates whether the footer is visible.
    /// </summary>
    public bool ShowFooter { get; private set; } = true;

    /// <summary>
    /// Indicates whether the floating action button is visible.
    /// </summary>
    public bool ShowFab { get; private set; } = false;

    /// <summary>
    /// Indicates whether snackbar notifications are enabled.
    /// </summary>
    public bool EnableSnackbar { get; private set; } = true;

    /// <summary>
    /// Event triggered when layout state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Sets the visibility of the top bar.
    /// </summary>
    public void SetTopBar(bool value) { ShowTopBar = value; Notify(); }

    /// <summary>
    /// Sets the visibility of the footer.
    /// </summary>
    public void SetFooter(bool value) { ShowFooter = value; Notify(); }

    /// <summary>
    /// Sets the visibility of the floating action button.
    /// </summary>
    public void SetFab(bool value) { ShowFab = value; Notify(); }

    /// <summary>
    /// Enables or disables snackbar notifications.
    /// </summary>
    public void SetSnackbar(bool value) { EnableSnackbar = value; Notify(); }

    /// <summary>
    /// Notifies subscribers that the layout state has changed.
    /// </summary>
    private void Notify() => OnChange?.Invoke();
    
    private CultureInfo _language = CultureInfo.CurrentCulture;
    public CultureInfo Language
    {
        get => _language;
        private set { _language = value; Notify(); }
    }

    /// <summary>
    /// Changes the current application language based on a UI event.
    /// </summary>
    /// <param name="language">The change event containing the selected culture value.</param>
    /// <returns>The updated <see cref="CultureInfo"/>.</returns>
    public void ChangeLanguage(string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Language = culture;
    }
}