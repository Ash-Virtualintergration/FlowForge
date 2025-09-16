using FlowForge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace FlowForge.Services
{
    public static class JsonDataService
    {
        private static string BaseFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FlowForge");


        private static string WorkflowsPath => Path.Combine(BaseFolder, "workflows.json");


        public static void SaveWorkflows(List<Workflow> workflows)
        {
            Directory.CreateDirectory(BaseFolder);
            var json = JsonConvert.SerializeObject(workflows, Formatting.Indented);
            File.WriteAllText(WorkflowsPath, json);
        }


        public static List<Workflow> LoadWorkflows()
        {
            try
            {
                if (!File.Exists(WorkflowsPath)) return new List<Workflow>();
                var json = File.ReadAllText(WorkflowsPath);
                var list = JsonConvert.DeserializeObject<List<Workflow>>(json);
                return list ?? new List<Workflow>();
            }
            catch
            {
                return new List<Workflow>();
            }
        }
    }
}