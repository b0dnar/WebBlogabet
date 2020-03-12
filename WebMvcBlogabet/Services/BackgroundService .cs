using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebMvcBlogabet.Services
{
    public class BackgroundService : IHostedService, IDisposable
    {
        private BlogabetParser _parser;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _parser = new BlogabetParser(cancellationToken);
            await _parser.Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
           
        }

        public virtual void Dispose()
        {
           // _stoppingCts.Cancel();
        }
    }
}
