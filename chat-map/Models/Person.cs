using System;
using System.Collections.Generic;

namespace ChatMap.Models
{
    public class PersonModel: BaseModel
    {
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public List<ChildModel> Children { get; set; }
        public string Address { get; set; }
        public string Instagram { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
