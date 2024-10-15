using Microsoft.AspNetCore.Mvc;
using LaChasseAuTresor_ISY.Services;
using LaChasseAuTresor_ISY.Models;
using System.IO;
using System.Threading.Tasks;

namespace LaChasseAuTresor_ISY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly InputParserService _inputParserService;
        private readonly GameEngine _gameEngine;

        public MapController(InputParserService inputParserService, GameEngine gameEngine)
        {
            _inputParserService = inputParserService;
            _gameEngine = gameEngine;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                Map map = await Task.Run(() => _inputParserService.ParseInputFile(file));
                return Ok(map); //Return the map
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("run-game")]
        public async Task<IActionResult> RunGame([FromBody] Map map)
        {
            if (map == null)
            {
                // Log the error to see if map is null or incorrect
                Console.WriteLine("Received null or invalid map data.");
                return BadRequest("Invalid map data.");
            }

            try
            {
                Console.WriteLine("Received map data: ");
                Console.WriteLine($"Map width: {map.Width}, height: {map.Height}");
                // Log for debugging

                // Initialize GameEngine with the parsed map
                var gameEngine = new GameEngine(map);
                gameEngine.RunGameTurns(); // Play the game with given Map

                // Define output file path
                var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "OutputFiles");
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                var outputFilePath = Path.Combine(outputDirectory, "output.txt");
                gameEngine.WriteOutputFile(outputFilePath); // Write output to a file

                // Return the file path or contents as a response
                return Ok(new { OutputFilePath = outputFilePath });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("download-output")]
        public IActionResult DownloadOutput(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound("Output file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", "output.txt");
        }
    }
}
