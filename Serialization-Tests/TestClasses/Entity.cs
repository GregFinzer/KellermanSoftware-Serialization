using System;
using System.Collections.Generic;

namespace SerializationTests.TestClasses
{
    public enum Level : int 
    {
        Company,
        Division,
        Department
    }

    [Serializable]
    public class Entity
    {
        private List<Entity> _children = new List<Entity>();

        public long EntityId
        {
            get; set;
        }

        public string Description
        {
            get;
            set;
        }

        public Level EntityLevel
        {
            get;
            set;
        }

        public Entity Parent
        {
            get;
            set;
        }

        public List<Entity> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}
