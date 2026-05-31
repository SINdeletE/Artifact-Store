using System.Net;
using System.Net.Http.Json;
using ArtifactStore.WebApp.Shared.Models;
using ArtifactStore.WebApp.Shared.Models.Inventory;
using ArtifactStore.WebApp.Shared.Responses;
using ArtifactStore.WebApp.Shared.Routes;
using Blazing.Mvvm.ComponentModel;

namespace ArtifactStore.WebApp.Client.ViewModel.Inventory;

[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed class InventoryViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;

    public int Page { get; set; } = 1;
    public int PageSize { get; } = 8;
    public int TotalItems { get; private set; }
    public bool IsLoading { get; private set; }
    public string? StatusMessage { get; private set; }
    public IReadOnlyList<InventoryItem> Items { get; private set; } = [];
    public InventoryItem? SelectedItem { get; private set; }

    public int TotalPages => Math.Max(Page, Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize)));
    public bool CanGoPrevious => Page > 1;
    public bool CanGoNext => Page < int.MaxValue;

    public InventoryViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoadAsync()
    {
        Page = Math.Max(1, Page);
        await LoadPageAsync(Page);
    }

    public async Task LoadPageAsync(int page)
    {
        page = Math.Max(1, page);
        Page = page;
        IsLoading = true;
        StatusMessage = null;

        try
        {
            var response = await _httpClient.GetAsync(InventoryRoute.GetPageUrl(page, PageSize));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var error = await ReadErrorAsync(response, "");
                StatusMessage = string.IsNullOrWhiteSpace(error) ? "Inventory was not found." : error;
                Items = [];
                TotalItems = 0;
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                Items = [];
                TotalItems = 0;
                
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

            var result = await response.Content.ReadFromJsonAsync<PageResponse<InventoryItem>>();
            Items = result?.Items?.ToArray() ?? [];
            TotalItems = result?.TotalItems ?? 0;
            Page = page;

            if (SelectedItem is not null && Items.All(item => item.Id != SelectedItem.Id))
            {
                SelectedItem = null;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task PreviousPageAsync()
    {
        if (CanGoPrevious)
        {
            await LoadPageAsync(Page - 1);
        }
    }

    public async Task NextPageAsync()
    {
        if (CanGoNext)
        {
            await LoadPageAsync(Page + 1);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(InventoryRoute.DeleteUrl(id));

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Inventory delete failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Inventory delete failed with unknown error.";
            }
        }

        if (SelectedItem?.Id == id)
        {
            SelectedItem = null;
        }

        var nextPage = Items.Count == 1 && Page > 1 ? Page - 1 : Page;
        await LoadPageAsync(nextPage);
    }

    public async Task ShowDetailsAsync(Guid id)
    {
        SelectedItem = null;
        StatusMessage = null;

        var response = await _httpClient.GetAsync(InventoryRoute.GetByIdUrl(id));

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Inventory item failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Inventory item failed with unknown error.";
            }
        }

        SelectedItem = await response.Content.ReadFromJsonAsync<InventoryItem>();
    }

    public void CloseDetails()
    {
        SelectedItem = null;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, string fallback)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return string.IsNullOrWhiteSpace(error?.Error) ? fallback : error.Error;
        }
        catch
        {
            return fallback;
        }
    }
}
