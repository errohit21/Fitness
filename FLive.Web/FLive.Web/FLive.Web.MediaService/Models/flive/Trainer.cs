using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;
using FLive.Web.MediaService.Models.ViewModels;

namespace FLive.Web.MediaService.Models.flive
{
    public class Trainer
    {
        public Trainer()
        {

        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string ProfileImageUrl { get; set; }

        public string PostCode { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string LocationLatitude { get; set; }
        public string LocationLongitude { get; set; }

        public string Bio { get; set; }
        public LevelOfCompetency LevelOfCompetency { get; set; }

        public string BSB { get; set; }
        public string AccountNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Suburb { get; set; }
        public string Country { get; set; }

        public bool IsVerified { get; set; }
        public long LikeCount { get; set; }
        public bool IsActive { get; set; }

        public IList<LiveWorkout> LiveWorkouts { get; set; }

        public string Address
        {
            get
            {
                var address = AddressLine1;
                if (!string.IsNullOrEmpty(AddressLine2))
                    address += ", " + AddressLine2;
                return address;
            }
        }


        public static IList<Trainer> SearchTrainers(string searchText)
        {
            var modelList = new List<Trainer>();

            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (200) T.[Id]
                              ,[AccountNumber]
                              ,[ApplicationUserId]
                              ,[BSB]
                              ,[Bio]
                              ,[IsVerified]
                              ,[LevelOfCompetency]
                              ,[LikeCount]
                              ,[AddressLine1]
                              ,[AddressLine2]
                              ,[Country]
                              ,T.[PostCode]
                              ,[Suburb]
	                          ,U.[Name]
	                          ,U.[Age]
	                          ,U.[ProfileImageUrl]
	                          ,U.[MobileNumber]
	                          ,U.[PhoneNumber]
	                          ,U.[Email]
                              ,U.[LocationLatitude]
                              ,U.[LocationLongitude]
                              ,T.[IsDeleted]
                          FROM [dbo].[Trainers] T
                          inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where [Name] like '%{0}%' OR [MobileNumber] like '%{0}%' OR [PhoneNumber] like '%{0}%' OR [Email] like '%{0}%'";
            #endregion

            searchSql = string.Format(searchSql, searchText);
            var dt = Helpers.SqlHelper.ExecuteStatement(searchSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                modelList.Add(MapTrainer(dt.Rows[i]));
            }

            return modelList;
        }

        public static Trainer GetTrainer(string id)
        {
            #region --- Select SQL ----
            var searchSql = @"SELECT TOP (200) T.[Id]
                              ,[AccountNumber]
                              ,[ApplicationUserId]
                              ,[BSB]
                              ,[Bio]
                              ,[IsVerified]
                              ,[LevelOfCompetency]
                              ,[LikeCount]
                              ,[AddressLine1]
                              ,[AddressLine2]
                              ,[Country]
                              ,T.[PostCode]
                              ,[Suburb]
	                          ,U.[Name]
	                          ,U.[Age]
	                          ,U.[ProfileImageUrl]
	                          ,U.[MobileNumber]
	                          ,U.[PhoneNumber]
	                          ,U.[Email]
                              ,U.[LocationLatitude]
                              ,U.[LocationLongitude]
                              ,T.[IsDeleted]
                          FROM [dbo].[Trainers] T
                          inner join [dbo].[AspNetUsers] U on U.[Id] = T.[ApplicationUserId]
                          where T.[Id] = {0}";
            #endregion

            searchSql = string.Format(searchSql, id);
            var dt = Helpers.SqlHelper.ExecuteStatement(searchSql);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return MapTrainer(dt.Rows[0]);
            }
        }

        public static TransactionStatus UpdateActiveStatus(ActiveStatusUpdateViewModel activeStatusModel)
        {
            var status = new TransactionStatus() { IsSuccessfull = true };

            #region --- Update SQL ----
            var updateSql = string.Format(@"update [Trainers] set [IsDeleted] = {1} where [Id] = {0}
                                    update [LiveWorkouts] set [IsDeleted] = {1} where [TrainerId] = {0}", activeStatusModel.Id, activeStatusModel.IsActive ? "0" : "1");
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

        private static Trainer MapTrainer(DataRow dr)
        {
            var model = new Trainer();
            model.AccountNumber = dr["AccountNumber"].ToString();
            model.AddressLine1 = dr["AddressLine1"].ToString();
            model.AddressLine2 = dr["AddressLine2"].ToString();
            model.Age = dr["Age"].ToString();
            model.Bio = dr["Bio"].ToString();
            model.BSB = dr["BSB"].ToString();
            model.Country = dr["Country"].ToString();
            model.Id = dr["Id"].ToString();
            model.IsVerified = bool.Parse(dr["IsVerified"].ToString());
            model.LevelOfCompetency = (LevelOfCompetency)Enum.ToObject(typeof(LevelOfCompetency), int.Parse(dr["LevelOfCompetency"].ToString()));
            model.LikeCount = long.Parse(dr["LikeCount"].ToString());
            model.LocationLatitude = dr["LocationLatitude"].ToString();
            model.LocationLongitude = dr["LocationLongitude"].ToString();
            model.MobileNumber = dr["MobileNumber"].ToString();
            model.Name = dr["Name"].ToString();
            model.PostCode = dr["PostCode"].ToString();
            model.ProfileImageUrl = dr["ProfileImageUrl"].ToString();
            model.Suburb = dr["Suburb"].ToString();
            model.Email = dr["Email"].ToString();
            model.PhoneNumber = dr["PhoneNumber"].ToString();
            model.IsActive = !bool.Parse(dr["IsDeleted"].ToString());

            if (string.IsNullOrEmpty(model.ProfileImageUrl))
                model.ProfileImageUrl = "/Content/admin/img/logo_light.png";

            return model;
        }
    }

    public enum LevelOfCompetency
    {
        Unspecified = 0,
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3
    }
}