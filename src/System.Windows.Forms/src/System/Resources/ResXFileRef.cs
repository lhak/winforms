// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Resources {
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.Windows.Forms;
    using System.Reflection;
    using Microsoft.Win32;
    using System.Drawing;
    using System.IO;
    using System.ComponentModel;
    using System.Collections;
    using System.Resources;
    using System.Text;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Runtime.Versioning;

    /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef"]/*' />
    /// <devdoc>
    ///     ResX File Reference class. This allows the developer to represent
    ///     a link to an external resource. When the resource manager asks
    ///     for the value of the resource item, the external resource is loaded.
    /// </devdoc>
    [TypeConverterAttribute(typeof(ResXFileRef.Converter)), Serializable]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]

    public class ResXFileRef {
        private string fileName;
        private string typeName;
        [OptionalField(VersionAdded = 2)]
        private Encoding textFileEncoding;

        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.ResXFileRef"]/*' />
        /// <devdoc>
        ///     Creates a new ResXFileRef that points to the specified file.
        ///     The type refered to by typeName must support a constructor
        ///     that accepts a System.IO.Stream as a parameter.
        /// </devdoc>
        public ResXFileRef(string fileName, string typeName) {
            if(fileName == null) {
                throw (new ArgumentNullException(nameof(fileName)));
            }
            if(typeName == null) {
                throw (new ArgumentNullException(nameof(typeName)));
            }
            this.fileName = fileName;
            this.typeName = typeName;
        }

        
        [OnDeserializing]      
        private void OnDeserializing(StreamingContext ctx) {
            textFileEncoding = null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMethodsAsStatic")]
        [OnDeserialized]      
        private void OnDeserialized(StreamingContext ctx) {
        }

        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.ResXFileRef"]/*' />
        /// <devdoc>
        ///     Creates a new ResXFileRef that points to the specified file.
        ///     The type refered to by typeName must support a constructor
        ///     that accepts a System.IO.Stream as a parameter.
        /// </devdoc>
        public ResXFileRef(string fileName, string typeName, Encoding textFileEncoding) : this(fileName, typeName)  {
            this.textFileEncoding = textFileEncoding;
        }
        
        internal ResXFileRef Clone() {
            return new ResXFileRef(fileName, typeName, textFileEncoding);
        }


        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.FileName"]/*' />
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.TextFileEncoding"]/*' />
        public string TypeName {
            get {
                return typeName;
            }
        }

        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.TextFileEncoding"]/*' />
        public Encoding TextFileEncoding {
            get {
                return textFileEncoding;
            }
        }


        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.PathDifference"]/*' />
        /// <devdoc>
        ///    path1+result = path2
        ///   A string which is the relative path difference between path1 and
        ///  path2 such that if path1 and the calculated difference are used
        ///  as arguments to Combine(), path2 is returned
        /// </devdoc>
        private static string PathDifference(string path1, string path2, bool compareCase) {
            int i;
            int si = -1;

            for (i = 0; (i < path1.Length) && (i < path2.Length); ++i) {
                if ((path1[i] != path2[i]) && (compareCase || (Char.ToLower(path1[i], CultureInfo.InvariantCulture) != Char.ToLower(path2[i], CultureInfo.InvariantCulture))))
                {
                    break;

                } else if (path1[i] == Path.DirectorySeparatorChar) {
                    si = i;
                }
            }

            if (i == 0) {
                return path2;
            }
            if ((i == path1.Length) && (i == path2.Length)) {
                return String.Empty;
            }

            StringBuilder relPath = new StringBuilder();

            for (; i < path1.Length; ++i) {
                if (path1[i] == Path.DirectorySeparatorChar) {
                    relPath.Append(".."+Path.DirectorySeparatorChar);
                }
            }
            return relPath.ToString() + path2.Substring(si + 1);
        }

        
        internal void MakeFilePathRelative(string basePath) {
            
            if(basePath==null || basePath.Length == 0) {
                return;
            }
            fileName = PathDifference(basePath, fileName, false);
        }

        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.ToString"]/*' />
        public override string ToString() {
            string result = "";
            
            if(fileName.IndexOf(";") != -1 || fileName.IndexOf("\"") != -1) {
                result += ("\""+ fileName + "\";");
            } else {
                result += (fileName + ";");
            }
            result += typeName;
            if(textFileEncoding != null) {
                result += (";" + textFileEncoding.WebName);
            }
            return result;
        }

       


        /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.Converter"]/*' />
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
        [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
        public class Converter : TypeConverter {
            /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.Converter.CanConvertFrom"]/*' />
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                Type sourceType) {
                if (sourceType == typeof(string)) {
                    return true;
                }
                return false;
            }

            /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.Converter.CanConvertTo"]/*' />
            public override bool CanConvertTo(ITypeDescriptorContext context, 
                                              Type destinationType) {
                if (destinationType == typeof(string)) {
                    return true;
                }
                return false;
            }

            /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.Converter.ConvertTo"]/*' />
            public override Object ConvertTo(ITypeDescriptorContext context, 
                                             CultureInfo culture, 
                                             Object value, 
                                             Type destinationType) {
                Object created = null;
                if (destinationType == typeof(string)) {
                    created = ((ResXFileRef)value).ToString();
                }
                return created;
            }

            // "value" is the parameter name of ConvertFrom, which calls this method.
            [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
            internal static string[] ParseResxFileRefString(string stringValue) {
                string[] result = null;
                if(stringValue != null ) {
                    stringValue = stringValue.Trim();
                    string fileName;
                    string remainingString;
                    if(stringValue.StartsWith("\"")) {
                        int lastIndexOfQuote = stringValue.LastIndexOf("\"");
                        if (lastIndexOfQuote-1<0)
                            throw new ArgumentException(nameof(stringValue));
                        fileName = stringValue.Substring(1, lastIndexOfQuote-1); // remove the quotes in" ..... " 
                        if(lastIndexOfQuote+2>stringValue.Length)
                            throw new ArgumentException(nameof(stringValue));
                        remainingString = stringValue.Substring(lastIndexOfQuote+2);
                    } else {
                        int nextSemiColumn = stringValue.IndexOf(";");
                        if(nextSemiColumn == -1)
                            throw new ArgumentException(nameof(stringValue));
                        fileName = stringValue.Substring(0,nextSemiColumn);
                        if(nextSemiColumn+1>stringValue.Length)
                            throw new ArgumentException(nameof(stringValue));
                        remainingString = stringValue.Substring(nextSemiColumn+1);
                    }
                    string[] parts = remainingString.Split(new char[] {';'});
                    if(parts.Length > 1) {
                        result = new string[] { fileName, parts[0], parts[1] };
                    } else if(parts.Length > 0) {
                        result = new string[] { fileName, parts[0] };
                    } else {
                        result = new string[] { fileName };
                    }
                }
                return result;  
            }

            /// <include file='doc\ResXFileRef.uex' path='docs/doc[@for="ResXFileRef.Converter.ConvertFrom"]/*' />
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            public override Object ConvertFrom(ITypeDescriptorContext context, 
                                               CultureInfo culture,
                                               Object value) {
                Object created = null;
                string stringValue = value as string;
                if (stringValue != null) {
                    string[] parts = ResXFileRef.Converter.ParseResxFileRefString(stringValue);
                    string fileName = parts[0];
                    Type toCreate = Type.GetType(parts[1], true);

                    // special case string and byte[]
                    if(toCreate.Equals(typeof(string))) {
                        // we have a string, now we need to check the encoding
                        Encoding textFileEncoding = Encoding.Default;
                        if(parts.Length > 2) {
                            textFileEncoding = Encoding.GetEncoding(parts[2]);
                        }
                        using (StreamReader sr = new StreamReader(fileName, textFileEncoding)) {
                            created = sr.ReadToEnd();
                        }
                    } else {

                        // this is a regular file, we call it's constructor with a stream as a parameter
                        // or if it's a byte array we just return that
                        byte[] temp = null;

                        using (FileStream s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                            Debug.Assert(s != null, "Couldn't open " + fileName);
                            temp = new byte[s.Length];
                            s.Read(temp, 0, (int)s.Length);
                        }

                        if(toCreate.Equals(typeof(byte[]))) {
                            created = temp;
                        } else {
                            MemoryStream memStream = new MemoryStream(temp);
                            if(toCreate.Equals(typeof(MemoryStream))) {
                                return memStream;
                            } else if(toCreate.Equals(typeof(System.Drawing.Bitmap)) && fileName.EndsWith(".ico")) {
                                // we special case the .ico bitmaps because GDI+ destroy the alpha channel component and
                                // we don't want that to happen
                                Icon ico = new Icon(memStream);
                                created = ico.ToBitmap();
                            } else {
                                created = Activator.CreateInstance(toCreate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new Object[] {memStream}, null);
                            }
                        }
                    }
                }
                return created;
            }

        }
    }
}


