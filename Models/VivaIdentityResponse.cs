using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaIdentityResponse {
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    public string Scope { get; set; }
}
