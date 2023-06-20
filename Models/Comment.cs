using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSweb.Models
{
    public class Comment
    {
        // 評語編號
        [Key]
        public string CommentID { get; set; }
        // 評語內容
        public string CContent { get; set; }

        public virtual CommentType CommentTypes { get; set; }
        public virtual Student Student { get; set; }
    }
}