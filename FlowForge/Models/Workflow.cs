using System;
using System.Collections.Generic;

namespace FlowForge.Models
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }   // ✅ New: user email or username
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public List<string> Notes { get; set; } = new List<string>();        // ✅ New: notes history
        
    }
}
