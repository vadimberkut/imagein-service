using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Helpers
{
    public static class DateTimeMapper
    {
        public static DateTime Normalize(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public static DateTime? NormalizeNullable(DateTime? value)
        {
            if(value.HasValue)
            {
                return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            }
            return null;
        }
    }
}
