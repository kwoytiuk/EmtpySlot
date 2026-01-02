using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(ProviderId), nameof(ProviderId))]
public partial class ServiceManagementViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string providerId = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Service> services = new();

    [ObservableProperty]
    private ObservableCollection<ServiceCategory> categories = new();

    public ServiceManagementViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Service Management";
    }

    [RelayCommand]
    async Task LoadServices()
    {
        if (string.IsNullOrEmpty(ProviderId)) return;

        try
        {
            IsBusy = true;

            // Load services
            var serviceList = await _apiService.GetProviderServicesAsync(Guid.Parse(ProviderId));
            Services.Clear();
            foreach (var service in serviceList)
            {
                Services.Add(service);
            }

            // Load categories if not already loaded
            if (Categories.Count == 0)
            {
                var categoryList = await _apiService.GetCategoriesAsync();
                foreach (var category in categoryList)
                {
                    Categories.Add(category);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load services: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddService()
    {
        await Shell.Current.GoToAsync($"AddServicePage?ProviderId={ProviderId}");
    }

    [RelayCommand]
    async Task DeleteService(Service service)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete {service.Name}?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _apiService.DeleteServiceAsync(service.Id);
            Services.Remove(service);

            await Shell.Current.DisplayAlert("Success", "Service deleted", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete service: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
