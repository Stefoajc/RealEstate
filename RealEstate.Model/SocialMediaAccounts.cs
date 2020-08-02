namespace RealEstate.Model
{
    public class SocialMediaAccounts
    {
        public int Id { get; set; }
        public string SocialMedia { get; set; }
        public string SocialMediaAccount { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
