using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMap.Entities
{
    public class PersonEntity: IEquatable<PersonEntity>
    {
        [Column("UserId")]
        public string UserId { get; set; }

        [Column("Your name")]
        public string FullName { get; set; }

        [Column("Child (children) name and date of birth")]
        public string Children { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("Instagram")]
        public string Instagram { get; set; }

        [Column("Email Address")]
        public string EmailAddress { get; set; }

        [Column("Telephone number")]
        public string TelephoneNumber { get; set; }

        [Column("ExternalId")]
        public int ExternalId { get; set; }

        [Column("Timestamp")]
        public string Timestamp { get; set; }

        public bool Equals(PersonEntity other)
        {
            return UserId == other.UserId
                && FullName == other.FullName
                && Children == other.Children
                && Location == other.Location
                && Instagram == other.Instagram
                && EmailAddress == other.EmailAddress
                && TelephoneNumber == other.TelephoneNumber
                && ExternalId == other.ExternalId;
        }
    }
}
