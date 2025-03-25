using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaPaymentRedirection {
    [FromQuery(Name = "t")]
    public string TransactionId { get; set; }
    [FromQuery(Name = "s")]
    public string OrderCode { get; set; }
    [FromQuery(Name = "lang")]
    public string Lang { get; set; }
    [FromQuery(Name = "eventId")]
    public string EventId { get; set; }
    [FromQuery(Name = "eci")]
    public string Eci { get; set; }
    [FromQuery(Name ="cancel")]
    public string Cancel { get; set; }
    public bool IsCancelled {
        get {
            return Cancel == "1";
        }
    }
}
