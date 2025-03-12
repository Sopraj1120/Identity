namespace IdentityPoddle.Entity
{
    public class OtpRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Otp { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; }
    }
}
