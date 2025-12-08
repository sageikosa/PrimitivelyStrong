using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SampleUse;

public class SampleTransaction
{
    [Key]
    public TransactionID TransactionID { get; set; }
    public NameString Name { get; set; }
}
