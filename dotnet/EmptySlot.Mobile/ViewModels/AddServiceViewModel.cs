using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(ProviderId), nameof(ProviderId))]
public partial class AddServiceViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string providerId = string.Empty;

    [ObservableProperty]
    private string serviceName = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private int durationMinutes = 30;

    [ObservableProperty]
    private decimal price = 0;

    [ObservableProperty]
    private ServiceCategory? selectedCategory;

    [ObservableProperty]
    private ObservableCollection<ServiceCategory> categories = new();

    public AddServiceViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Add Service";
    }

    [RelayCommand]
    async Task LoadCategories()
    {
        try
        {
            var categoryList = await _apiService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var category in categoryList)
            {
                Categories.Add(category);
            }

            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task SaveService()
    {
        if (string.IsNullOrWhiteSpace(ServiceName) || SelectedCategory == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please enter service name and select a category", "OK");
            return;
        }

        if (Price <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a valid price", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new CreateServiceRequest(
                ProviderId: Guid.Parse(ProviderId),
                CategoryId: SelectedCategory.Id,
                Name: ServiceName,
                Description: Description,
                DurationMinutes: DurationMinutes,
                Price: Price,
                DepositRequired: null,
                ImageUrl: null);

            await _apiService.CreateServiceAsync(request);

            await Shell.Current.DisplayAlert("Success", "Service added successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add service: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
