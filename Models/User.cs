using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskish.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        public string Question { get; set; }
        public string QuestionAnswer { get; set; }
        public DateTime RegisteredOn { get; set; } = DateTime.Now;
        public string? ResetPasswordToken { get; set; }
        public DateTime? TokenSentTime { get; set; }
        public bool EmailWasSent { get; set; }

        //set foreign keys
        public ICollection<Task>? Tasks { get; set; }
        public ICollection<Category>? Categories { get; set; }

        [NotMapped]
        public double LoggedIn
        {
            get
            {
                return (DateTime.Now.Date - RegisteredOn.Date).TotalDays;
            }
        }
    }
}
