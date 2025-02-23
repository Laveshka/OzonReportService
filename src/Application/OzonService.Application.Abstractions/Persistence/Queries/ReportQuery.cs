using SourceKit.Generators.Builder.Annotations;

namespace OzonService.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public sealed partial record ReportQuery(long[] Ids);