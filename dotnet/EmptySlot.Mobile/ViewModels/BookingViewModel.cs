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

    [RelayCommand]
    void SelectService(Service service)
    {
        SelectedService = service;
        GenerateTimeSlots();
    }

    public void OnDateChanged()
    {
        GenerateTimeSlots();
    }

    private void GenerateTimeSlots()
    {
        AvailableTimeSlots.Clear();

        // Generate time slots starting from current time, rounded to next 30-min interval
        var now = DateTime.Now;

        // Round up to next 30-minute interval
        int startMinute = now.Minute < 30 ? 30 : 0;
        int startHour = now.Minute < 30 ? now.Hour : now.Hour + 1;

        // If it's past 8 PM, start from 5:45 PM (next day context)
        if (startHour >= 20)
        {
            startHour = 17;
            startMinute = 45;
        }

        var endHour = 21; // Extended to 9 PM for dinner reservations

        // Generate slots starting from the rounded time
        var currentHour = startHour;
        var currentMin = startMinute;

        while (currentHour < endHour)
        {
            var time = new TimeSpan(currentHour, currentMin, 0);
            var displayTime = DateTime.Today.Add(time).ToString("h:mm tt");
            AvailableTimeSlots.Add(displayTime);

            // Increment by 30 minutes
            currentMin += 30;
            if (currentMin >= 60)
            {
                currentMin = 0;
                currentHour++;
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

            // Parse the selected time slot (format: "h:mm tt")
            if (!DateTime.TryParse(SelectedTimeSlot, out var parsedTime))
            {
                await Shell.Current.DisplayAlert("Error", "Invalid time slot format", "OK");
                return;
            }

            var startTime = parsedTime.TimeOfDay;
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
