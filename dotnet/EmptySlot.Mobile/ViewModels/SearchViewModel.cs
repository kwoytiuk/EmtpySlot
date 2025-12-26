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

    [ObservableProperty]
    private string searchCity = "Seattle";

    [ObservableProperty]
    private ObservableCollection<string> radiusOptions = new() { "5 km", "10 km", "25 km", "50 km", "100 km" };

    [ObservableProperty]
    private string selectedRadius = "25 km";

    [ObservableProperty]
    private bool verifiedOnly = true;

    [ObservableProperty]
    private bool featuredOnly = false;

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

            // Parse radius from selected option (e.g., "25 km" -> 25)
            var radiusKm = double.TryParse(SelectedRadius.Split(' ')[0], out var radius) ? (double?)radius : null;

            var request = new SearchProvidersRequest(
                CategoryId: SelectedCategory?.Id,
                Latitude: null, // TODO: Add geocoding for city search or use device location
                Longitude: null,
                RadiusKm: radiusKm,
                MinRating: null,
                Verified: VerifiedOnly ? true : null
            );

            var results = await _apiService.SearchProvidersAsync(request);

            // Apply featured filter locally since it's not in the API
            if (FeaturedOnly)
            {
                results = results.Where(p => p.IsFeatured).ToList();
            }

            Providers.Clear();
            foreach (var provider in results)
            {
                Providers.Add(provider);
            }

            if (Providers.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No providers found with current filters");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
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
