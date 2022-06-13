using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskish.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public byte Priority { get; set; }

        //set foreign key from User entity
        public int UserId { get; set; }
        public User User { get; set; }

        //set foreign key from Category entity
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Completed Completed { get; set; }
        public Deleted Deleted { get; set; }

        [NotMapped]
        public string Status
        {
            get => IsCompleted ? "Completed" : "Pending";
        }
        [NotMapped]
        public string IsPassed
        {
            get => DueDate < DateOnly.FromDateTime(DateTime.Now) ? "Has passed" : "Hasn't passed";
        }

        public enum TaskPriorities
        {
            High = 3,
            Medium = 2,
            Low = 1,
            None = 0
        }
    }
}
