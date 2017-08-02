using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parsing_the_json
{

    public class Rootobject
    {
        public QueryLabel[] Queries { get; set; }
    }

    public class QueryLabel
    {
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public object[] entities { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

}
