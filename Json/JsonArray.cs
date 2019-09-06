using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Json
{
	public sealed class JsonArray : IList<object>
	{
		IList<object> _inner;
		JsonArray(IList<object> inner, bool dummy = false)
		{
			_inner = inner;
		}
		public JsonArray(IEnumerable<object> items) { _inner = new List<object>(items); }
		public JsonArray(int capacity) { _inner = new List<object>(capacity); }
		public JsonArray() { _inner = new List<object>(); }
		public IList<object> BaseList { get { return _inner; } }
		public object this[int index] { get => _inner[index]; set => _inner[index] = value; }

		public int Count => _inner.Count;

		public bool IsReadOnly => ((IList<object>)_inner).IsReadOnly;

		public void Add(object item)
		{
			_inner.Add(item);
		}
		public void AddRange(IEnumerable<object> items)
		{
			foreach (var item in items)
				_inner.Add(item);
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(object item)
		{
			return _inner.Contains(item);
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			_inner.CopyTo(array, arrayIndex);
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		public int IndexOf(object item)
		{
			return _inner.IndexOf(item);
		}
		public int LastIndexOf(object item)
		{
			for (var i = _inner.Count - 1; -1 < i; --i)
				if (Equals(item, _inner[i]))
					return i;
			return -1;
		}

		public void Insert(int index, object item)
		{
			_inner.Insert(index, item);
		}

		public bool Remove(object item)
		{
			return _inner.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_inner.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _inner.GetEnumerator();
		}
		public T[] ToArray<T>()
		{
			return ToArray<T>(this);
		}
		public T[] ToArray<T>(Func<object, T> createItem)
		{
			return ToArray(this,createItem);
		}
		public static T[] ToArray<T>(IList<object> list, Func<object, T> createItem)
		{
			if (null != list)
			{ 
				var result = new T[list.Count];
				for (var i = 0; i < result.Length; ++i)
					result[i] = createItem(list[i]);
				return result;
			}
			return null;
		}
		public static T[] ToArray<T>(IList<object> list)
		{
			var result = new T[list.Count];
			for (var i = 0; i < result.Length; ++i)
				result[i] = (T)list[i];
			return result;
		}

		public static JsonArray Adapt(IList<object> list)
		{
			if (null == list) return null;
			var result = list as JsonArray;
			if (null == result)
				result = new JsonArray(list,false);
			return result;
		}
	}
}
