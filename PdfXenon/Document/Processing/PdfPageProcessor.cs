﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfXenon.Standard
{
    public class PdfPageProcessor : IPdfRendererResolver
    {
        private PdfPage _page;
        private PdfRenderer _renderer;

        public PdfPageProcessor(PdfPage page, PdfRenderer renderer)
        {
            _page = page;
            _renderer = renderer;
            _renderer.Resolver = this;

            // MediaBox is mandatory but CropBox inherits from MediaBox if not present
            PdfRectangle mediaBox = _page.MediaBox;
            PdfRectangle cropBox = _page.CropBox;
            if (cropBox == null)
                cropBox = _page.MediaBox;

            _renderer.Initialize(mediaBox, cropBox);
        }

        public PdfStream GetStream(PdfObjectReference reference)
        {
            return (PdfStream)_page.Document.ResolveReference(reference);
        }

        public PdfDictionary GetGraphicsStateDictionary(string dictName)
        {
            // Page resources should have a dictionary with set of graphics state dictionaries
            PdfDictionary extGStates = _page.Resources.MandatoryValueRef<PdfDictionary>("ExtGState");
            return extGStates.OptionalValueRef<PdfDictionary>(dictName);
        }

        public PdfObject GetColorSpaceObject(string colorSpaceName)
        {
            // Page resources should have a dictionary with set of graphics state dictionaries
            PdfDictionary colorSpaces = _page.Resources.MandatoryValueRef<PdfDictionary>("ColorSpace");
            return colorSpaces.OptionalValueRef<PdfObject>(colorSpaceName);
        }

        public void Process()
        {
            PdfContentsParser parser = _page.Contents.CreateParser();
            PdfObject obj = null;

            do
            {
                obj = parser.GetObject();
                if (obj != null)
                    _renderer.Render(obj);

            } while (obj != null);
        }
    }
}
