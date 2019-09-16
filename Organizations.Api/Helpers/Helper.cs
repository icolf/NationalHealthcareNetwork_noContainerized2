using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Enums;

namespace Organizations.Api.Helpers
{
    public class Helper
    {
        public static class Helpers
        {
            public static EntityState ConvertState(TrackedStatus objectState)
            {
                switch (objectState)
                {
                    case TrackedStatus.Added:
                        return EntityState.Added;
                    case TrackedStatus.Deleted:
                        return EntityState.Deleted;
                    case TrackedStatus.Modified:
                        return EntityState.Modified;
                    case TrackedStatus.Unchanged:
                        return EntityState.Unchanged;
                    default:
                        return EntityState.Unchanged;
                }
            }

            //public static void ApplyStateChanges(this ApplicationDbContext context)
            //{
            //    foreach (var entry in context.ChangeTracker.Entries<IObjectWithState>())
            //    {
            //        IObjectWithState stateInfo = entry.Entity;
            //        entry.State = ConvertState(stateInfo.TrackedStatus);
            //    }
            //}
        }

    }
}
