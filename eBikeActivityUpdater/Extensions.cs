using System;
using eBikeActivityUpdater.Models;

namespace eBikeActivityUpdater
{
    public static class Extensions
    {
        public static UpdateableActivity ToUpdateable(this Activity source)
        {
            return new UpdateableActivity
            {
                 Name = source.Name,
                 Description = source.Description?.ToString(),
                 Commute = source.Commute,
                 Trainer = source.Trainer,
                 GearId = "b5873380",
                 Type = "EBikeRide"
            };
        }
    }
}
