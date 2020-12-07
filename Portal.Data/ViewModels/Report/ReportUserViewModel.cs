using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Portal.Data.ViewModels.Report
{
    public class ReportUserViewModel
    {
        [Display(Name = "Employee")]
        [Required]
        public Guid Id { get; set; }
        public List<Portal.Data.Entities.User> Users { get; set; }
    }
}
