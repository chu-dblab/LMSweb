using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class User
    {
        [Key]
        public string ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UPassword { get; set; } //因為Password在T-SQL內是個關鍵字所以在前面加個U來表示
        [Required]
        public string Gender { get; set; }
        [Required]
        public string RoleName { get; set; } //因為Role在T-SQL內是個關鍵字所以在後方補上Name
    }
}