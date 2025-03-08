using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.VivaPayments.Services;

namespace Nop.Plugin.Payments.VivaPayments.Infrastracture;
internal class NopStartup : INopStartup {
    public int Order => 1;

    public void Configure(IApplicationBuilder application) {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        services.AddScoped<VivaApiService>();
    }
}
