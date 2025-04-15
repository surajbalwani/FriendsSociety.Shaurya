namespace FriendsSociety.Shaurya.Entities
{
    public class GroundAllocation
    {
        public int GroundAllocationID { get; set; }

        public int GroundID { get; set; }
        public Ground? Ground { get; set; }

        public int ActivityID { get; set; }
        public Activity? Activity { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
