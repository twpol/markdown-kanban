using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Markdown_Kanban
{
    class Program
    {
        static void Main(string directory = ".")
        {
            Console.WriteLine($"Building Kanban from <{directory}>...");
            var board = LoadKanbanBoard(directory);
            SaveKanbanBoard(Path.Combine(directory, "index.html"), board);
        }

        static MarkdownDocument Load(params string[] paths)
        {
            try
            {
                return Markdown.Parse(File.ReadAllText(Path.Combine(paths)));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Potentially missing file: {Path.Combine(paths)}");
                return Markdown.Parse("");
            }
        }

        static void WriteText(TextWriter writer, params MarkdownObject[] markdown)
        {
            var renderer = new HtmlRenderer(writer)
            {
                EnableHtmlForBlock = false,
                EnableHtmlForInline = false,
                EnableHtmlEscape = false,
            };
            foreach (var obj in markdown) renderer.Render(obj);
            writer.Flush();
        }

        static void WriteHtml(TextWriter writer, params MarkdownObject[] markdown)
        {
            var renderer = new HtmlRenderer(writer);
            foreach (var obj in markdown) renderer.Render(obj);
            writer.Flush();
        }

        static KanbanBoard LoadKanbanBoard(string directory)
        {
            return new KanbanBoard()
            {
                Markdown = Load(directory, "index.md"),
                Lists = Directory.GetDirectories(directory).Select(LoadKanbanList).ToList(),
            };
        }

        static KanbanList LoadKanbanList(string directory)
        {
            return new KanbanList()
            {
                Id = Path.GetFileName(directory),
                Markdown = Load(directory, "index.md"),
                Cards = Directory.GetFiles(directory).Select(LoadKanbanCard).ToList(),
            };
        }

        static KanbanCard LoadKanbanCard(string file)
        {
            return new KanbanCard()
            {
                Id = Path.GetFileNameWithoutExtension(file),
                Markdown = Load(file),
            };
        }

        static MarkdownObject GetTitle(MarkdownDocument markdown)
        {
            return markdown.First(obj => obj is HeadingBlock && (obj as HeadingBlock).Level == 1);
        }

        static MarkdownObject[] GetBody(MarkdownDocument markdown)
        {
            return markdown.Where(obj => !(obj is HeadingBlock) || (obj as HeadingBlock).Level > 1).ToArray();
        }

        static void SaveKanbanBoard(string file, KanbanBoard board)
        {
            using var stream = File.Create(file);
            using var writer = new StreamWriter(stream);
            writer.WriteLine(@"<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"" crossorigin=""anonymous"">
    <title>");
            WriteText(writer, GetTitle(board.Markdown));
            writer.WriteLine(@"</title>
  </head>
  <body>
    <div class=container-fluid>");
            WriteHtml(writer, GetTitle(board.Markdown));
            WriteHtml(writer, GetBody(board.Markdown));
            writer.WriteLine(@"
    </div>
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"" integrity=""sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p"" crossorigin=""anonymous""></script>
  </body>
</html>");
        }
    }
}
