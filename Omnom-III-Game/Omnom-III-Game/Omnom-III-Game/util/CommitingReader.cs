using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Omnom_III_Game.util {
    public class CommitingReader {
        
        private String filename;
        private String line;
        private StreamReader internalReader;

        public CommitingReader(String filename) {
            this.filename = filename;
            this.internalReader = new StreamReader(filename);
        }

        public String read() {
            if (null == line) {
                this.line = this.internalReader.ReadLine();
            }
            return line;
        }

        public String readIgnorigEmptys() {
            String line = null;
            while (null != (line = this.read())
                && "".Equals(line.Trim()))
                this.commit();
            return line;
        }

        public void commit() {
            line = null;
        }

        public void close() {
            this.internalReader.Close();
        }

    }
}
