using System;
using System.Collections.Generic;
using System.Text;

namespace EntityCreater.Utility
{
    public static class StringBuilderExtend
    {
        public static void AppendSpaceLine(this StringBuilder stringBuilder, string str, int moveRight = 0,
            int moveDown = 1)
        {
            for (int i = 0; i < moveDown; i++)
            {
                stringBuilder.Append("\r\n");
            }

            for (int i = 0; i < moveRight; i++)
            {
                stringBuilder.Append(" ");
            }

            stringBuilder.Append(str);
        }
    }
}