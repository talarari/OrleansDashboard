using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Core.Options;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Tagging;
using Microsoft.AspNetCore.Mvc;

namespace OrleansDashboard
{
    [Route("/metrics")]
    public class MetricsController: ControllerBase
    {
        private readonly IMetrics _metrics;

        public MetricsController(IMetrics metrics)
        {
            _metrics = metrics;
        }

        [HttpGet]
        public async Task Index()
        {
            _metrics.Measure.Gauge.SetValue(new GaugeOptions()
                {
                    Name = "test",
                    MeasurementUnit = Unit.None,
                    Context = "myapp",
                    Tags = MetricTags.Empty
                },() => 10);

        var writer = new PrometheusPlainTextMetricsWriter();
        await writer.WriteAsync(HttpContext, _metrics.Snapshot.Get());
    }
    }
}
