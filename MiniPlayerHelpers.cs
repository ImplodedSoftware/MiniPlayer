namespace MiniPlayer
{
    public static class MiniPlayerHelpers
    {
        public static string FormatTime(double duration, bool includeHour = false)
        {
            if (duration < 0)
                duration = 0;
            if (includeHour)
            {
                var h = duration / 3600;
                duration = duration - (h * 3600);
                var m = duration / 60;
                duration = duration - (m * 60);
                return $"{h:00}:{m:00}:{duration:00}";
            }
            else
            {
                var m = (int)(duration / 60.0);
                duration = duration - (m * 60);
                return $"{m:00}:{duration:00}";
            }
        }
    }
}
