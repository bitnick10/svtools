using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemStore
{
    class Program
    {
        static void Main(string[] args)
        {
            MemStore ms = new MemStore();
            ms.HDDBName = "hdins";
            ms.RAMDBName = "ramins";
            foreach (string id in ms.AllTableNames()) {
               // Console.WriteLine(id);
            }
            ms.Restore();
            while (true) {
                for (int i = 60; i >= 0; i--)
                {
                    Console.Write(i);
                    Console.Write(" ");
                    System.Threading.Thread.Sleep(60 * 1000);
                }
                ms.Persistence();
            }
        }
    }
}
