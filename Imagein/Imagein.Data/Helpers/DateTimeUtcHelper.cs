using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Data.Helpers
{
    public static class DateTimeUtcHelper
    {

        /// <summary>
        /// Converts all DateTime properties of changed entities to UTC
        /// </summary>
        /// <param name="changes"></param>
        internal static void SetDatesToUtc(IEnumerable<EntityEntry> changes)
        {
            var trackList = new[] { EntityState.Added, EntityState.Modified };
            foreach (var dbEntry in changes)
            {
                foreach (var property in dbEntry.CurrentValues.Properties)
                {
                    // using reflection add logic to determine if its a DateTime or nullable DateTime
                    // && if its kind = DateTimeKind.Local or Unspecified
                    // and then convert set the Utc value
                    // and write it back to the entry using dbEntry.CurrentValues[propertyName] = utcvalue;

                    if (property.PropertyInfo.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)dbEntry.CurrentValues[property.Name];
                        if(dateTime.Kind != DateTimeKind.Utc)
                        {
                            dbEntry.CurrentValues[property.Name] = (object)dateTime.ToUniversalTime();
                        }
                    }

                    if(property.PropertyInfo.GetType() == typeof(DateTime?))
                    {
                        DateTime? dateTime = (DateTime?)dbEntry.CurrentValues[property.Name];
                        if(dateTime != null && dateTime.Value.Kind != DateTimeKind.Utc)
                        {
                            dbEntry.CurrentValues[property.Name] = (object)dateTime.Value.ToUniversalTime();
                        }
                    }
                }
            }
        }
    }
}
