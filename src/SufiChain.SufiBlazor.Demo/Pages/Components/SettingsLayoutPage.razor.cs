using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Demo.Pages.Components;

public partial class SettingsLayoutPage : ComponentBase
{
    private string _emailSaved = "";
    private string _emailDraft = "";
    private string _identitySaved = "";
    private string _identityDraft = "";
    private string _notesSaved = "";
    private string _notesDraft = "";
    private string _singleSaved = "";
    private string _singleDraft = "";
    private bool _emailDirty;
    private bool _identityDirty;
    private bool _notesDirty;
    private bool _singleDirty;
    private bool _asyncPending = true;

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

    private string NotesDraft
    {
        get => _notesDraft;
        set
        {
            _notesDraft = value;
            _notesDirty = _notesDraft != _notesSaved;
        }
    }

    private string SingleDraft
    {
        get => _singleDraft;
        set
        {
            _singleDraft = value;
            _singleDirty = _singleDraft != _singleSaved;
        }
    }

    private Task<bool> SaveNotesAsync()
    {
        _notesSaved = _notesDraft;
        _notesDirty = false;
        return Task.FromResult(true);
    }

    private Task DiscardNotesAsync()
    {
        _notesDraft = _notesSaved;
        _notesDirty = false;
        return Task.CompletedTask;
    }

    private Task<bool> SaveSingleAsync()
    {
        _singleSaved = _singleDraft;
        _singleDirty = false;
        return Task.FromResult(true);
    }

    private Task DiscardSingleAsync()
    {
        _singleDraft = _singleSaved;
        _singleDirty = false;
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(400);
        _asyncPending = false;
    }
}
