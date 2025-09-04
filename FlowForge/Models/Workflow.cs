using System;

namespace FlowForge.Models
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } = "In Progress";
        public string AssignedTo { get; set; }  // email or user
        public string Notes { get; set; }       // ✅ new field for notes
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
    }
}
