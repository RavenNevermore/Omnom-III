using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Omnom_III_Game.util {
    public class ContentScript {
        public class TypedAccessor<T> {
            ContentScript parent;

            public delegate T func(String value);
            public delegate String funcBack(T value);
            public func convert;
            public funcBack convertBack;

            public TypedAccessor(ContentScript parent, func convert, funcBack convertBack) {
                this.parent = parent;
                this.convert = convert;
                this.convertBack = convertBack;
            }

            public TypedListAccessor<T> this[String key] {
                get {
                    return null != this.parent[key] ?
                        new TypedListAccessor<T>(this, this.parent[key]) : null;
                }
            }


            public class TypedListAccessor<T> : ICollection<T> {
                TypedAccessor<T> parent;
                List<String> values;

                public class MyEnumerator : IEnumerator<T> {
                    TypedListAccessor<T> parent;
                    IEnumerator<String> sourceEnum;

                    public MyEnumerator(TypedListAccessor<T> parent) {
                        this.parent = parent;
                        this.sourceEnum = parent.values.GetEnumerator();
                    }

                    public T Current {
                        get { return parent.parent.convert(sourceEnum.Current); }
                    }

                    public void Dispose() {
                        this.sourceEnum.Dispose();
                    }

                    object System.Collections.IEnumerator.Current {
                        get { return this.Current; }
                    }

                    public bool MoveNext() {
                        return this.sourceEnum.MoveNext();
                    }

                    public void Reset() {
                        this.sourceEnum.Reset();
                    }
                }

                public TypedListAccessor(TypedAccessor<T> parent, List<String> values) {
                    this.parent = parent;
                    this.values = values;
                }

                public T this[int i] {
                    get { return parent.convert(this.values[i]); }
                }

                public void Add(T item) {
                    throw new NotSupportedException();
                }

                public void Clear() {
                    throw new NotSupportedException();
                }

                public bool Contains(T item) {
                    return values.Contains(this.parent.convertBack(item));
                }

                public void CopyTo(T[] array, int arrayIndex) {
                    throw new NotSupportedException();
                }

                public int Count {
                    get { return this.values.Count; }
                }

                public bool IsReadOnly {
                    get { return true; }
                }

                public bool Remove(T item) {
                    throw new NotSupportedException();
                }

                public IEnumerator<T> GetEnumerator() {
                    return new MyEnumerator(this);
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                    return this.values.GetEnumerator();
                }
            }
        }

        public String title;

        private Dictionary<String, List<String>> content;

        

        public ContentScript() {
            this.content = new Dictionary<string, List<string>>();
        }

        public ContentScript(String title) : this() {
            this.title = title;
        }

        private void set(String name, List<String> values) {
            this.content[name] = values;
        }

        public List<String> this[String key] {
            get { return this.content.ContainsKey(key) ? this.content[key] : null; }
            set { this.set(key, value); }
        }

        public TypedAccessor<float> asFloat {
            get {
                
                return new TypedAccessor<float>(this,
                    //x => { return float.Parse(x, 
                    //    System.Globalization.CultureInfo.InvariantCulture); },
                    x => {return ParserUtil.toFloat(x);},
                    x => {return "" + x;});
            }
        }

        public static ContentScript FromFile(String simpleFilename) {
            String filename = "Content/" + simpleFilename + ".content";

            CommitingReader reader = new CommitingReader(filename);
            Regex regexContentName = new Regex("(\\s*)(\\w+):(.*)");

            try {
                String line = null;
                ContentScript script = new ContentScript();
                
                while (null != (line = reader.readIgnorigEmptys())) {
                    line = line.TrimEnd();
                    if (null == script.title && line.Trim().StartsWith("# ")) {
                        script.title = getTitleFromLine(line.Trim());
                        reader.commit();
                        continue;
                    } else {
                        Match match = regexContentName.Match(line);
                        if (match.Success) {
                            int skipInLine = match.Groups[1].Length;
                            String contentname = match.Groups[2].Value;
                            String content = match.Groups[3].Value;
                            content = content.Trim();
                            List<String> contents = new List<string>();

                            reader.commit();
                            int indexDepth = -1;
                            while (null != (line = reader.readIgnorigEmptys())) {
                                if (-1 == indexDepth) {
                                    indexDepth = getLineIndex(line);
                                }
                                String inline = getIntended(skipInLine, indexDepth, line);
                                if (null == inline)
                                    break;

                                int spaces = getLineIndex(inline);
                                if (spaces >= 2) {
                                    if (!"".Equals(content)) {
                                        content += " ";
                                    }
                                    content += inline.Trim();
                                } else {
                                    if (!"".Equals(content))
                                        contents.Add(content);
                                    content = inline.Trim();
                                }

                                
                                reader.commit();
                            }

                            if (!"".Equals(content))
                                contents.Add(content);

                            script[contentname] = contents;
                        }
                    }
                }
                return script;
            } finally {
                reader.close();
            }
        }

        
        private static String getIntended(int skipStart, int indexDepth, String line) {
            if (indexDepth < 2)
                return null;

            String inline = line.Substring(skipStart);
            

            int spaces = getLineIndex(inline);
            if (spaces < 2) {
                return null;
            }
            if (spaces < indexDepth)
                indexDepth = spaces;

            return inline.Substring(skipStart + indexDepth);
        }

        private static int getLineIndex(String line) {
            char[] chars = line.ToCharArray();
            int spaces = 0;
            for (; spaces < chars.Length && ' ' == chars[spaces]; spaces++) ;
            return spaces;
        }

        private static String getTitleFromLine(String line) {
            int startIndex = 0;
            char[] chars = line.ToCharArray();
            for (; startIndex < chars.Length; startIndex++)
                if ('#' != chars[startIndex] && !Char.IsWhiteSpace(chars[startIndex]))
                    break;
            String title = line.Substring(startIndex).Trim();
            return "".Equals(title) ? null : title;
        }        
    }
}
