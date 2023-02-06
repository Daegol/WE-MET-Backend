using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Concrete
{
    public enum RedisDatabaseNumber
    {
        /// Database for caching
        Cache = 1,
        /// Database for plugins
        Plugin = 2,
        /// Database for data protection keys
        DataProtectionKeys = 3
    }
}
