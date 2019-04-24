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
            ms.HDDBName = "hdxx";
            ms.RAMDBName = "ramxx";
            foreach (string id in ms.AllTableNames()) {
               // Console.WriteLine(id);
            }
            ms.Restore();
            while (true) {
                for (int i = 60; i >= 0; i--)
                {
                    System.Threading.Thread.Sleep(60 * 1000);
                    Console.Write(i);
                    Console.Write(" ");
                }
                ms.Persistence();
            }
        }
    }
}
