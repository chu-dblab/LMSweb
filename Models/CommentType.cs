using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class CommentType
    {
        // 評論類型編號
        [Key]
        public string CommentTypeID { get; set; }
        // 評論類型内容
        public string CTContent { get; set; }
        // 評論類型屬性
        public string CTAttribute { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}