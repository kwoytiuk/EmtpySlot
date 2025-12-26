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
    private ObservableCollection<string> availableTimeSlots = new();

    [ObservableProperty]
    private string? selectedTimeSlot;

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
            GenerateTimeSlots();
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
    void ServiceSelected()
    {
        GenerateTimeSlots();
    }

    [RelayCommand]
    void SelectTimeSlot(string timeSlot)
    {
        SelectedTimeSlot = timeSlot;
    }

    public void OnDateChanged()
    {
        GenerateTimeSlots();
    }

    private void GenerateTimeSlots()
    {
        AvailableTimeSlots.Clear();

        // Generate time slots from 9 AM to 6 PM in 30-minute intervals
        var startHour = 9;
        var endHour = 18;

        for (int hour = startHour; hour < endHour; hour++)
        {
            for (int minute = 0; minute < 60; minute += 30)
            {
                var time = new TimeSpan(hour, minute, 0);
                AvailableTimeSlots.Add($"{hour:D2}:{minute:D2}");
            }
        }

        // Auto-select first slot if none selected
        if (AvailableTimeSlots.Count > 0 && string.IsNullOrEmpty(SelectedTimeSlot))
        {
            SelectedTimeSlot = AvailableTimeSlots[0];
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

        if (string.IsNullOrEmpty(SelectedTimeSlot))
        {
            await Shell.Current.DisplayAlert("Error", "Please select a time slot", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // Parse the selected time slot
            var timeParts = SelectedTimeSlot.Split(':');
            var startTime = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            var endTime = startTime.Add(TimeSpan.FromMinutes(SelectedService.DurationMinutes));

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

            await Shell.Current.DisplayAlert(
                "Success",
                $"Appointment booked for {SelectedDate:MMM dd, yyyy} at {SelectedTimeSlot}!",
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
