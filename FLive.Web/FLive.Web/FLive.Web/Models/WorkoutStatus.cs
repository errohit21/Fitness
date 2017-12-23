namespace FLive.Web.Models
{
    public enum WorkoutStatus {
        Created = 1,
        Started = 2,
        InProgress = 3,
        StreamStopped = 4,
        RecordingDownloaded = 5,
        StreamDeleted = 6,
        StartedCharingSubscribers=20,
        FinishedCharingSubscribers=21,
        FailedToStart =101, }
}