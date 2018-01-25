using System;

namespace PFSign.Models
{
    public class Record
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime SignInTime { get; set; }
        public DateTime? SignOutTime { get; set; }
        public int Seat { get; set; }
    }
}