using System;

namespace WA.Services;

public class LayoutStateService
{
    public bool ShowTopBar { get; private set; } = true;
    public bool ShowFooter { get; private set; } = true;
    public bool ShowFab { get; private set; } = false;
    public bool EnableSnackbar { get; private set; } = true;

    public event Action? OnChange;

    public void SetTopBar(bool value) { ShowTopBar = value; Notify(); }
    public void SetFooter(bool value) { ShowFooter = value; Notify(); }
    public void SetFab(bool value) { ShowFab = value; Notify(); }
    public void SetSnackbar(bool value) { EnableSnackbar = value; Notify(); }

    private void Notify() => OnChange?.Invoke();
}