using System.Text.Json;
using RulesEngine.Models;
using FitRank.API.Application.Rutinas.Abstractions;

namespace FitRank.API.Infrastructure.RulesEngineImpl
{
    public sealed class RulesEvaluator : IRulesEvaluator
    {
        private readonly RulesEngine.RulesEngine _re;
        private readonly string[] _workflowNames;

        public RulesEvaluator(IHostEnvironment env)
        {
            var basePath = Path.Combine(env.ContentRootPath, "Infrastructure", "Rules");
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta de reglas: {basePath}");

            var files = Directory.GetFiles(basePath, "*.json", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new InvalidOperationException($"No se encontraron archivos *.json en {basePath}");

            var workflows = new List<Workflow>();
            foreach (var f in files)
            {
                var txt = File.ReadAllText(f);
                // valida json:
                using var _ = JsonDocument.Parse(txt);
                var arr = JsonSerializer.Deserialize<List<Workflow>>(txt);
                if (arr is not null) workflows.AddRange(arr);
            }

            _re = new RulesEngine.RulesEngine(workflows.ToArray());
            _workflowNames = workflows.Select(w => w.WorkflowName).Distinct().ToArray();

            // Debug útil:
            Console.WriteLine("Workflows cargados: " + string.Join(", ", _workflowNames));
        }

        public async Task<IReadOnlyCollection<string>> EvaluateAsync(string workflowName, object input)
        {
            var results = await _re.ExecuteAllRulesAsync(workflowName, new RuleParameter("input", input));
            return results.Where(r => r.IsSuccess)
                          .SelectMany(r => (r.Rule.SuccessEvent ?? "")
                              .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                          .Distinct()
                          .ToArray();
        }

        public async Task<IDictionary<string, IReadOnlyCollection<string>>> EvaluateAllAsync(object input)
        {
            var map = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var wf in _workflowNames)
            {
                var tags = await EvaluateAsync(wf, input);
                map[wf] = tags;
                Console.WriteLine($"{wf}: {string.Join(", ", tags)}"); // debug
            }
            return map;
        }
    }
}
