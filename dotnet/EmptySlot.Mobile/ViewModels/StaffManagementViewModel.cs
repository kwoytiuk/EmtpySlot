using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(ProviderId), nameof(ProviderId))]
public partial class StaffManagementViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string providerId = string.Empty;

    [ObservableProperty]
    private ObservableCollection<StaffMember> staffMembers = new();

    [ObservableProperty]
    private StaffMember? selectedStaff;

    public StaffManagementViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Staff Management";
    }

    [RelayCommand]
    async Task LoadStaff()
    {
        if (string.IsNullOrEmpty(ProviderId)) return;

        try
        {
            IsBusy = true;
            var staff = await _apiService.GetProviderStaffAsync(Guid.Parse(ProviderId));

            StaffMembers.Clear();
            foreach (var member in staff)
            {
                StaffMembers.Add(member);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load staff: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddStaff()
    {
        var name = await Shell.Current.DisplayPromptAsync("Add Staff Member", "Enter staff member name:", placeholder: "Full Name");
        if (string.IsNullOrWhiteSpace(name)) return;

        var email = await Shell.Current.DisplayPromptAsync("Add Staff Member", "Enter email address:", placeholder: "email@example.com", keyboard: Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;

        var phone = await Shell.Current.DisplayPromptAsync("Add Staff Member", "Enter phone number:", placeholder: "+1 (403) 555-1234", keyboard: Keyboard.Telephone);

        try
        {
            IsBusy = true;

            var request = new CreateStaffRequest(
                ProviderId: Guid.Parse(ProviderId),
                Name: name,
                Email: email,
                Phone: phone,
                Bio: null);

            await _apiService.CreateStaffMemberAsync(request);
            await LoadStaffCommand.ExecuteAsync(null);

            await Shell.Current.DisplayAlert("Success", "Staff member added successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to add staff member: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ManageSchedule(StaffMember staff)
    {
        await Shell.Current.GoToAsync($"ScheduleManagementPage?StaffId={staff.Id}&StaffName={staff.Name}");
    }

    [RelayCommand]
    async Task DeleteStaff(StaffMember staff)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to remove {staff.Name}? This will also delete all their schedules and time slots.",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _apiService.DeleteStaffMemberAsync(staff.Id);
            StaffMembers.Remove(staff);

            await Shell.Current.DisplayAlert("Success", "Staff member deleted", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete staff member: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
