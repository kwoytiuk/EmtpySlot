using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Mobile.Pages;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

public partial class SearchViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Provider> providers = new();

    [ObservableProperty]
    private ObservableCollection<ServiceCategory> categories = new();

    [ObservableProperty]
    private ServiceCategory? selectedCategory;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    public SearchViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Find Services";
    }

    [RelayCommand]
    async Task LoadCategories()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Loading categories from API: {DateTime.Now}");
            var cats = await _apiService.GetCategoriesAsync();
            System.Diagnostics.Debug.WriteLine($"Received {cats.Count} categories");
            Categories.Clear();
            foreach (var cat in cats)
            {
                Categories.Add(cat);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadCategories error: {ex}");
            await Shell.Current.DisplayAlert("Connection Error",
                $"Failed to load categories.\n\nError: {ex.Message}\n\nAPI: http://10.0.2.2:5000/api",
                "OK");
        }
    }

    [RelayCommand]
    async Task Search()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var request = new SearchProvidersRequest(
                CategoryId: SelectedCategory?.Id,
                Verified: true
            );

            var results = await _apiService.SearchProvidersAsync(request);
            Providers.Clear();
            foreach (var provider in results)
            {
                Providers.Add(provider);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SelectProvider(Provider provider)
    {
        if (provider == null) return;

        await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={provider.Id}");
    }
}
