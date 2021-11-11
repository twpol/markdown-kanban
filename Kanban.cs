using System.Collections.Generic;
using Markdig.Syntax;

namespace Markdown_Kanban
{
    class KanbanBoard
    {
        public MarkdownDocument Markdown;

        public List<KanbanList> Lists = new();
    }

    class KanbanList
    {
        public string Id;
        public MarkdownDocument Markdown;

        public List<KanbanCard> Cards;
    }

    class KanbanCard
    {
        public string Id;
        public MarkdownDocument Markdown;
    }
}
