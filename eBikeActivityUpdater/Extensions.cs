using System;
using eBikeActivityUpdater.Models;

namespace eBikeActivityUpdater
{
    public static class Extensions
    {
        public static UpdateableActivity ToUpdateable(this Activity source, string activityType, string gearId)
        {
            return new UpdateableActivity
            {
                 Name = source.Name,
                 Description = source.Description?.ToString(),
                 Commute = source.Commute,
                 Trainer = source.Trainer,
                 GearId = gearId,
                 Type = activityType
            };
        }
    }
}
