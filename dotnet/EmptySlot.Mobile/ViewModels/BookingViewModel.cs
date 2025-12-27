using System.Collections.ObjectModel;
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
    private StaffMember? selectedStaff;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<TimeSlotDto> availableTimeSlots = new();

    [ObservableProperty]
    private TimeSlotDto? selectedTimeSlot;

    [ObservableProperty]
    private string? customerNotes;

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
            await LoadTimeSlots();
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
    async Task ServiceSelected()
    {
        await LoadTimeSlots();
    }

    [RelayCommand]
    void SelectTimeSlot(TimeSlotDto timeSlot)
    {
        SelectedTimeSlot = timeSlot;
    }

    [RelayCommand]
    async Task SelectService(Service service)
    {
        SelectedService = service;
        await LoadTimeSlots();
    }

    public async void OnDateChanged()
    {
        await LoadTimeSlots();
    }

    private async Task LoadTimeSlots()
    {
        if (Provider == null) return;

        try
        {
            AvailableTimeSlots.Clear();

            // Fetch available time slots for the selected date
            var slots = await _apiService.GetAvailableTimeSlotsAsync(
                Provider.Id,
                SelectedDate,
                SelectedDate);

            foreach (var slot in slots)
            {
                AvailableTimeSlots.Add(slot);
            }

            // Auto-select first slot if none selected
            if (AvailableTimeSlots.Count > 0 && SelectedTimeSlot == null)
            {
                SelectedTimeSlot = AvailableTimeSlots[0];
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load time slots: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    async Task BookAppointment()
    {
        if (Provider == null || SelectedService == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a service", "OK");
            return;
        }

        if (SelectedTimeSlot == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a time slot", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // Use the actual time slot data
            var startTime = SelectedTimeSlot.StartTime.ToTimeSpan();
            var endTime = SelectedTimeSlot.EndTime.ToTimeSpan();

            var dto = new CreateAppointmentDto(
                ProviderId: Provider.Id,
                LocationId: Provider.Locations.FirstOrDefault()?.Id ?? Guid.Empty,
                ServiceId: SelectedService.Id,
                StaffId: SelectedStaff?.Id,
                AppointmentDate: SelectedDate,
                StartTime: startTime,
                EndTime: endTime,
                Price: SelectedService.Price,
                CustomerNotes: CustomerNotes
            );

            await _apiService.CreateAppointmentAsync(dto);

            // Block the time slot after successful booking
            await _apiService.BlockTimeSlotAsync(SelectedTimeSlot.Id);

            await Shell.Current.DisplayAlert(
                "Success",
                $"Appointment booked for {SelectedDate:MMM dd, yyyy} at {SelectedTimeSlot.DisplayTime} with {SelectedTimeSlot.StaffMemberName}!",
                "OK");

            await Shell.Current.GoToAsync("///AppointmentsPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to book appointment: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
