using System.Net;
using System.Net.Http.Json;
using ArtifactStore.WebApp.Shared.Models;
using ArtifactStore.WebApp.Shared.Models.Balance;
using ArtifactStore.WebApp.Shared.Models.Store;
using ArtifactStore.WebApp.Shared.Responses;
using ArtifactStore.WebApp.Shared.Routes;
using Blazing.Mvvm.ComponentModel;

namespace ArtifactStore.WebApp.Client.ViewModel.Shop;

[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed class ShopViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;

    public int Page { get; set; } = 1;
    public int PageSize { get; } = 4;
    public int TotalItems { get; private set; }
    public bool IsLoading { get; private set; }
    public string? StatusMessage { get; private set; }
    public IReadOnlyList<StoreItem> Items { get; private set; } = [];
    public StoreItem? SelectedItem { get; private set; }
    public IReadOnlyList<CharacterCurrency> CharacterCurrencies { get; private set; } = [];

    public int TotalPages => Math.Max(Page, Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize)));
    public bool CanGoPrevious => Page > 1;
    public bool CanGoNext => Page < int.MaxValue;

    public ShopViewModel(HttpClient httpClient)
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
            var response = await _httpClient.GetAsync(StoreRoute.GetPageUrl(page, PageSize));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Items = [];
                TotalItems = 0;
                SelectedItem = null;
                StatusMessage = await ReadErrorAsync(response, "Store items were not found.");
                await LoadCharacterCurrenciesAsync();
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                Items = [];
                TotalItems = 0;
                SelectedItem = null;
                StatusMessage = await ReadErrorAsync(response, "Store item load failed.");
                await LoadCharacterCurrenciesAsync();
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<PageResponse<StoreItem>>();
            Items = result?.Items?.ToArray() ?? [];
            TotalItems = result?.TotalItems ?? 0;
            Page = page;

            await LoadCharacterCurrenciesAsync();

            if (SelectedItem is not null && Items.All(item => item.Id != SelectedItem.Id))
            {
                SelectedItem = null;
            }
        }
        catch
        {
            Items = [];
            TotalItems = 0;
            SelectedItem = null;
            CharacterCurrencies = [];
            StatusMessage = "Store item load failed.";
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

    public void SelectItem(Guid id)
    {
        SelectedItem = Items.FirstOrDefault(item => item.Id == id);
    }

    public void CloseSummary()
    {
        SelectedItem = null;
    }

    public static bool HasDiscount(StoreItem item)
    {
        return item.Discount is { Percent: > 0 };
    }

    public static decimal GetDisplayPrice(StoreItem item)
    {
        if (!HasDiscount(item))
        {
            return item.Price;
        }

        var discountPercent = Math.Clamp(item.Discount!.Percent, 0, 1);
        return Math.Round(item.Price * (1 - discountPercent), 2, MidpointRounding.AwayFromZero);
    }

    public static string FormatDiscountPercent(StoreItem item)
    {
        return HasDiscount(item) ? $"-{item.Discount!.Percent * 100:0.##}%" : string.Empty;
    }

    public static string FormatPrice(decimal price) => price.ToString("0.##");

    public static string FormatDiscountName(StoreItem item)
        => item.Discount?.Name ?? string.Empty;

    public static string FormatDiscountDescription(StoreItem item)
        => item.Discount?.Description ?? string.Empty;

    public static string FormatDiscountPeriod(StoreItem item)
        => item.Discount is null
            ? string.Empty
            : $"{item.Discount.StartsAt:dd.MM.yyyy} — {item.Discount.EndsAt:dd.MM.yyyy}";

    public async Task PurchaseSelectedAsync()
    {
        if (SelectedItem is null)
        {
            StatusMessage = "Select a store item first.";
            return;
        }

        var response = await _httpClient.PostAsJsonAsync(StoreRoute.Purchase, new
        {
            StoreItemId = SelectedItem.Id
        });

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Purchase failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Purchase failed with unknown error.";
            }

            return;
        }

        StatusMessage = "Purchase completed.";
        await LoadCharacterCurrenciesAsync();
        await LoadPageAsync(Page);
    }

    private async Task LoadCharacterCurrenciesAsync()
    {
        var response = await _httpClient.GetAsync(BalanceRoute.GetCharacterBalances);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            CharacterCurrencies = [];
            StatusMessage = await ReadErrorAsync(response, "Current character is not selected.");
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            CharacterCurrencies = [];
            StatusMessage = await ReadErrorAsync(response, "Character currencies load failed.");
            return;
        }

        var balances = await response.Content.ReadFromJsonAsync<IEnumerable<Balance>>();
        CharacterCurrencies = balances?
            .Select(balance => new CharacterCurrency
            {
                BalanceId = balance.Id,
                CurrencyId = balance.CurrencyId,
                Name = balance.Currency?.Name ?? balance.CurrencyId.ToString(),
                Amount = balance.Amount
            })
            .OrderBy(currency => currency.Name)
            .ToArray() ?? [];
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
