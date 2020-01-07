using System;
using System.Collections.Generic;
using System.Text;

namespace PrefabInfoExporter
{
    public static class Util
    {
        public static string Indent(this string textToIndent, int len)
        {
            string indent = new string('\t', len);
            return indent + textToIndent.Replace("\n", "\n" + indent);
        }

        public static string CreateCategory(string categoryName, string categoryContents, int indentBy = 0)
        {
            StringBuilder category = new StringBuilder(categoryName.Trim().Indent(indentBy));

            category.AppendLine("{".Indent(indentBy));
            category.AppendLine(categoryContents.Trim().Indent(indentBy + 1));
            category.AppendLine("}".Indent(indentBy));

            return category.ToString();
        }
    }
}
