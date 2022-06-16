using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskish.Models
{
    public class Deleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeletedTaskId { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime Expire { get; set; }

        //set foreign key
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
