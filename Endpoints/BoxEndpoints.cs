using Par.Api.DTOs;
using Par.Api.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.OpenApi;  


namespace Par.Api.Endpoints;

public static class BoxEndpoints
{
    public static RouteGroupBuilder MapBoxEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/boxes")
            .WithTags("Boxes")
            .WithOpenApi();

        
        group.MapGet("/", GetAllBoxes)
            .WithName("GetAllBoxes")
            .WithSummary("Get all boxes with pagination");

        group.MapGet("/{id:int}", GetBoxById)
            .WithName("GetBoxById")
            .WithSummary("Get a box by its ID");

        
        group.MapPost("/", CreateBox)
            .WithName("CreateBox")
            .WithSummary("Create a new box with products");

        group.MapPut("/{id:int}", UpdateBox)
            .WithName("UpdateBox")
            .WithSummary("Update an existing box (replaces all data)");

        
        group.MapPatch("/{id:int}", PatchBox)
            .WithName("PatchBox")
            .WithSummary("Partially update a box (only specified fields)");

        group.MapDelete("/{id:int}", DeleteBox)
            .WithName("DeleteBox")
            .WithSummary("Delete a box and all its products");

        group.MapGet("/export", ExportBoxes)
            .WithName("ExportBoxes")
            .WithSummary("Export all boxes to CSV format");

        group.MapGet("/benchmark", BenchmarkExport)
            .WithName("BenchmarkExport")
            .WithSummary("Run benchmark of the export endpoint");

        return group;
    }

   
    private static async Task<IResult> GetAllBoxes(
        BoxService boxService, 
        int page = 1, 
        int pageSize = 20)
    {
        var boxes = await boxService.GetAllBoxesAsync(page, pageSize);
        return Results.Ok(boxes);
    }

    private static async Task<IResult> GetBoxById(BoxService boxService, int id)
    {
        var box = await boxService.GetBoxByIdAsync(id);
        return box == null 
            ? Results.NotFound(new { message = $"Box with ID {id} not found" })
            : Results.Ok(box);
    }
   
    private static async Task<IResult> CreateBox(
        BoxService boxService, 
        CreateBoxDto createDto)
    {
        try
        {
            var box = await boxService.CreateBoxAsync(createDto);
            return Results.Created($"/api/boxes/{box.Id}", box);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error creating box",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
    
    private static async Task<IResult> UpdateBox(
        BoxService boxService, 
        int id, 
        UpdateBoxDto updateDto)
    {
        try
        {
            var box = await boxService.UpdateBoxAsync(id, updateDto);
            
            if (box == null)
                return Results.NotFound(new { message = $"Box with ID {id} not found" });

            return Results.Ok(box);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error updating box",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

   
    private static async Task<IResult> PatchBox(
        BoxService boxService, 
        int id, 
        PatchBoxDto patchDto)
    {
        try
        {
            var box = await boxService.PatchBoxAsync(id, patchDto);
            
            if (box == null)
                return Results.NotFound(new { message = $"Box with ID {id} not found" });

            return Results.Ok(box);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error patching box",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    private static async Task<IResult> DeleteBox(BoxService boxService, int id)
    {
        try
        {
            var deleted = await boxService.DeleteBoxAsync(id);
            
            if (!deleted)
                return Results.NotFound(new { message = $"Box with ID {id} not found" });

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Error deleting box",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    private static async Task<IResult> ExportBoxes(
        BoxService boxService)
    {
        try
        {
            var stream = await boxService.GetBoxesExportStreamAsync();
            var fileName = $"Boxes_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return Results.Stream(
                stream, 
                contentType: "text/csv", 
                fileDownloadName: fileName,
                enableRangeProcessing: true
            );
        }
        catch (Exception ex)
        {
      
            return Results.Problem(
                title: "Export error",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }

    private static async Task<IResult> BenchmarkExport(
        BoxService boxService)
    {
        try
        {
            int iterations = 5;
            var stopwatch = new Stopwatch();
            var times = new List<long>();

            for (int i = 0; i < iterations; i++)
            {
                stopwatch.Restart();
                await boxService.ExportBoxesToCsvAsync();
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }

            return Results.Ok(new
            {
                TestName = "CSV Export Benchmark (50k records)",
                Iterations = iterations,
                AverageTimeMs = $"{times.Average():F2} ms",
                MinTimeMs = $"{times.Min()} ms",
                MaxTimeMs = $"{times.Max()} ms",
            });
        }
        catch (Exception ex)
        {
          
            return Results.Problem(
                title: "Benchmark error",
                detail: ex.Message,
                statusCode: 500
            );
        }
    }
}