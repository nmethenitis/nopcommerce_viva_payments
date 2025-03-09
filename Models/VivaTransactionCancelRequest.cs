using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaTransactionCancelRequest {
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string SourceCode { get; set; }
}
