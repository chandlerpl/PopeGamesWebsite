using ColorCode.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Markdig.SyntaxHighlighting
{
    public static class SyntaxHighlightingExtensions
    {
        public static MarkdownPipelineBuilder UseSyntaxHighlighting(this MarkdownPipelineBuilder pipeline, StyleDictionary customCss = null)
        {
            pipeline.Extensions.Add(new SyntaxHighlightingExtension());
            return pipeline;
        }
    }
}
