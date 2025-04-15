namespace FriendsSociety.Shaurya.Entities
{
    public class ActivityCategory
    {
        public int ActivityCategoryID { get; set; }

        public int ActivityID { get; set; }
        public Activity? Activity { get; set; }

        public int AbilityTypeID { get; set; }
        public AbilityType? AbilityType { get; set; }

        public string? ExclusionType { get; set; }
    }
}
