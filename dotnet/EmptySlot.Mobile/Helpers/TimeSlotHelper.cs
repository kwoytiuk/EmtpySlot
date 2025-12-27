namespace EmptySlot.Mobile.Helpers;

public static class TimeSlotHelper
{
    public static List<string> GetNext3TimeSlots()
    {
        var slots = new List<string>();
        var now = DateTime.Now;

        // Round to next 30-minute interval
        var minute = now.Minute < 30 ? 30 : 0;
        var hour = now.Minute < 30 ? now.Hour : now.Hour + 1;

        // If it's past 8 PM, start from noon next day or 12 PM today
        if (hour >= 20)
        {
            hour = 12;
            minute = 0;
        }

        // Generate next 3 slots (30-minute intervals for realistic booking)
        for (int i = 0; i < 3; i++)
        {
            if (hour >= 21) break; // Don't go past 9 PM

            var slotTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            slots.Add(slotTime.ToString("h:mm tt"));

            // Increment by variable intervals for more realistic slots
            if (i == 0)
            {
                // First slot: 30 min from now
                minute += 30;
            }
            else if (i == 1)
            {
                // Second slot: 2 hours later
                hour += 2;
                minute = 0;
            }
            else
            {
                // Third slot: 1.5 hours later
                hour += 1;
                minute = 30;
            }

            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }
        }

        // Fallback if no slots generated
        if (slots.Count == 0)
        {
            slots.Add("12:00 PM");
            slots.Add("2:30 PM");
            slots.Add("4:00 PM");
        }

        return slots;
    }

    public static string GetPopularBookingMessage()
    {
        var random = new Random();
        var count = random.Next(2, 8);
        return $"🎉 {count} people booked in the last hour";
    }
}
