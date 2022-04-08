namespace Finerd.Api.Model.Entities
{
    public class User: EntityKey
    {
        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
            Transactions = new HashSet<Transaction>();
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
