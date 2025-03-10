﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaPaymentRedirection {
    [FromQuery(Name = "transaction")]
    public string TransactionId { get; set; }
    [FromQuery(Name = "code")]
    public string OrderCode { get; set; }
    [FromQuery(Name = "lang")]
    public string Lang { get; set; }
    [FromQuery(Name = "eventId")]
    public string EventId { get; set; }
    [FromQuery(Name = "eci")]
    public string Eci { get; set; }
}
