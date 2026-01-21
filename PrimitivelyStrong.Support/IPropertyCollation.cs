using System;
using System.Collections.Generic;
using System.Text;

namespace PrimitivelyStrong.Support;

public interface IPropertyCollation : IPropertyConfigurator
{
    bool IsCaseSensitive { get; }
    string Collation { get; set; }
}
