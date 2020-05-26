using ColorCode;
using ColorCode.Styling;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System;
using System.IO;
using System.Text;

namespace Markdig.SyntaxHighlighting
{
    class SyntaxHighlightingCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private readonly CodeBlockRenderer _underlyingRenderer;
        private readonly HtmlClassFormatter formatter;

        public SyntaxHighlightingCodeBlockRenderer(CodeBlockRenderer underlyingRenderer = null)
        {
            _underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();
            formatter = new HtmlClassFormatter();
        }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            FencedCodeBlock fencedCodeBlock = obj as FencedCodeBlock;
            FencedCodeBlockParser parser = obj.Parser as FencedCodeBlockParser;
            if (fencedCodeBlock == null || parser == null)
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            var languageMoniker = fencedCodeBlock.Info.Replace(parser.InfoPrefix, string.Empty);
            if (string.IsNullOrEmpty(languageMoniker))
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            string firstLine;
            var code = GetCode(obj, out firstLine);

            Console.WriteLine(formatter.GetCSSString());
            renderer.WriteLine(formatter.GetHtmlString(code, Languages.FindById(languageMoniker)));
        }

        private static string GetCode(LeafBlock obj, out string firstLine)
        {
            var code = new StringBuilder();
            firstLine = null;
            foreach (var line in obj.Lines.Lines)
            {
                var slice = line.Slice;
                if (slice.Text == null)
                {
                    continue;
                }

                var lineText = slice.Text.Substring(slice.Start, slice.Length);

                if (firstLine == null)
                {
                    firstLine = lineText;
                }
                else
                {
                    code.AppendLine();
                }

                code.Append(lineText);
            }
            return code.ToString();
        }
    }
}
