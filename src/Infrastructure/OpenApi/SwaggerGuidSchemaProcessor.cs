﻿using NJsonSchema.Generation;
using NJsonSchema;

namespace TD.WebApi.Infrastructure.OpenApi;
public class SwaggerGuidSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var type = context.ContextualType;
        var schema = context.Schema;

        // Check if the type is a Guid
        if (type == typeof(Guid))
        {
            schema.Type = JsonObjectType.String;
            schema.Format = "uuid";
        }
    }
}