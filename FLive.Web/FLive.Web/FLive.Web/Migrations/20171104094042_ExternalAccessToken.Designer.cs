using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FLive.Web.Data;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models;

namespace FLive.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20171104094042_ExternalAccessToken")]
    partial class ExternalAccessToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FLive.Web.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("Age");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("DeviceToken");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FacebookToken");

                    b.Property<double?>("LocationLatitude");

                    b.Property<double?>("LocationLongitude");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("MobileNumber");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("Platform");

                    b.Property<string>("PostCode");

                    b.Property<string>("ProfileImageUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Timezone");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<int>("UserType");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("FLive.Web.Models.Competency", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Competencies");
                });

            modelBuilder.Entity("FLive.Web.Models.Country", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DialingCode");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("FLive.Web.Models.IdentityVerification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Code");

                    b.Property<string>("Email");

                    b.Property<string>("IdentityToken");

                    b.Property<string>("MobileNumber");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("IdentityVerifications");
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkout", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedTime");

                    b.Property<long>("DurationMins");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("ExternalAccessToken");

                    b.Property<string>("InstructionsLink");

                    b.Property<bool>("IsArchived");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublishImmediately");

                    b.Property<long>("LikeCount");

                    b.Property<string>("PreviewImagelUrl");

                    b.Property<string>("PreviewVideoUrl");

                    b.Property<long>("PriceTierId");

                    b.Property<DateTime?>("PublishDateTime");

                    b.Property<string>("RecordingUrl");

                    b.Property<long?>("SponsorId");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("StreamId");

                    b.Property<long>("SubscriberCount");

                    b.Property<long?>("SubscriberId");

                    b.Property<string>("Title");

                    b.Property<long>("TrainerId");

                    b.Property<long>("TrainingCategoryId");

                    b.Property<long>("TransactionId");

                    b.Property<int>("WorkoutLevel");

                    b.Property<int>("WorkoutPublishStatus");

                    b.Property<int>("WorkoutStatus");

                    b.Property<int>("WorkoutType");

                    b.HasKey("Id");

                    b.HasIndex("PriceTierId");

                    b.HasIndex("SponsorId");

                    b.HasIndex("SubscriberId");

                    b.HasIndex("TrainerId");

                    b.HasIndex("TrainingCategoryId");

                    b.ToTable("LiveWorkouts");
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkoutFavor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("LiveWorkoutId");

                    b.Property<long>("SubscriberId");

                    b.HasKey("Id");

                    b.HasIndex("LiveWorkoutId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("LiveWorkoutFavors");
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkoutSubscriber", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("LiveWorkoutId");

                    b.Property<long>("SubscriberId");

                    b.HasKey("Id");

                    b.HasIndex("LiveWorkoutId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("LiveWorkoutSubscriber");
                });

            modelBuilder.Entity("FLive.Web.Models.Notification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime>("EventDateTime");

                    b.Property<string>("Message");

                    b.Property<string>("MessageType");

                    b.Property<int>("NotificationStatus");

                    b.Property<string>("PreviewImagelUrl");

                    b.Property<string>("ProfileImage");

                    b.Property<string>("Title");

                    b.Property<long?>("TrainerId");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName");

                    b.Property<DateTime>("WorkoutEndTime");

                    b.Property<DateTime>("WorkoutStartTime");

                    b.HasKey("Id");

                    b.ToTable("Notifications");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Notification");
                });

            modelBuilder.Entity("FLive.Web.Models.PriceTier", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency");

                    b.Property<string>("IapKey");

                    b.Property<bool>("IsSubscriptionTier");

                    b.Property<decimal>("Price");

                    b.HasKey("Id");

                    b.ToTable("PriceTiers");
                });

            modelBuilder.Entity("FLive.Web.Models.Settings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PrivacyPolicy");

                    b.Property<string>("Support");

                    b.Property<string>("TrainerTermsAndConditions");

                    b.Property<string>("UserTermsAndConditions");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("FLive.Web.Models.Sponsor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("SponsorsMessage");

                    b.Property<string>("TrademarkUrl");

                    b.HasKey("Id");

                    b.ToTable("Sponsors");
                });

            modelBuilder.Entity("FLive.Web.Models.Stream", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Streams");
                });

            modelBuilder.Entity("FLive.Web.Models.Subscriber", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserId");

                    b.Property<bool>("CreditCardVerified");

                    b.Property<string>("Currency");

                    b.Property<long>("FollowingCount");

                    b.Property<int>("LevelOfCompetency");

                    b.Property<string>("StripeCustomerId");

                    b.Property<long>("WorkoutsCount");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("FLive.Web.Models.SubscriberTrainingGoal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("SubscriberId");

                    b.Property<long>("TrainingGoalId");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberId");

                    b.HasIndex("TrainingGoalId");

                    b.ToTable("SubscriberTrainingGoals");
                });

            modelBuilder.Entity("FLive.Web.Models.Trainer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountNumber");

                    b.Property<string>("AddressLine1");

                    b.Property<string>("AddressLine2");

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("BSB");

                    b.Property<string>("Bio");

                    b.Property<string>("Country");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsVerified");

                    b.Property<int>("LevelOfCompetency");

                    b.Property<long>("LikeCount");

                    b.Property<string>("PostCode");

                    b.Property<string>("SripeAuthResult");

                    b.Property<string>("StripeUserId");

                    b.Property<long?>("SubscriberId");

                    b.Property<string>("Suburb");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("SubscriberId");

                    b.ToTable("Trainers");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerFavor", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("FavorerId");

                    b.Property<string>("FavorerId1");

                    b.Property<long>("TrainerId");

                    b.HasKey("Id");

                    b.HasIndex("FavorerId1");

                    b.HasIndex("TrainerId");

                    b.ToTable("TrainerFavors");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerFollower", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("SubscriberId");

                    b.Property<long>("TrainerId");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberId");

                    b.HasIndex("TrainerId");

                    b.ToTable("TrainerFollowers");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerSubsriber", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("SubscriberId");

                    b.Property<long>("TrainerId");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberId");

                    b.HasIndex("TrainerId");

                    b.ToTable("TrainerSubsribers");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerTrainingCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("TrainerId");

                    b.Property<long>("TrainingCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("TrainerId");

                    b.HasIndex("TrainingCategoryId");

                    b.ToTable("TrainerTrainingCategories");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerTrainingGoal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("TrainerId");

                    b.Property<long>("TrainingGoalId");

                    b.HasKey("Id");

                    b.HasIndex("TrainerId");

                    b.HasIndex("TrainingGoalId");

                    b.ToTable("TrainerTrainingGoals");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainingCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("DescriptionColor");

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsSponsorCategory");

                    b.Property<long>("LiveWorkoutCount");

                    b.Property<string>("Name");

                    b.Property<string>("NameColor");

                    b.Property<string>("SponsorLogo");

                    b.Property<string>("SponsorText");

                    b.HasKey("Id");

                    b.ToTable("TrainingCategories");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainingGoal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TrainingGoals");
                });

            modelBuilder.Entity("FLive.Web.Models.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<bool>("IsSuccessfull");

                    b.Property<long>("LiveWorkoutId");

                    b.Property<string>("Notes");

                    b.Property<long>("SubscriberId");

                    b.Property<long>("TrainerId");

                    b.Property<DateTime>("TranactionDateTime");

                    b.HasKey("Id");

                    b.HasIndex("LiveWorkoutId")
                        .IsUnique();

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("FLive.Web.Models.UpcomingLiveWorkout", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreateStreamJson");

                    b.Property<bool>("IsPublishImmediately");

                    b.Property<long>("LiveWorkoutId");

                    b.Property<string>("MediaServiceUrl");

                    b.Property<DateTime?>("PublishDateTime");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("WorkoutPublishStatus");

                    b.Property<int>("WorkoutType");

                    b.HasKey("Id");

                    b.ToTable("UpcomingLiveWorkouts");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("FLive.Web.Models.PushNotification", b =>
                {
                    b.HasBaseType("FLive.Web.Models.Notification");

                    b.Property<string>("DeviceToken");

                    b.Property<int>("DeviceType");

                    b.Property<string>("Tags");

                    b.ToTable("PushNotification");

                    b.HasDiscriminator().HasValue("PushNotification");
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkout", b =>
                {
                    b.HasOne("FLive.Web.Models.PriceTier", "PriceTier")
                        .WithMany()
                        .HasForeignKey("PriceTierId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.Sponsor", "Sponsor")
                        .WithMany()
                        .HasForeignKey("SponsorId");

                    b.HasOne("FLive.Web.Models.Subscriber")
                        .WithMany("Workouts")
                        .HasForeignKey("SubscriberId");

                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("LiveWorkouts")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.TrainingCategory", "TrainingCategory")
                        .WithMany()
                        .HasForeignKey("TrainingCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkoutFavor", b =>
                {
                    b.HasOne("FLive.Web.Models.LiveWorkout", "LiveWorkout")
                        .WithMany("LiveWorkoutFavors")
                        .HasForeignKey("LiveWorkoutId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.Subscriber", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.LiveWorkoutSubscriber", b =>
                {
                    b.HasOne("FLive.Web.Models.LiveWorkout", "LiveWorkout")
                        .WithMany("LiveWorkoutSubscribers")
                        .HasForeignKey("LiveWorkoutId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.Subscriber", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.Subscriber", b =>
                {
                    b.HasOne("FLive.Web.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("FLive.Web.Models.SubscriberTrainingGoal", b =>
                {
                    b.HasOne("FLive.Web.Models.Subscriber", "Subscriber")
                        .WithMany("SubscriberTrainingGoals")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.TrainingGoal", "TrainingGoal")
                        .WithMany()
                        .HasForeignKey("TrainingGoalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.Trainer", b =>
                {
                    b.HasOne("FLive.Web.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("FLive.Web.Models.Subscriber")
                        .WithMany("Following")
                        .HasForeignKey("SubscriberId");
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerFavor", b =>
                {
                    b.HasOne("FLive.Web.Models.ApplicationUser", "Favorer")
                        .WithMany()
                        .HasForeignKey("FavorerId1");

                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("TrainerFavors")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerFollower", b =>
                {
                    b.HasOne("FLive.Web.Models.Subscriber", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("TrainerFollowers")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerSubsriber", b =>
                {
                    b.HasOne("FLive.Web.Models.Subscriber", "Subscriber")
                        .WithMany()
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("TrainerSubscribers")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerTrainingCategory", b =>
                {
                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("TrainerTrainingCategories")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.TrainingCategory", "TrainingCategory")
                        .WithMany()
                        .HasForeignKey("TrainingCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.TrainerTrainingGoal", b =>
                {
                    b.HasOne("FLive.Web.Models.Trainer", "Trainer")
                        .WithMany("TrainerTrainingGoals")
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.TrainingGoal", "TrainingGoal")
                        .WithMany()
                        .HasForeignKey("TrainingGoalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FLive.Web.Models.Transaction", b =>
                {
                    b.HasOne("FLive.Web.Models.LiveWorkout", "LiveWorkout")
                        .WithOne("Transaction")
                        .HasForeignKey("FLive.Web.Models.Transaction", "LiveWorkoutId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FLive.Web.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FLive.Web.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FLive.Web.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
