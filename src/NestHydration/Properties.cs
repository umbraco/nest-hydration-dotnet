﻿using System.Collections.Generic;

namespace NestHydration
{
    /// <summary>
    /// List of Properties
    /// </summary>
    public class Properties : List<IProperty>
    {
        /// <summary>
        /// 
        /// </summary>
        public Properties()
        {
        }

        public Properties(List<IProperty> list)
            : base((IEnumerable<IProperty>)list)
        {
        }
    }
}
