using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(StaffId), nameof(StaffId))]
[QueryProperty(nameof(StaffName), nameof(StaffName))]
public partial class ScheduleManagementViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string staffId = string.Empty;

    [ObservableProperty]
    private string staffName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<StaffSchedule> schedules = new();

    [ObservableProperty]
    private DayOfWeek selectedDay = DayOfWeek.Monday;

    [ObservableProperty]
    private TimeSpan startTime = new TimeSpan(9, 0, 0);

    [ObservableProperty]
    private TimeSpan endTime = new TimeSpan(17, 0, 0);

    public ScheduleManagementViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Schedule Management";
    }

    [RelayCommand]
    async Task LoadSchedules()
    {
        if (string.IsNullOrEmpty(StaffId)) return;

        try
        {
            IsBusy = true;
            var scheduleList = await _apiService.GetStaffSchedulesAsync(Guid.Parse(StaffId));

            Schedules.Clear();
            foreach (var schedule in scheduleList)
            {
                Schedules.Add(schedule);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load schedules: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddSchedule()
    {
        try
        {
            IsBusy = true;

            var request = new CreateScheduleRequest(
                DayOfWeek: SelectedDay,
                StartTime: TimeOnly.FromTimeSpan(StartTime),
                EndTime: TimeOnly.FromTimeSpan(EndTime));

            await _apiService.CreateScheduleAsync(Guid.Parse(StaffId), request);
            await LoadSchedulesCommand.ExecuteAsync(null);

            await Shell.Current.DisplayAlert("Success", "Schedule added successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add schedule: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DeleteSchedule(StaffSchedule schedule)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Remove {schedule.DayOfWeek} schedule?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _apiService.DeleteScheduleAsync(schedule.Id);
            Schedules.Remove(schedule);

            await Shell.Current.DisplayAlert("Success", "Schedule deleted", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete schedule: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GenerateSlots()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Generate Time Slots",
            "Generate time slots for the next 14 days based on this staff member's schedules?",
            "Generate",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            await _apiService.GenerateTimeSlotsAsync(
                Guid.Parse(StaffId),
                DateTime.Today,
                DateTime.Today.AddDays(14));

            await Shell.Current.DisplayAlert("Success", "Time slots generated for the next 14 days!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to generate time slots: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
