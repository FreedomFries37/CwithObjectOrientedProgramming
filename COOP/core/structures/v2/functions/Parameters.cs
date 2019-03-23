using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks.Sources;
using COOP.core.structures.v2.global;
using COOP.core.structures.v2.global.type;

namespace COOP.core.structures.v2.functions {
	public class Parameters : IEnumerable<VarDefinition>, CConvertable {

		private Dictionary<string, COOPType> varsAndTypes;

		public IEnumerable<string> vars => varsAndTypes.Keys;
		public IEnumerable<COOPType> types => varsAndTypes.Values;
		
		public List<VarDefinition> correctOrder = new  List<VarDefinition>();

		public Parameters(List<VarDefinition> list) {
			varsAndTypes = new Dictionary<string, COOPType>();
			foreach (VarDefinition varDefinition in list) {
				varsAndTypes.Add(varDefinition.name, varDefinition.type);
			}
			list = new List<VarDefinition>(list);
		}
		
		public Parameters(params VarDefinition[] list) : this(list.ToList()){ }

		public Parameters(Dictionary<string, COOPType> varsAndTypes) {
			this.varsAndTypes = new Dictionary<string,COOPType>(varsAndTypes);
			correctOrder= new List<VarDefinition>();
			foreach (var keyValuePair in varsAndTypes) {
				correctOrder.Add(new VarDefinition(keyValuePair.Value, keyValuePair.Key));
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<VarDefinition> GetEnumerator() {
			List<VarDefinition> output = new List<VarDefinition>();

			foreach (var keyValuePair in varsAndTypes) {
				output.Add(new VarDefinition(keyValuePair.Value, keyValuePair.Key));
			}

			return output.GetEnumerator();
		}

		public COOPType this[string s] => varsAndTypes[s];
		public VarDefinition this[int i] => correctOrder[i];

		public string toUsableToC() {
			string output = "(";
			
			foreach (VarDefinition varDefinition in this) {
				output += varDefinition.toUsableToC() + ",";
			}

			output = output.TrimEnd(',') + ")";

			return output;
		}
	}
}