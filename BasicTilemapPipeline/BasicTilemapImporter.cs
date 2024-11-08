using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BasicTilemapPipeline
{
    /// <summary>
    /// An importer for a basic tilemap file. The purpose of an importer to to load all important data 
    /// from a file into a content object; any processing of that data occurs in the subsequent content
    /// processor step. 
    /// </summary>
    [ContentImporter(".tmap", DisplayName = "BasicTilemapImporter", DefaultProcessor = "BasicTilemapProcessor")]
    public class BasicTilemapImporter : ContentImporter<BasicTilemapContent>
    {
        public override BasicTilemapContent Import(string filename, ContentImporterContext context)
        {
            // Create a new BasicTilemapContent
            BasicTilemapContent map = new();

            // Read in the map file and split along newlines 
            string data = File.ReadAllText(filename);
            var lines = data.Split('\n');

            // First line in the map file is the image file name,
            // we store it so it can be loaded in the processor
            map.TilesetImageFilename = lines[0].Trim();

            // Second line is the tileset image size
            var secondLine = lines[1].Split(',');
            map.TileWidth = int.Parse(secondLine[0]);
            map.TileHeight = int.Parse(secondLine[1]);

            // Third line is the map size (in tiles)
            var thirdLine = lines[2].Split(',');
            map.MapWidth = int.Parse(thirdLine[0]);
            map.MapHeight = int.Parse(thirdLine[1]);

            // Fourth line is the map data (the indices of tiles in the map)
            // We can use the Linq Select() method to convert the array of strings
            // into an array of ints
            map.TileIndices = lines[3].Split(',').Select(index => int.Parse(index)).ToArray();

            // At this point, we've copied all of the file data into our
            // BasicTilemapContent object, so we pass it on to the processor
            return map;
        }
    }
}