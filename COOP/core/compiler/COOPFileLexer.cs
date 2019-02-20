using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace COOP.core.compiler {
	public class COOPFileLexer {
		private string str;
		private int index;

		private static HashSet<string> deliminators = new HashSet<string> {
			"}",
			"{",
			"(",
			")",
			".",
			",",
			"[",
			"]",
			"\"",
			"\'"
		};

		public COOPFileLexer() {
			str = "";
			index = 0;
		}

		public bool loadFile(string path) {
			StreamReader reader = new StreamReader(path);
			if (reader.EndOfStream) return false;
			string s = reader.ReadToEnd();
			reader.Close();
			str = s;
			index = 0;
			return true;
		}

		public void loadString(string s) {
			str = s;
			index = 0;
		}

		public string[] lex() {
			
			List<string> temp = new List<string>();
			temp.AddRange(Regex.Split(str, "\\s+"));

			for (int i = 0; i < temp.Count; i++) {
				string word = temp[i];

				int locDeliminator = -1;
				int deliminatorLength = 0;
				bool isDeliminator = false;
				foreach (string deliminator in deliminators) {
					if (word.Contains(deliminator)) {
						if (word == deliminator) {
							isDeliminator = true;
						} else {
							int loc = word.IndexOf(deliminator, StringComparison.Ordinal);
							if (loc < locDeliminator || locDeliminator == -1) {
								locDeliminator = loc;
								deliminatorLength = deliminator.Length;
							}

						}
					}
				}

				if (!isDeliminator && locDeliminator >= 0) {
					string x, y, z;
					x = word.Substring(0, locDeliminator);
					y = word.Substring(locDeliminator, deliminatorLength);
					z = word.Substring(locDeliminator + deliminatorLength);

					if (x != "") {
						temp[i] = x;
						if (y != "") temp.Insert(i + 1, y);
						if (z != "") temp.Insert(i + 2, z);
					} else {
						temp[i] = y;
						if (z != "") temp.Insert(i + 1, z);
					}
				}
			}
			
			List<string> output = new List<string>(temp);
			output.RemoveAll(x => x == "");
			
			return output.ToArray();
		}

		
	}
}