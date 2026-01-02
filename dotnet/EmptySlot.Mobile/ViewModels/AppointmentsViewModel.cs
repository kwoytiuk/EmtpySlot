using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

public partial class AppointmentsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Appointment> appointments = new();


    public AppointmentsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "My Appointments";
    }

    [RelayCommand]
    async Task LoadAppointments()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var appts = await _apiService.GetMyAppointmentsAsync();
            Appointments.Clear();
            foreach (var appt in appts)
            {
                Appointments.Add(appt);
            }
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
