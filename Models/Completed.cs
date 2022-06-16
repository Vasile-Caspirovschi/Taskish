using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskish.Models
{
    public class Completed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompletedTaskId { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime Expire { get; set; }

        //set foreign key
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
