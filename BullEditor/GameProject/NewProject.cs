﻿using BullEditor.Common;
using BullEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BullEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath {  get; set; }
        public string ScreenshotFilePath { get; set;}
        public string ProjectFilePath {  get; set; }

    }
    internal class NewProject : ViewModelBase
    {
        //TODO: get path from installation location
        private readonly string _templatePath = @"..\..\BullEditor\ProjectTemplates\";
        private string _projectName = "New Project";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged(nameof(_projectName));
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments) }\BullProject\"; 
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }
            path += $@"{ProjectName}\";
            //Test
            return true;
        }
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();  
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        {
            get;
        }
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                //Should have one template for a project                
                Debug.Assert(templatesFiles.Any());
                foreach (var templatesFile in templatesFiles)
                {
                    /*
                    var template = new ProjectTemplate()
                    {
                        ProjectType = "Empty Project",
                        ProjectFile = "project.bull",
                        Folders = new List<string> { ".Bull", "Content", "GameCode" }
                    };
                    Serializer.ToFile(template, templatesFile);
                    */
                    var template = Serializer.FromFile<ProjectTemplate>(templatesFile);
                    template.IconFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templatesFile), "icon.png"));
                    template.ScreenshotFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templatesFile), "screen.jpg"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = System.IO.Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templatesFile), template.ProjectFile));
                    _projectTemplates.Add(template);
                }
            }
            
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO: log error
            }


        }
    }
    
}
