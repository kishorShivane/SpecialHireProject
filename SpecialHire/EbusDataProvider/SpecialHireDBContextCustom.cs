using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace EbusDataProvider
{
    public partial class SpecialHireDBContext : DbContext
    {
        public SpecialHireDBContext(string connectionString)
        : base("name = " + connectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    }
}
