using FLive.Web.MediaService.Helpers;
using FLive.Web.MediaService.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace FLive.Web.MediaService.Models.flive
{
    public class LiveWorkout
    {
        public LiveWorkout()
        {

        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string TrainerId { get; set; }
        public string TrainerUsername { get; set; }
        public string TrainingCategory { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? StartTime { get; set; }
        public long DurationMins { get; set; }
        public DateTime? EndTime { get; set; }
        public string PreviewImagelUrl { get; set; }
        public long SubscriberCount { get; set; }
        public long LikeCount { get; set; }
        public bool IsActive { get; set; }

        public static IList<LiveWorkout> GetLiveWorkoutsForTrainer(string trainerId)
        {
            var modelList = new List<LiveWorkout>();

            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (1000) L.[Id]
                              ,[DurationMins]
                              ,[EndTime]
                              ,[PreviewVideoUrl]
                              ,[PriceTierId]
                              ,[StartTime]
                              ,[StreamId]
                              ,[SubscriberCount]
                              ,L.[SubscriberId]
                              ,[TrainerId]
                              ,C.[Name] as [TrainingCategory]
                              ,[WorkoutLevel]
                              ,[WorkoutStatus]
                              ,[CreatedTime]
                              ,[Title]
                              ,[PreviewImagelUrl]
                              ,L.[LikeCount]
                              ,[RecordingUrl]
                              ,[TransactionId]
                              ,L.[IsDeleted]
                              ,U.[Email]
                          FROM [dbo].[LiveWorkouts] L
                          inner join [dbo].[TrainingCategories] C on C.[Id] = L.[TrainingCategoryId]
						  inner join [dbo].[Trainers] T on T.Id = L.[TrainerId]
						  inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where [TrainerId] = {0}";
            #endregion

            searchSql = string.Format(searchSql, trainerId);
            var dt = Helpers.SqlHelper.ExecuteStatement(searchSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                modelList.Add(MapLiveWorkout(dt.Rows[i]));
            }

            return modelList;
        }

        public static IList<LiveWorkout> GetFreeLiveWorkouts(string searchText)
        {
            var modelList = new List<LiveWorkout>();

            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (1000) L.[Id]
                              ,[DurationMins]
                              ,[EndTime]
                              ,[PreviewVideoUrl]
                              ,[PriceTierId]
                              ,[StartTime]
                              ,[StreamId]
                              ,[SubscriberCount]
                              ,L.[SubscriberId]
                              ,[TrainerId]
                              ,C.[Name] as [TrainingCategory]
                              ,[WorkoutLevel]
                              ,[WorkoutStatus]
                              ,[CreatedTime]
                              ,[Title]
                              ,[PreviewImagelUrl]
                              ,L.[LikeCount]
                              ,[RecordingUrl]
                              ,[TransactionId]
                              ,L.[IsDeleted]
                              ,U.[Email]
                          FROM [dbo].[LiveWorkouts] L
                          inner join [dbo].[TrainingCategories] C on C.[Id] = L.[TrainingCategoryId]
						  inner join [dbo].[Trainers] T on T.Id = L.[TrainerId]
						  inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where SponsorId is not NULL
                          AND (U.[Name] like '%{0}%' OR U.[Email] like '%{0}%' OR L.[Title] like '%{0}%' OR C.[Name] like '%{0}%')";
            #endregion

            searchSql = string.Format(searchSql, searchText);
            var dt = SqlHelper.ExecuteStatement(searchSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                modelList.Add(MapLiveWorkout(dt.Rows[i]));
            }

            return modelList;
        }

        public static IList<LiveWorkout> GetPaidLiveWorkouts(string searchText)
        {
            var modelList = new List<LiveWorkout>();

            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (1000) L.[Id]
                              ,[DurationMins]
                              ,[EndTime]
                              ,[PreviewVideoUrl]
                              ,[PriceTierId]
                              ,[StartTime]
                              ,[StreamId]
                              ,[SubscriberCount]
                              ,L.[SubscriberId]
                              ,[TrainerId]
                              ,C.[Name] as [TrainingCategory]
                              ,[WorkoutLevel]
                              ,[WorkoutStatus]
                              ,[CreatedTime]
                              ,[Title]
                              ,[PreviewImagelUrl]
                              ,L.[LikeCount]
                              ,[RecordingUrl]
                              ,[TransactionId]
                              ,L.[IsDeleted]
                              ,U.[Email]
                          FROM [dbo].[LiveWorkouts] L
                          inner join [dbo].[TrainingCategories] C on C.[Id] = L.[TrainingCategoryId]
						  inner join [dbo].[Trainers] T on T.Id = L.[TrainerId]
						  inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where SponsorId is NULL
                          AND (U.[Name] like '%{0}%' OR U.[Email] like '%{0}%' OR L.[Title] like '%{0}%' OR C.[Name] like '%{0}%')";
            #endregion

            searchSql = string.Format(searchSql, searchText);
            var dt = SqlHelper.ExecuteStatement(searchSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                modelList.Add(MapLiveWorkout(dt.Rows[i]));
            }

            return modelList;
        }

        public static IList<LiveWorkout> GetNonFreeLiveWorkouts(string searchText)
        {
            var modelList = new List<LiveWorkout>();

            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (1000) L.[Id]
                              ,[DurationMins]
                              ,[EndTime]
                              ,[PreviewVideoUrl]
                              ,[PriceTierId]
                              ,[StartTime]
                              ,[StreamId]
                              ,[SubscriberCount]
                              ,L.[SubscriberId]
                              ,[TrainerId]
                              ,C.[Name] as [TrainingCategory]
                              ,[WorkoutLevel]
                              ,[WorkoutStatus]
                              ,[CreatedTime]
                              ,[Title]
                              ,[PreviewImagelUrl]
                              ,L.[LikeCount]
                              ,[RecordingUrl]
                              ,[TransactionId]
                              ,L.[IsDeleted]
                              ,U.[Email]
                          FROM [dbo].[LiveWorkouts] L
                          inner join [dbo].[TrainingCategories] C on C.[Id] = L.[TrainingCategoryId]
						  inner join [dbo].[Trainers] T on T.Id = L.[TrainerId]
						  inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where SponsorId is NULL
                          AND (U.[Name] like '%{0}%' OR U.[Email] like '%{0}%' OR L.[Title] like '%{0}%' OR C.[Name] like '%{0}%')";
            #endregion

            searchSql = string.Format(searchSql, searchText);
            var dt = SqlHelper.ExecuteStatement(searchSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                modelList.Add(MapLiveWorkout(dt.Rows[i]));
            }

            return modelList;
        }

        public static TransactionStatus UpdateWorkoutFreeStatus(ActiveStatusUpdateViewModel activeStatusModel)
        {
            var status = new TransactionStatus() { IsSuccessfull = true };

            #region --- Update SQL ----
            var updateSql = string.Format(@"update [LiveWorkouts] set [SponsorId] = {0} where [Id] = {1}",
                activeStatusModel.IsActive ? ConfigurationManager.AppSettings["DefaultSpnsor"].ToString() : "null",
                activeStatusModel.Id);
            #endregion

            try
            {
                var updateCount = Helpers.SqlHelper.ExecuteUpdate(updateSql);

                if (updateCount == 0)
                {
                    status.IsSuccessfull = false;
                }
            }
            catch (Exception exception)
            {
                status.IsSuccessfull = false;
                status.Message = exception.Message;
                status.Exception = exception;
            }

            return status;
        }

        public static TransactionStatus UpdateWorkoutStreamingURL(string workoutId, string stramingUrl)
        {
            var status = new TransactionStatus() { IsSuccessfull = true };

            #region --- Update SQL ----
            var updateSql = string.Format(@"INSERT INTO [dbo].[UpcomingLiveWorkouts]([LiveWorkoutId],
                                            [StartTime],[WorkoutType],[IsPublishImmediately],[WorkoutPublishStatus],
                                            [MediaServiceUrl]) VALUES ({0}, GETDATE(),2,1,4,'{1}')",
                workoutId, stramingUrl);
            #endregion

            try
            {
                var updateCount = Helpers.SqlHelper.ExecuteUpdate(updateSql);

                if (updateCount == 0)
                {
                    status.IsSuccessfull = false;
                }
            }
            catch (Exception exception)
            {
                status.IsSuccessfull = false;
                status.Message = exception.Message;
                status.Exception = exception;
            }

            return status;
        }

        private static LiveWorkout MapLiveWorkout(DataRow dr)
        {
            var model = new LiveWorkout();
            model.Id = dr["Id"].ToString();
            model.CreatedTime = Common.ParseDate(dr["CreatedTime"].ToString());
            model.DurationMins = int.Parse(dr["DurationMins"].ToString());
            model.EndTime = Common.ParseDate(dr["EndTime"].ToString());
            model.LikeCount = int.Parse(dr["LikeCount"].ToString());
            model.PreviewImagelUrl = dr["PreviewImagelUrl"].ToString();
            model.StartTime = Common.ParseDate(dr["StartTime"].ToString());
            model.SubscriberCount = int.Parse(dr["SubscriberCount"].ToString());
            model.Title = dr["Title"].ToString();
            model.TrainerId = dr["TrainerId"].ToString();
            model.TrainingCategory = dr["TrainingCategory"].ToString();
            model.IsActive = !bool.Parse(dr["IsDeleted"].ToString());
            model.TrainerUsername = dr["Email"].ToString();

            return model;
        }
    }
}