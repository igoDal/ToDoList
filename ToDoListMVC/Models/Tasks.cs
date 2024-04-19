using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListMVC.Models;

public class Tasks
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    [DefaultValue(false)]
    public bool IsDone { get; set; }
}