﻿using System;
using System.Text;

namespace PdfXenon.Standard
{
    public class PdfDateTime : PdfString
    {
        public PdfDateTime(PdfObject parent, PdfString str)
            : base(parent, str.ParseObject as ParseString)
        {
            DateTime = str.ValueAsDateTime;
        }

        public override string ToString()
        {
            return $"PdfDateTime {DateTime}";
        }

        public override int ToDebug(StringBuilder sb, int indent)
        {
            string output = DateTime.ToString();
            sb.Append(output);
            return indent + output.Length;
        }

        public DateTime DateTime { get; private set; }
    }
}