using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

public partial class EmployeeDashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string staffName = string.Empty;

    [ObservableProperty]
    private string businessName = string.Empty;

    [ObservableProperty]
    private Guid staffId;

    [ObservableProperty]
    private ObservableCollection<StaffSchedule> schedules = new();

    public EmployeeDashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Employee Dashboard";
    }

    [RelayCommand]
    async Task LoadDashboard()
    {
        var staffIdString = Preferences.Get("StaffId", string.Empty);
        if (string.IsNullOrEmpty(staffIdString))
        {
            await Shell.Current.DisplayAlert("Error", "Please log in first", "OK");
            await Shell.Current.GoToAsync("///EmployeeLoginPage");
            return;
        }

        StaffId = Guid.Parse(staffIdString);
        StaffName = Preferences.Get("StaffName", "");
        BusinessName = Preferences.Get("StaffBusinessName", "");

        await LoadSchedules();
    }

    async Task LoadSchedules()
    {
        try
        {
            IsBusy = true;
            var scheduleList = await _apiService.GetStaffSchedulesAsync(StaffId);

            Schedules.Clear();
            foreach (var schedule in scheduleList.OrderBy(s => s.DayOfWeek))
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
    async Task ManageSchedule()
    {
        await Shell.Current.GoToAsync($"ScheduleManagementPage?StaffId={StaffId}&StaffName={StaffName}");
    }

    [RelayCommand]
    async Task Logout()
    {
        Preferences.Remove("StaffId");
        Preferences.Remove("StaffName");
        Preferences.Remove("StaffProviderId");
        Preferences.Remove("StaffBusinessName");

        await Shell.Current.GoToAsync("///EmployeeLoginPage");
    }
}
