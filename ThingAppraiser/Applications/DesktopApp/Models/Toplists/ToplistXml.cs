using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    [Serializable]
    [XmlRoot(ElementName = "Toplist", Namespace = "")]
    public class ToplistXml
    {
        [XmlAttribute(DataType = "string")]
        public string Name { get; set; }

        [XmlAttribute(DataType = "int")]
        public int Type { get; set; }

        [XmlAttribute(DataType = "int")]
        public int Format { get; set; }

        [XmlAnyElement(Name = "Blocks")]
        public XElement Blocks { get; set; }

        public ToplistXml()
        {
        }

        internal ToplistXml(ToplistBase toplist)
        {
            toplist.ThrowIfNull(nameof(toplist));

            Name = toplist.Name;
            Type = (int) toplist.Type;
            Format = (int) toplist.Format;

            Blocks = ConvertBlocksToXElements(toplist.Blocks);
        }

        internal IReadOnlyList<ToplistBlock> ConvertXElementsToBlocks()
        {
            IReadOnlyList<XElement> xmlBlocks = XDocumentParser.FindSubelements(Blocks).ToList();

            var blocks = new List<ToplistBlock>(xmlBlocks.Count);
            foreach (XElement xmlBlock in xmlBlocks)
            {
                blocks.Add(ConvertXElementToBlock(xmlBlock));
            }

            return blocks;
        }

        private static XElement ConvertBlocksToXElements(
            IReadOnlyCollection<ToplistBlock> toplistBlocks)
        {
            toplistBlocks.ThrowIfNull(nameof(toplistBlocks));

            var xmlBlocks = new List<XElement>(toplistBlocks.Count);
            foreach (ToplistBlock block in toplistBlocks)
            {
                xmlBlocks.Add(ConvertBlockToXElement(block));
            }

            return new XElement("Blocks", xmlBlocks.ToArray());
        }

        private static XElement ConvertBlockToXElement(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            return new XElement("Block",
                new XAttribute(nameof(ToplistBlock.Title), block.Title),
                new XAttribute(nameof(ToplistBlock.Number), block.Number),

                new XElement(nameof(block.Items),
                    block.Items.Select(ConvertItemToXElement).ToArray()
                )
            );
        }

        private static XElement ConvertItemToXElement(ToplistItem item)
        {
            item.ThrowIfNull(nameof(item));

            return new XElement("Item",
                new XAttribute(nameof(ToplistItem.Name), item.Name),
                new XAttribute(nameof(ToplistItem.Position),
                               item.Position.GetValueOrDefault(-1))
            );
        }

        private static ToplistBlock ConvertXElementToBlock(XElement xmlBlock)
        {
            xmlBlock.ThrowIfNull(nameof(xmlBlock));

            string title = XDocumentParser.GetAttributeValue(xmlBlock, nameof(ToplistBlock.Title));
            int number = XDocumentParser.GetAttributeValue<int>(xmlBlock,
                                                                nameof(ToplistBlock.Number));

            XElement itemsXml = XDocumentParser.FindSubelement(xmlBlock,
                                                               nameof(ToplistBlock.Items));

            var result = new ToplistBlock(title, number);
            result.UpdateItems(
                XDocumentParser.FindSubelements(itemsXml)
                    .Select(xmlItem => ConvertXElementToItem(xmlItem, result))
                    .ToList()
            );

            return result;
        }

        private static ToplistItem ConvertXElementToItem(XElement xmlItem, ToplistBlock parentBlock)
        {
            xmlItem.ThrowIfNull(nameof(xmlItem));

            string name = XDocumentParser.GetAttributeValue(xmlItem, nameof(ToplistItem.Name));
            int parsedPosition = XDocumentParser.GetAttributeValue<int>(
                xmlItem, nameof(ToplistItem.Position)
            );
            int? position = parsedPosition == -1 ? (int?) null : parsedPosition;

            return new ToplistItem(name, position, parentBlock);
        }
    }
}
