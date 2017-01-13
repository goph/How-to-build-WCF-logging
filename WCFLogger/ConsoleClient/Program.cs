using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ServiceReference.Service1Client();
          var result=   service.GetData(10);
        }
    }
}
