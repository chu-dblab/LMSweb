using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMSweb.ViewModel
{
    [MetadataType(typeof(MissionCreateViewModel))]
    public class MissionCreateViewModel
    {
        [Required]
        public string CID { get; set; }
        public string CName { get; set; }

        public IEnumerable<SelectListItem> KnowledgeList { get; set; }


        [Required]
        [Display(Name ="知識點")]
        public IEnumerable<int> SelectKnowledgeList { get; set; }

        [Required]
        public LMSweb.Models.Mission Mission { get; set; }

    }
}