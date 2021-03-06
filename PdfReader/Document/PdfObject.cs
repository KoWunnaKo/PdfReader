﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PdfReader
{
    public abstract class PdfObject
    {
        public PdfObject(PdfObject parent)
            : this(parent, null)
        {
        }

        public PdfObject(PdfObject parent, ParseObjectBase parse)
        {
            Parent = parent;
            ParseObject = parse;
        }

        public override string ToString()
        {
            return $"({GetType().Name})";
        }

        public virtual void Visit(IPdfObjectVisitor visitor)
        {
            visitor.Visit(this);
        }

        public PdfObject Parent { get; private set; }
        public ParseObjectBase ParseObject { get; private set; }
        public PdfDocument Document { get => TypedParent<PdfDocument>(); }
        public PdfDecrypt Decrypt { get => TypedParent<PdfDocument>().DecryptHandler; }

        public T TypedParent<T>() where T : PdfObject
        {
            PdfObject parent = Parent;

            while (parent != null)
            {
                if (parent is T)
                    return parent as T;
                else
                    parent = parent.Parent;
            }

            return null;
        }

        public bool AsBoolean()
        {
            if (this is PdfBoolean boolean)
                return boolean.Value;

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected a boolean.");
        }

        public string AsString()
        {
            if (this is PdfName name)
                return name.Value;
            else if (this is PdfString str)
                return str.Value;

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected a string.");
        }

        public int AsInteger()
        {
            if (this is PdfInteger integer)
                return integer.Value;

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected an integer.");
        }

        public float AsNumber()
        {
            if (this is PdfInteger integer)
                return integer.Value;
            else if (this is PdfReal real)
                return real.Value;

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected a number.");
        }

        public float[] AsNumberArray()
        {
            if (this is PdfArray array)
            {
                List<float> numbers = new List<float>();
                foreach (PdfObject item in array.Objects)
                {
                    if (item is PdfInteger integer)
                        numbers.Add(integer.Value);
                    else if (item is PdfReal real)
                        numbers.Add(real.Value);
                    else
                        throw new ApplicationException($"Array contains object of type '{GetType().Name}', expected only numbers.");

                }

                return numbers.ToArray();
            }

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected an integer array.");
        }

        public List<PdfObject> AsArray()
        {
            if (this is PdfArray array)
                return array.Objects;

            throw new ApplicationException($"Unexpected object in content '{GetType().Name}', expected an integer array.");
        }

        public PdfObject WrapObject(ParseObjectBase obj)
        {
            if (obj is ParseString str)
                return new PdfString(this, str);
            if (obj is ParseName name)
                return new PdfName(this, name);
            else if (obj is ParseInteger integer)
                return new PdfInteger(this, integer);
            else if (obj is ParseReal real)
                return new PdfReal(this, real);
            else if (obj is ParseDictionary dictionary)
                return new PdfDictionary(this, dictionary);
            else if (obj is ParseObjectReference reference)
                return new PdfObjectReference(this, reference);
            else if (obj is ParseStream stream)
                return new PdfStream(this, stream);
            else if (obj is ParseArray array)
                return new PdfArray(this, array);
            else if (obj is ParseIdentifier identifier)
                return new PdfIdentifier(this, identifier);
            else if (obj is ParseBoolean boolean)
                return new PdfBoolean(this, boolean);
            if (obj is ParseNull nul)
                return new PdfNull(this);

            throw new ApplicationException($"Cannot wrap object '{obj.GetType().Name}' as a pdf object .");
        }

        public static PdfRectangle ArrayToRectangle(PdfArray array)
        {
            if (array != null)
                return new PdfRectangle(array.Parent, array.ParseArray);
            else
                return null;
        }
    }
}
