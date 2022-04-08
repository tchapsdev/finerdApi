﻿namespace Finerd.Api.Model.Entities
{
    public class RefreshToken: EntityKey
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public string TokenSalt { get; set; }
        public DateTime Ts { get; set; }
        public DateTime ExpiryDate { get; set; }
        public virtual User User { get; set; }
    }
}
