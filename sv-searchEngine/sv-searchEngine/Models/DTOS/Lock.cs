namespace sv_searchEngine.Models.DTOS
{
    public class Lock
    {
        public string Id { get; set; }
        public string BuildingId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public string SerialNumber { get; set; }
        public string Floor { get; set; }
        public string RoomNumber { get; set; }
    }
}
