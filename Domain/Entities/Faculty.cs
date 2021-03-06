﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public class Faculty : Department
    {
        [JsonIgnore]
        public virtual School School { get; set; }

        public virtual string SchoolName { get; set; }

        [JsonIgnore]
        public virtual ISet<Profession> professionList
        {
            set;
            get;
        }
    }
}
