using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(ProviderId), nameof(ProviderId))]
public partial class BookingViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string providerId = string.Empty;

    [ObservableProperty]
    private Provider? provider;

    [ObservableProperty]
    private Service? selectedService;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    public BookingViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Book Appointment";
    }

    [RelayCommand]
    async Task LoadProvider()
    {
        if (string.IsNullOrEmpty(ProviderId)) return;

        try
        {
            IsBusy = true;
            Provider = await _apiService.GetProviderAsync(Guid.Parse(ProviderId));
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
    async Task BookAppointment()
    {
        if (Provider == null || SelectedService == null) return;

        try
        {
            IsBusy = true;

            var dto = new CreateAppointmentDto(
                ProviderId: Provider.Id,
                LocationId: Provider.Locations.FirstOrDefault()?.Id ?? Guid.Empty,
                ServiceId: SelectedService.Id,
                StaffId: null,
                AppointmentDate: SelectedDate,
                StartTime: new TimeSpan(9, 0, 0),
                EndTime: new TimeSpan(9, SelectedService.DurationMinutes, 0),
                Price: SelectedService.Price,
                CustomerNotes: null
            );

            await _apiService.CreateAppointmentAsync(dto);
            await Shell.Current.DisplayAlert("Success", "Appointment booked!", "OK");
            await Shell.Current.GoToAsync("///AppointmentsPage");
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
}
