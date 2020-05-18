using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Server
{
    public partial class NServer
    {
        private NDatabase           _database;


        /// <summary>
        /// Creates a database instance and populates data
        /// </summary>
        /// <returns></returns>
        public bool CreateDatabase()
        {
            _database = new NDatabase();




            return true;
        }
    }
}
