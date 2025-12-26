namespace EmptySlot.Mobile.Helpers;

public static class TimeSlotHelper
{
    public static List<string> GetNext4TimeSlots()
    {
        var slots = new List<string>();
        var now = DateTime.Now;

        // Round to next 15-minute interval
        var minute = (now.Minute / 15 + 1) * 15;
        var hour = now.Hour;

        if (minute >= 60)
        {
            minute = 0;
            hour++;
        }

        // Generate next 4 slots (15-minute intervals)
        for (int i = 0; i < 4; i++)
        {
            var slotTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

            // Skip if past 9 PM
            if (slotTime.Hour >= 21)
                break;

            slots.Add(slotTime.ToString("h:mm tt"));

            // Increment by 15 minutes
            minute += 15;
            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }
        }

        // If no slots available today, start from 5:45 PM (typical dinner time)
        if (slots.Count == 0)
        {
            var dinnerTime = new DateTime(now.Year, now.Month, now.Day, 17, 45, 0);
            for (int i = 0; i < 4; i++)
            {
                slots.Add(dinnerTime.AddMinutes(i * 15).ToString("h:mm tt"));
            }
        }

        return slots;
    }

    public static string GetPopularBookingMessage()
    {
        var random = new Random();
        var count = random.Next(23, 87);
        return $"Booked {count} times today";
    }
}
