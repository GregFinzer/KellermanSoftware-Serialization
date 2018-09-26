using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SerializationTests.TestClasses
{
    public class Goal
    {
        public string Name { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }
    }
}
