using Application.API.Formulas.Configure;

namespace Web.API.Requests.Formulas;

internal sealed record ConfigureRequest(string Name, Dictionary<string, FieldConfiguration> Fields);
