using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanExploringLocalToDB
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadLocal localToDB = new ReadLocal();
            localToDB.GetFolders();
        }
    }
}
