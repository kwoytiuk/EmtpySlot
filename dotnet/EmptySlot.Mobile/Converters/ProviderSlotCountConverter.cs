using System.Globalization;
using EmptySlot.Mobile.ViewModels;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Converters;

/// <summary>
/// Converts a Provider to its available slot count text
/// Requires the ConverterParameter to be the SearchViewModel instance
/// </summary>
public class ProviderSlotCountConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Provider provider && parameter is SearchViewModel viewModel)
        {
            var count = viewModel.GetProviderSlotCount(provider.Id);

            if (count == 0)
                return "No slots today";
            else if (count == 1)
                return "1 slot today";
            else
                return $"{count} slots today";
        }

        return "Loading...";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
