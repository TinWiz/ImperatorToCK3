﻿using commonItems;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ImperatorToCK3.Imperator.Provinces {
	public class Provinces : IReadOnlyDictionary<ulong, Province> {
		public Provinces() { }
		public Provinces(BufferedReader reader) {
			var parser = new Parser();
			RegisterKeys(parser);
			parser.ParseStream(reader);
		}
		private void RegisterKeys(Parser parser) {
			parser.RegisterRegex(CommonRegexes.Integer, (reader, provIdStr) => {
				var newProvince = Province.Parse(reader, ulong.Parse(provIdStr));
				Add(newProvince);
			});
			parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
		}
		public void LinkPops(Pops.Pops pops) {
			var counter = Values.Sum(province => province.LinkPops(pops));
			Logger.Info($"{counter} pops linked to provinces.");
		}
		public void LinkCountries(Countries.Countries countries) {
			var counter = Values.Count(province => province.TryLinkOwnerCounty(countries));
			Logger.Info($"{counter} provinces linked to countries.");
		}

		public void Add(Province province) {
			provincesDict.Add(province.Id, province);
		}

		public bool ContainsKey(ulong key) => provincesDict.ContainsKey(key);
		public bool TryGetValue(ulong key, [MaybeNullWhen(false)] out Province value) => provincesDict.TryGetValue(key, out value);
		public IEnumerator<KeyValuePair<ulong, Province>> GetEnumerator() => provincesDict.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => provincesDict.GetEnumerator();
		public IEnumerable<ulong> Keys => provincesDict.Keys;
		public IEnumerable<Province> Values => provincesDict.Values;
		public int Count => provincesDict.Count;
		public Province this[ulong key] => provincesDict[key];
		private readonly Dictionary<ulong, Province> provincesDict = new();
	}
}
