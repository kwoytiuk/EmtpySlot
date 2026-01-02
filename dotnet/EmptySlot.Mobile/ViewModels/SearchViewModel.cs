using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;
using EmptySlot.Mobile.Pages;
using EmptySlot.Mobile.Helpers;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

public partial class SearchViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    // Static cache to share search results with MapViewModel
    public static List<Provider>? LastSearchResults { get; private set; }

    [ObservableProperty]
    private ObservableCollection<Provider> providers = new();

    // Track available slot counts per provider
    private Dictionary<Guid, int> _providerSlotCounts = new();

    [ObservableProperty]
    private ObservableCollection<ServiceCategory> categories = new();

    [ObservableProperty]
    private ServiceCategory? selectedCategory;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private string searchCity = "Calgary";

    [ObservableProperty]
    private ObservableCollection<string> radiusOptions = new() { "5 km", "10 km", "25 km", "50 km", "100 km" };

    [ObservableProperty]
    private string selectedRadius = "50 km";

    [ObservableProperty]
    private bool verifiedOnly = false;

    [ObservableProperty]
    private bool featuredOnly = false;

    [ObservableProperty]
    private bool isDetectingLocation = false;

    [ObservableProperty]
    private double? currentLatitude;

    [ObservableProperty]
    private double? currentLongitude;

    // Dynamic time slots based on current time
    public List<string> AvailableTimeSlots => TimeSlotHelper.GetNext3TimeSlots();

    // Get slot count for a specific provider
    public int GetProviderSlotCount(Guid providerId)
    {
        return _providerSlotCounts.TryGetValue(providerId, out var count) ? count : 0;
    }

    public SearchViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Find Services";
    }

    [RelayCommand]
    async Task DetectLocation()
    {
        try
        {
            IsDetectingLocation = true;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    CurrentLatitude = location.Latitude;
                    CurrentLongitude = location.Longitude;

                    // Try to get city name from coordinates
                    SearchCity = await GetCityFromCoordinates(location.Latitude, location.Longitude);

                    await Shell.Current.DisplayAlert("Location Detected",
                        $"Searching near {SearchCity}",
                        "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Location Not Available",
                        "Could not detect your current location. Using default location.",
                        "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Permission Denied",
                    "Location permission is required to detect your location. You can still search by entering a city name.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error detecting location: {ex.Message}");
            await Shell.Current.DisplayAlert("Error",
                "Unable to detect location. Please enter a city manually.",
                "OK");
        }
        finally
        {
            IsDetectingLocation = false;
        }
    }

    private async Task<string> GetCityFromCoordinates(double latitude, double longitude)
    {
        try
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null && !string.IsNullOrEmpty(placemark.Locality))
            {
                return placemark.Locality;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geocoding error: {ex.Message}");
        }

        // Default to Calgary if geocoding fails
        return "Calgary";
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
                Latitude: CurrentLatitude,
                Longitude: CurrentLongitude,
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

            // Store results for MapViewModel to use
            LastSearchResults = results;

            Providers.Clear();
            foreach (var provider in results)
            {
                Providers.Add(provider);
            }

            if (Providers.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No providers found with current filters");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Search found {Providers.Count} providers. Stored for map view.");

                // Load available slot counts for today
                _ = LoadProviderSlotCountsAsync(); // Fire and forget - loads in background
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

    [RelayCommand]
    async Task QuickBook(Provider provider)
    {
        if (provider == null) return;

        // Show quick booking dialog with service selection
        var services = provider.Services.Take(3).Select(s => s.Name).ToArray();
        if (services.Length == 0)
        {
            await Shell.Current.DisplayAlert("No Services", "This provider doesn't have any services available.", "OK");
            return;
        }

        var selectedService = await Shell.Current.DisplayActionSheet(
            $"Book at {provider.BusinessName}",
            "Cancel",
            null,
            services
        );

        if (selectedService != null && selectedService != "Cancel")
        {
            // Navigate to booking page with pre-selected service
            await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={provider.Id}");
        }
    }

    [RelayCommand]
    async Task ShowMap()
    {
        // Navigate to interactive map page with current search parameters
        var queryParams = new Dictionary<string, object>
        {
            ["CategoryId"] = SelectedCategory?.Id.ToString() ?? "",
            ["City"] = SearchCity,
            ["Latitude"] = CurrentLatitude?.ToString() ?? "",
            ["Longitude"] = CurrentLongitude?.ToString() ?? "",
            ["Radius"] = SelectedRadius.Replace(" km", ""),
            ["VerifiedOnly"] = VerifiedOnly.ToString(),
            ["FeaturedOnly"] = FeaturedOnly.ToString()
        };

        await Shell.Current.GoToAsync(nameof(MapPage), queryParams);
    }

    [RelayCommand]
    async Task ShowFilters()
    {
        // Toggle filters visibility or show dialog
        var action = await Shell.Current.DisplayActionSheet(
            "Filter Options",
            "Cancel",
            null,
            VerifiedOnly ? "Hide Verified Only" : "Show Verified Only",
            FeaturedOnly ? "Hide Featured Only" : "Show Featured Only"
        );

        if (action == "Show Verified Only" || action == "Hide Verified Only")
        {
            VerifiedOnly = !VerifiedOnly;
            await Search();
        }
        else if (action == "Show Featured Only" || action == "Hide Featured Only")
        {
            FeaturedOnly = !FeaturedOnly;
            await Search();
        }
    }

    [RelayCommand]
    async Task ShowLocationFilter()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Change Location",
            "Enter city name:",
            initialValue: SearchCity,
            placeholder: "Calgary");

        if (!string.IsNullOrWhiteSpace(result))
        {
            SearchCity = result;
            await Search();
        }
    }

    [RelayCommand]
    async Task ShowDistanceFilter()
    {
        var result = await Shell.Current.DisplayActionSheet(
            "Search Radius",
            "Cancel",
            null,
            "5 km",
            "10 km",
            "25 km",
            "50 km",
            "100 km");

        if (result != null && result != "Cancel")
        {
            SelectedRadius = result;
            await Search();
        }
    }

    [RelayCommand]
    async Task ShowSort()
    {
        var result = await Shell.Current.DisplayActionSheet(
            "Sort By",
            "Cancel",
            null,
            "Distance",
            "Rating (High to Low)",
            "Rating (Low to High)",
            "Price (Low to High)",
            "Price (High to Low)",
            "Most Popular");

        if (result != null && result != "Cancel")
        {
            // TODO: Implement sorting logic
            await Shell.Current.DisplayAlert("Sort", $"Sorting by: {result}", "OK");
        }
    }

    private async Task LoadProviderSlotCountsAsync()
    {
        try
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Fetch slot counts for all providers in parallel
            var tasks = Providers.Select(async provider =>
            {
                try
                {
                    var slots = await _apiService.GetAvailableTimeSlotsAsync(
                        provider.Id,
                        today,
                        tomorrow);

                    // Count slots for today only
                    var todaySlots = slots.Where(s => s.Date == DateOnly.FromDateTime(today)).Count();
                    _providerSlotCounts[provider.Id] = todaySlots;

                    return (provider.Id, todaySlots);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading slots for {provider.BusinessName}: {ex.Message}");
                    _providerSlotCounts[provider.Id] = 0;
                    return (provider.Id, 0);
                }
            });

            await Task.WhenAll(tasks);

            // Notify UI to refresh
            OnPropertyChanged(nameof(Providers));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading slot counts: {ex.Message}");
        }
    }
}
