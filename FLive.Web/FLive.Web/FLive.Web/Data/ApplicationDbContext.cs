using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FLive.Web.Models;

namespace FLive.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<Transaction>().HasOne<LiveWorkout>();
            builder.Entity<LiveWorkout>()
            .HasOne(p => p.Transaction)
            .WithOne(i => i.LiveWorkout)
            .HasForeignKey<Transaction>(b => b.LiveWorkoutId);
        }

        public DbSet<Stream> Streams { get; set; }
        public DbSet<LiveWorkout> LiveWorkouts { get; set; }
        public DbSet<UpcomingLiveWorkout> UpcomingLiveWorkouts { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainingCategory> TrainingCategories { get; set; }
        public DbSet<TrainerTrainingCategory> TrainerTrainingCategories { get; set; }
        public DbSet<TrainingGoal> TrainingGoals { get; set; }
        public DbSet<TrainerTrainingGoal> TrainerTrainingGoals { get; set; }
        public DbSet<PriceTier> PriceTiers { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubscriberTrainingGoal> SubscriberTrainingGoals { get; set; }
        public DbSet<TrainerFavor> TrainerFavors { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<LiveWorkoutFavor> LiveWorkoutFavors { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IdentityVerification> IdentityVerifications { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<TrainerFollower> TrainerFollowers { get; set; }
        public DbSet<TrainerSubsriber> TrainerSubsribers { get; set; }
        public DbSet<WorkoutLength> WorkoutLength { get; set; }
        public DbSet<WorkoutType> WorkoutType { get; set; }

    }
}
