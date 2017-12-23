using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            PlatformCounts = new List<PlatformCountViewModel>();
            UserLocations = new List<LocationViewmodel>();
        }

        public string TrainersCount { get; set; }
        public string SubscribersCount { get; set; }
        public string LiveWorkoutsCount { get; set; }
        public string UpcomingLiveWorkoutsCount { get; set; }

        public IList<PlatformCountViewModel> PlatformCounts { get; set; }
        public IList<LocationViewmodel> UserLocations { get; set; }
        public string UserLocationsJsonStr { get; set; }

        public static DashboardViewModel Build() {
            var model = new DashboardViewModel();

            var trainerDT = Helpers.SqlHelper.ExecuteStatement("select count(Id) from Trainers where IsDeleted = 0");
            if (trainerDT.Rows.Count > 0)
            {
                model.TrainersCount = trainerDT.Rows[0][0].ToString();
            }

            var subscriberDT = Helpers.SqlHelper.ExecuteStatement("select count(Id) from [Subscribers]");
            if (subscriberDT.Rows.Count > 0)
            {
                model.SubscribersCount = subscriberDT.Rows[0][0].ToString();
            }

            var liveWorkoutDT = Helpers.SqlHelper.ExecuteStatement("select count(Id) from [LiveWorkouts] where IsDeleted = 0");
            if (liveWorkoutDT.Rows.Count > 0)
            {
                model.LiveWorkoutsCount = liveWorkoutDT.Rows[0][0].ToString();
            }

            var upcomingWorkoutDT = Helpers.SqlHelper.ExecuteStatement("select count(Id) from UpcomingLiveWorkouts");
            if (upcomingWorkoutDT.Rows.Count > 0)
            {
                model.UpcomingLiveWorkoutsCount = upcomingWorkoutDT.Rows[0][0].ToString();
            }

            var platformDT = Helpers.SqlHelper.ExecuteStatement("select count(id), [Platform] FROM [dbo].[AspNetUsers] group by [Platform]");
            var otheresCount = 0;
            for (int i = 0; i < platformDT.Rows.Count; i++)
            {
                if (platformDT.Rows[i][1].ToString().Equals("Android") || platformDT.Rows[i][1].ToString().Equals("Apple"))
                {
                    model.PlatformCounts.Add(new PlatformCountViewModel()
                    {
                        Count = platformDT.Rows[i][0].ToString(),
                        Platform = platformDT.Rows[i][1].ToString()
                    });
                }
                else {
                    otheresCount += int.Parse(platformDT.Rows[i][0].ToString());
                }
            }

            if (otheresCount > 0)
            {
                model.PlatformCounts.Add(new PlatformCountViewModel()
                {
                    Count = otheresCount.ToString(),
                    Platform = "Other"
                });
            }

            var locationDT = Helpers.SqlHelper.ExecuteStatement("select [LocationLatitude], [LocationLongitude] from [AspNetUsers] where [LocationLatitude] is not null and [LocationLongitude] is not null");
            for (int i = 0; i < locationDT.Rows.Count; i++)
            {
                model.UserLocations.Add(new LocationViewmodel()
                {
                    Lat = locationDT.Rows[i][0].ToString(),
                    Lng = locationDT.Rows[i][1].ToString()
                });
            }
            model.UserLocationsJsonStr = JsonConvert.SerializeObject(model.UserLocations);

            return model;
        }
    }

    public class LocationViewmodel
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}