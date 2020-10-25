using System;
using System.Collections.Generic;
using System.Linq;

namespace middler.Common.SharedModels.Models
{
    public class SimpleDictionary<T> : Dictionary<string, T> {

        private readonly StringComparer _stringComparer = StringComparer.CurrentCulture;

        public SimpleDictionary() : base(StringComparer.CurrentCulture) { }

        public SimpleDictionary(bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            _stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(StringComparer stringComparer) : base(stringComparer) {
            _stringComparer = stringComparer;
        }

        public SimpleDictionary(IDictionary<string, T> dict) : base(dict, StringComparer.CurrentCulture) {

        }
        public SimpleDictionary(IDictionary<string, T> dict, bool ignoreCase) : base(dict, ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            _stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(IDictionary<string, T> dict, StringComparer stringComparer) : base(dict, stringComparer) {
            _stringComparer = stringComparer;
        }

        public SimpleDictionary(SortedList<string, T> dict) : base(dict, StringComparer.CurrentCulture) {

        }
        public SimpleDictionary(SortedList<string, T> dict, bool ignoreCase) : base(dict, ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            _stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
        }

        public SimpleDictionary(SortedList<string, T> dict, StringComparer stringComparer) : base(dict, stringComparer) {
            _stringComparer = stringComparer;
        }


        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> enumerable) : base(StringComparer.CurrentCulture) {
            foreach (var kvp in enumerable) {
                Add(kvp.Key, kvp.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> enumerable, bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            _stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            foreach (var kvp in enumerable) {
                Add(kvp.Key, kvp.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValuePair<string, T>> enumerable, StringComparer stringComparer) : base(stringComparer) {
            _stringComparer = stringComparer;
            foreach (var kvp in enumerable) {
                Add(kvp.Key, kvp.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> enumerable) : base(StringComparer.CurrentCulture) {
            foreach (var keyValuePair in enumerable) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> enumerable, bool ignoreCase) : base(ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) {
            _stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            foreach (var keyValuePair in enumerable) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public SimpleDictionary(IEnumerable<KeyValueItem<string, T>> enumerable, StringComparer stringComparer) : base(stringComparer) {
            _stringComparer = stringComparer;
            foreach (var keyValuePair in enumerable) {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }



        public bool IsEmpty() {
            return !this.Any();
        }


        public string[] GetKeys() {
            return Keys.ToArray();
        }

        public new T this[string key] {
            get => TryGetValue(key, out var val) ? val : default;
            set => base[key] = value;
        }

        public SimpleDictionary<T> Map(Action<KeyValueItem<string, T>> action) {

            var act = this.Select(kvp => {
                var kvi = new KeyValueItem<string, T>(kvp);
                action?.Invoke(kvi);
                return kvi;
            });

            return new SimpleDictionary<T>(act.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public SimpleDictionary<T> Filter(Func<KeyValueItem<string, T>, bool> filter) {

            var list = this.Select(kvp => new KeyValueItem<string, T>(kvp))
                .Where(filter);

            return new SimpleDictionary<T>(list.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), _stringComparer);
        }

        public KeyValueItem<string, T>[] Entries() {
            return this.Select(kvp => new KeyValueItem<string, T>(kvp)).ToArray();
        }


        public SimpleDictionary<T> Merge(SimpleDictionary<T> dict) {
            return Merge(new[] {this, dict});
        }

        public static SimpleDictionary<T> Merge(SimpleDictionary<T>[] dictionaries) {

            
            var dict = new SimpleDictionary<T>();

            dictionaries.ToList().ForEach(d => {
                if (d == null)
                    return;

                d.Entries().ToList().ForEach(ent => {
                    dict[ent.Key] = ent.Value;
                });
            });

            return dict;
        }

        public class KeyValueItem<TKey, TValue> {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public KeyValueItem() {

            }

            public KeyValueItem(KeyValuePair<TKey, TValue> kvp): this (kvp.Key, kvp.Value) {
           
            }

            public KeyValueItem(TKey key, TValue value) {
                Key = key;
                Value = value;
            }

            public static implicit operator KeyValuePair<TKey, TValue>(KeyValueItem<TKey, TValue> kvi) {
                return new KeyValuePair<TKey, TValue>(kvi.Key, kvi.Value);
            }
        }

    }

   
}