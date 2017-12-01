using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanExploringLocalToDB
{
    public class ObjectInfo
    {
        public  int id { get; set; }
        public string Name { get; set; } = "";
        public string MapsURL { get; set; } = "";
        public string Info { get; set; } = "";
        public string Notes { get; set; } = "";
        public string MapsPhoto { get; set; } = "";
        public string TopoPhoto { get; set; } = "";
        public string SatellitePhoto { get; set; } = "";
        public DataTable Plan { get; set; }
        public List<string> Photos { get; set; }
        public List<string> Streetview { get; set; }
    }
}
