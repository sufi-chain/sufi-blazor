using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Demo.Pages.Components;

public partial class SettingsLayoutPage : ComponentBase
{
    private string _emailSaved = "";
    private string _emailDraft = "";
    private string _identitySaved = "";
    private string _identityDraft = "";
    private bool _emailDirty;
    private bool _identityDirty;

    private string EmailDraft
    {
        get => _emailDraft;
        set
        {
            _emailDraft = value;
            _emailDirty = _emailDraft != _emailSaved;
        }
    }

    private string IdentityDraft
    {
        get => _identityDraft;
        set
        {
            _identityDraft = value;
            _identityDirty = _identityDraft != _identitySaved;
        }
    }

    private Task<bool> SaveEmailAsync()
    {
        _emailSaved = _emailDraft;
        _emailDirty = false;
        return Task.FromResult(true);
    }

    private Task DiscardEmailAsync()
    {
        _emailDraft = _emailSaved;
        _emailDirty = false;
        return Task.CompletedTask;
    }

    private Task<bool> SaveIdentityAsync()
    {
        _identitySaved = _identityDraft;
        _identityDirty = false;
        return Task.FromResult(true);
    }

    private Task DiscardIdentityAsync()
    {
        _identityDraft = _identitySaved;
        _identityDirty = false;
        return Task.CompletedTask;
    }
}
