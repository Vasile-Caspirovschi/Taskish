using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskish.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        //set foreign keys
        public ICollection<Task> Tasks { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
