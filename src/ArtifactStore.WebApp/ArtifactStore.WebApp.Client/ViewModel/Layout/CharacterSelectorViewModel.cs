using System.Net;
using System.Net.Http.Json;
using ArtifactStore.WebApp.Shared.Models.Character;
using ArtifactStore.WebApp.Shared.Responses;
using ArtifactStore.WebApp.Shared.Routes;
using Blazing.Mvvm.ComponentModel;

namespace ArtifactStore.WebApp.Client.ViewModel.Layout;

[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed class CharacterSelectorViewModel : ViewModelBase
{
    HttpClient _httpClient;
    IReadOnlyList<CharacterOption> _characters = new List<CharacterOption>();
    
    public int Page { get; private set; } = 1;
    public int PageSize { get; } = 3;
    public int TotalItems { get; private set; }
    public Guid? SelectedCharacterId { get; set; }
    public string NewCharacterName { get; set; } = "";
    public string? StatusMessage { get; private set; }

    public IReadOnlyList<CharacterOption> CurrentPageCharacters { get; private set; } = [];

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
    public bool CanGoPrevious => Page > 1;
    public bool CanGoNext => Page < TotalPages;

    public CharacterOption? SelectedCharacter =>
        _characters.FirstOrDefault(character => character.Id == SelectedCharacterId);

    public CharacterSelectorViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoadPageAsync()
    {
        var response = await _httpClient.GetAsync(CharacterRoute.GetUserAll);
        
        if (!response.IsSuccessStatusCode)
        {
            _characters = [];
            CurrentPageCharacters = Array.Empty<CharacterOption>();
            TotalItems = 0;
            SelectedCharacterId = null;
            
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                try
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    StatusMessage = error?.Error ?? "Character load failed.";
                }
                catch (Exception e)
                {
                    StatusMessage = "Character load failed with unknown error.";
                }
            }
        }
        else
        {
            var characters = await response.Content
                .ReadFromJsonAsync<IEnumerable<CharacterOption>>();

            _characters = characters?.ToList() ?? [];
            TotalItems = _characters.Count;
            Page = Math.Min(Page, TotalPages);
            UpdateCurrentPageCharacters();
            StatusMessage = null;
            await SelectFirstCurrentPageCharacterIfMissingAsync();
        }
    }

    public async Task RefreshAsync()
    {
        await LoadPageAsync();
    }

    public async Task SelectCharacterAsync(Guid characterId)
    {
        SelectedCharacterId = characterId;

        var response = await _httpClient.PostAsJsonAsync(CharacterRoute.Choose, new
        {
            characterId
        });

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Character select failed.";
            }
            catch (Exception)
            {
                StatusMessage = "Character select failed with unknown error.";
            }

            return;
        }

        StatusMessage = null;
    }

    public async Task AddCharacterAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCharacterName))
        {
            StatusMessage = "Enter character name.";
            return;
        }

        var response = await _httpClient.PostAsJsonAsync(CharacterRoute.Add, new
        {
            name = NewCharacterName.Trim()
        });

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Character add failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Character add failed with unknown error.";
            }

            return;
        }

        NewCharacterName = "";
        await LoadPageAsync();
    }

    public async Task DeleteSelectedCharacterAsync()
    {
        if (SelectedCharacterId is null)
        {
            StatusMessage = "Select character first.";
            return;
        }

        var response = await _httpClient.DeleteAsync(CharacterRoute.DeleteUrl(SelectedCharacterId.Value));

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Character delete failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Character delete failed with unknown error.";
            }

            return;
        }

        await LoadPageAsync();
    }

    public async Task PreviousPage()
    {
        if (!CanGoPrevious)
        {
            return;
        }

        Page--;
        UpdateCurrentPageCharacters();
        await SelectFirstCurrentPageCharacterIfMissingAsync();
    }

    public async Task NextPage()
    {
        if (!CanGoNext)
        {
            return;
        }

        Page++;
        UpdateCurrentPageCharacters();
        await SelectFirstCurrentPageCharacterIfMissingAsync();
    }

    private void UpdateCurrentPageCharacters()
    {
        CurrentPageCharacters = _characters
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToArray();
    }

    private async Task SelectFirstCurrentPageCharacterIfMissingAsync()
    {
        if (SelectedCharacterId is not null &&
            CurrentPageCharacters.Any(character => character.Id == SelectedCharacterId))
        {
            return;
        }

        var firstCharacter = CurrentPageCharacters.FirstOrDefault();
        if (firstCharacter is null)
        {
            SelectedCharacterId = null;
            return;
        }

        await SelectCharacterAsync(firstCharacter.Id);
    }
}
